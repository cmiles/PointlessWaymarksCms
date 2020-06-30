﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.Xmp;
using PointlessWaymarksCmsData.FolderStructureAndGeneratedContent;
using PointlessWaymarksCmsData.JsonFiles;
using PointlessWaymarksCmsData.Models;
using PointlessWaymarksCmsData.PhotoHtml;
using PointlessWaymarksCmsData.Pictures;
using XmpCore;

namespace PointlessWaymarksCmsData.Generation
{
    public static class PhotoGenerator
    {
        public static void GenerateHtml(PhotoContent toGenerate, IProgress<string> progress)
        {
            progress?.Report($"Photo Content - Generate HTML for {toGenerate.Title}");

            var htmlContext = new SinglePhotoPage(toGenerate);

            htmlContext.WriteLocalHtml();
        }

        public static async Task<(GenerationReturn, PhotoContent)> PhotoMetadataToPhotoContent(FileInfo selectedFile,
            string createdBy, IProgress<string> progress)
        {
            progress?.Report("Starting Metadata Processing");

            selectedFile.Refresh();

            if (!selectedFile.Exists) return (await GenerationReturn.Error("File Does Not Exist?"), null);

            var toReturn = new PhotoContent
            {
                OriginalFileName = selectedFile.Name,
                ContentId = Guid.NewGuid(),
                CreatedBy =
                    string.IsNullOrWhiteSpace(createdBy)
                        ? UserSettingsSingleton.CurrentSettings().DefaultCreatedBy
                        : createdBy.Trim(),
                CreatedOn = DateTime.Now,
                ContentVersion = DateTime.Now.ToUniversalTime()
            };

            progress?.Report("Getting Directories");

            var exifSubIfDirectory = ImageMetadataReader.ReadMetadata(selectedFile.FullName)
                .OfType<ExifSubIfdDirectory>().FirstOrDefault();
            var exifDirectory = ImageMetadataReader.ReadMetadata(selectedFile.FullName).OfType<ExifIfd0Directory>()
                .FirstOrDefault();
            var iptcDirectory = ImageMetadataReader.ReadMetadata(selectedFile.FullName).OfType<IptcDirectory>()
                .FirstOrDefault();
            var gpsDirectory = ImageMetadataReader.ReadMetadata(selectedFile.FullName).OfType<GpsDirectory>()
                .FirstOrDefault();
            var xmpDirectory = ImageMetadataReader.ReadMetadata(selectedFile.FullName).OfType<XmpDirectory>()
                .FirstOrDefault();

            toReturn.PhotoCreatedBy = exifDirectory?.GetDescription(ExifDirectoryBase.TagArtist) ?? string.Empty;

            var createdOn = exifSubIfDirectory?.GetDescription(ExifDirectoryBase.TagDateTimeOriginal);
            if (string.IsNullOrWhiteSpace(createdOn))
            {
                var gpsDateTime = DateTime.MinValue;
                if (gpsDirectory?.TryGetGpsDate(out gpsDateTime) ?? false)
                {
                    if (gpsDateTime != DateTime.MinValue) toReturn.PhotoCreatedOn = gpsDateTime.ToLocalTime();
                }
                else
                {
                    toReturn.PhotoCreatedOn = DateTime.Now;
                }
            }
            else
            {
                var createdOnParsed = DateTime.TryParseExact(
                    exifSubIfDirectory?.GetDescription(ExifDirectoryBase.TagDateTimeOriginal), "yyyy:MM:dd HH:mm:ss",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate);

                toReturn.PhotoCreatedOn = createdOnParsed ? parsedDate : DateTime.Now;
            }

            toReturn.Folder = toReturn.PhotoCreatedOn.Year.ToString("F0");

            var isoString = exifSubIfDirectory?.GetDescription(ExifDirectoryBase.TagIsoEquivalent);
            if (!string.IsNullOrWhiteSpace(isoString)) toReturn.Iso = int.Parse(isoString);

            toReturn.CameraMake = exifDirectory?.GetDescription(ExifDirectoryBase.TagMake) ?? string.Empty;
            toReturn.CameraModel = exifDirectory?.GetDescription(ExifDirectoryBase.TagModel) ?? string.Empty;
            toReturn.FocalLength = exifSubIfDirectory?.GetDescription(ExifDirectoryBase.TagFocalLength) ?? string.Empty;

            toReturn.Lens = exifSubIfDirectory?.GetDescription(ExifDirectoryBase.TagLensModel) ?? string.Empty;

            if (toReturn.Lens == string.Empty || toReturn.Lens == "----")
                toReturn.Lens = xmpDirectory?.XmpMeta?.GetProperty(XmpConstants.NsExifAux, "Lens")?.Value ??
                                string.Empty;
            if (toReturn.Lens == string.Empty || toReturn.Lens == "----")
            {
                toReturn.Lens =
                    xmpDirectory?.XmpMeta?.GetProperty(XmpConstants.NsCameraraw, "LensProfileName")?.Value ??
                    string.Empty;

                if (toReturn.Lens.StartsWith("Adobe ("))
                {
                    toReturn.Lens = toReturn.Lens.Substring(7, toReturn.Lens.Length - 7);
                    if (toReturn.Lens.EndsWith(")"))
                        toReturn.Lens = toReturn.Lens.Substring(0, toReturn.Lens.Length - 1);
                }
            }

            if (toReturn.Lens == "----") toReturn.Lens = string.Empty;

            toReturn.Aperture = exifSubIfDirectory?.GetDescription(ExifDirectoryBase.TagAperture) ?? string.Empty;
            toReturn.License = exifDirectory?.GetDescription(ExifDirectoryBase.TagCopyright) ?? string.Empty;

            var shutterValue = new Rational();
            if (exifSubIfDirectory?.TryGetRational(37377, out shutterValue) ?? false)
                toReturn.ShutterSpeed = ExifHelpers.ShutterSpeedToHumanReadableString(shutterValue);
            else
                toReturn.ShutterSpeed = string.Empty;

            //The XMP data - vs the IPTC - will hold the full Title for a very long title (the IPTC will be truncated) - 
            //for a 'from Lightroom with no other concerns' export Title makes the most sense, but there are other possible
            //metadata fields to pull from that could be relevant in other contexts.
            toReturn.Title = xmpDirectory?.XmpMeta?.GetArrayItem(XmpConstants.NsDC, "title", 1)?.Value;

            if (string.IsNullOrWhiteSpace(toReturn.Title))
                toReturn.Title = iptcDirectory?.GetDescription(IptcDirectory.TagObjectName) ?? string.Empty;
            //Use a variety of guess on common file names and make that the title - while this could result in an initial title 
            //like DSC001 style out of camera names but after having experimented with loading files I think 'default' is better
            //than an invalid blank.
            if (string.IsNullOrWhiteSpace(toReturn.Title))
                toReturn.Title = Path.GetFileNameWithoutExtension(selectedFile.Name).Replace("-", " ").Replace("_", " ")
                    .SplitCamelCase();

            toReturn.Summary = iptcDirectory?.GetDescription(IptcDirectory.TagObjectName) ?? string.Empty;

            //2020/3/22 - This matches a personal naming pattern where pictures 'always' start with 4 digit year 2 digit month
            if (!string.IsNullOrWhiteSpace(toReturn.Title))
            {
                if (toReturn.Title.StartsWith("2"))
                {
                    var possibleTitleDate =
                        Regex.Match(toReturn.Title, @"\A(?<possibleDate>\d\d\d\d[\s-]\d\d[\s-]*).*",
                            RegexOptions.IgnoreCase).Groups["possibleDate"].Value;
                    if (!string.IsNullOrWhiteSpace(possibleTitleDate))
                        try
                        {
                            var tempDate = new DateTime(int.Parse(possibleTitleDate.Substring(0, 4)),
                                int.Parse(possibleTitleDate.Substring(5, 2)), 1);

                            toReturn.Summary =
                                $"{toReturn.Title.Substring(possibleTitleDate.Length, toReturn.Title.Length - possibleTitleDate.Length)}.";
                            toReturn.Title =
                                $"{tempDate:yyyy} {tempDate:MMMM} {toReturn.Title.Substring(possibleTitleDate.Length, toReturn.Title.Length - possibleTitleDate.Length)}";
                            toReturn.Folder = $"{tempDate:yyyy}";

                            progress?.Report("Title updated based on 2yyy MM start pattern for file name");
                        }
                        catch
                        {
                            progress?.Report("Did not successfully parse 2yyy MM start pattern for file name");
                        }
                }
                else if (toReturn.Title.StartsWith("0") || toReturn.Title.StartsWith("1"))
                {
                    try
                    {
                        if (Regex.IsMatch(toReturn.Title, @"\A[01]\d\d\d\s.*", RegexOptions.IgnoreCase))
                        {
                            var year = int.Parse(toReturn.Title.Substring(0, 2));
                            var month = int.Parse(toReturn.Title.Substring(2, 2));

                            var tempDate = year < 20
                                ? new DateTime(2000 + year, month, 1)
                                : new DateTime(1900 + year, month, 1);

                            toReturn.Summary = $"{toReturn.Title.Substring(5, toReturn.Title.Length - 5)}.";
                            toReturn.Title =
                                $"{tempDate:yyyy} {tempDate:MMMM} {toReturn.Title.Substring(5, toReturn.Title.Length - 5)}";
                            toReturn.Folder = $"{tempDate:yyyy}";

                            progress?.Report("Title updated based on YYMM start pattern for file name");
                        }
                    }
                    catch
                    {
                        progress?.Report("Did not successfully parse YYMM start pattern for file name");
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(toReturn.Title))
                toReturn.Title = Regex.Replace(toReturn.Title, @"\s+", " ").TrimNullSafe();

            //Order is important here - the title supplies the summary in the code above - but overwrite that if there is a 
            //description.
            var description = exifDirectory?.GetDescription(ExifDirectoryBase.TagImageDescription) ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(description))
                toReturn.Summary = description;

            toReturn.Slug = SlugUtility.Create(true, toReturn.Title);

            var xmpSubjectKeywordList = new List<string>();

            var xmpSubjectArrayItemCount = xmpDirectory?.XmpMeta?.CountArrayItems(XmpConstants.NsDC, "subject");

            if (xmpSubjectArrayItemCount != null)
                for (var i = 1; i <= xmpSubjectArrayItemCount; i++)
                {
                    var subjectArrayItem = xmpDirectory?.XmpMeta?.GetArrayItem(XmpConstants.NsDC, "subject", i);
                    if (subjectArrayItem == null || string.IsNullOrWhiteSpace(subjectArrayItem.Value)) continue;
                    xmpSubjectKeywordList.AddRange(subjectArrayItem.Value.Replace(";", ",").Split(",")
                        .Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()));
                }

            xmpSubjectKeywordList = xmpSubjectKeywordList.Distinct().ToList();

            var keywordTagList = new List<string>();

            var keywordValue = iptcDirectory?.GetDescription(IptcDirectory.TagKeywords)?.Replace(";", ",") ??
                               string.Empty;

            if (!string.IsNullOrWhiteSpace(keywordValue))
                keywordTagList.AddRange(keywordValue.Replace(";", ",").Split(",")
                    .Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()));

            if (xmpSubjectKeywordList.Count == 0 && keywordTagList.Count == 0)
                toReturn.Tags = string.Empty;
            else if (xmpSubjectKeywordList.Count >= keywordTagList.Count)
                toReturn.Tags = string.Join(",", xmpSubjectKeywordList);
            else
                toReturn.Tags = string.Join(",", keywordTagList);

            if (!string.IsNullOrWhiteSpace(toReturn.Tags))
                toReturn.Tags = Db.TagListJoin(Db.TagListParse(toReturn.Tags));

            return (await GenerationReturn.Success($"Parsed Photo Metadata for {selectedFile.FullName} without error"),
                toReturn);
        }


        public static async Task<GenerationReturn> SaveAndGenerateHtml(PhotoContent toSave, FileInfo selectedFile,
            bool overwriteExistingFiles, IProgress<string> progress)
        {
            var validationReturn = await Validate(toSave, selectedFile);

            if (validationReturn.HasError) return validationReturn;

            WriteSelectedFileToMediaArchive(selectedFile);
            await Db.SavePhotoContent(toSave);
            await WriteSelectedFileFromMediaArchiveToLocalSite(toSave, overwriteExistingFiles, progress);
            GenerateHtml(toSave, progress);
            await Export.WriteLocalDbJson(toSave);

            DataNotifications.PhotoContentDataNotificationEventSource.Raise("Photo Generator",
                new DataNotificationEventArgs
                {
                    UpdateType = DataNotificationUpdateType.LocalContent,
                    ContentIds = new List<Guid> {toSave.ContentId}
                });

            return await GenerationReturn.Success($"Saved and Generated Content And Html for {toSave.Title}");
        }

        public static async Task<GenerationReturn> Validate(PhotoContent photoContent, FileInfo selectedFile)
        {
            var rootDirectoryCheck = await UserSettingsUtilities.ValidateLocalSiteRootDirectory();

            if (!rootDirectoryCheck.Item1)
                return await GenerationReturn.Error($"Problem with Root Directory: {rootDirectoryCheck.Item2}",
                    photoContent.ContentId);

            var mediaArchiveCheck = await UserSettingsUtilities.ValidateLocalMediaArchive();
            if (!mediaArchiveCheck.Item1)
                return await GenerationReturn.Error($"Problem with Media Archive: {mediaArchiveCheck.Item2}",
                    photoContent.ContentId);

            var commonContentCheck = CommonContentValidation.ValidateContentCommon(photoContent);
            if (!commonContentCheck.Item1)
                return await GenerationReturn.Error(commonContentCheck.Item2, photoContent.ContentId);

            selectedFile.Refresh();

            if (photoContent == null) return await GenerationReturn.Error("Photo Content is Null?");

            if (!selectedFile.Exists)
                return await GenerationReturn.Error("Selected File doesn't exist?", photoContent.ContentId);

            if (!FolderFileUtility.IsNoUrlEncodingNeededFilename(selectedFile.Name))
                return await GenerationReturn.Error("Limit File Names to A-Z a-z - . _", photoContent.ContentId);

            if (!FolderFileUtility.PhotoFileTypeIsSupported(selectedFile))
                return await GenerationReturn.Error("The file doesn't appear to be a supported file type.",
                    photoContent.ContentId);

            if (await (await Db.Context()).PhotoFilenameExistsInDatabase(selectedFile.Name, photoContent.ContentId))
                return await GenerationReturn.Error(
                    "This filename already exists in the database - photo file names must be unique.",
                    photoContent.ContentId);

            if (await (await Db.Context()).SlugExistsInDatabase(photoContent.Slug, photoContent.ContentId))
                return await GenerationReturn.Error("This slug already exists in the database - slugs must be unique.",
                    photoContent.ContentId);

            return await GenerationReturn.Success("Photo Content Validation Successful");
        }

        public static async Task WriteSelectedFileFromMediaArchiveToLocalSite(PhotoContent photoContent,
            bool forcedResizeOverwriteExistingFiles, IProgress<string> progress)
        {
            var userSettings = UserSettingsSingleton.CurrentSettings();

            var sourceFile = new FileInfo(Path.Combine(userSettings.LocalMediaArchivePhotoDirectory().FullName,
                photoContent.OriginalFileName));

            var targetFile = new FileInfo(Path.Combine(
                userSettings.LocalSitePhotoContentDirectory(photoContent).FullName, photoContent.OriginalFileName));

            if (targetFile.Exists && forcedResizeOverwriteExistingFiles)
            {
                targetFile.Delete();
                targetFile.Refresh();
            }

            if (!targetFile.Exists) sourceFile.CopyTo(targetFile.FullName);

            PictureResizing.DeleteSupportedPhotoFilesInPhotoDirectoryOtherThanOriginalFile(photoContent, progress);

            PictureResizing.CleanDisplayAndSrcSetFilesInPhotoDirectory(photoContent, forcedResizeOverwriteExistingFiles,
                progress);

            await PictureResizing.ResizeForDisplayAndSrcset(photoContent, forcedResizeOverwriteExistingFiles, progress);
        }

        public static void WriteSelectedFileToMediaArchive(FileInfo selectedFile)
        {
            var userSettings = UserSettingsSingleton.CurrentSettings();
            var destinationFileName = Path.Combine(userSettings.LocalMediaArchivePhotoDirectory().FullName,
                selectedFile.Name);
            if (destinationFileName == selectedFile.FullName) return;

            var destinationFile = new FileInfo(destinationFileName);

            if (destinationFile.Exists) destinationFile.Delete();

            selectedFile.CopyTo(destinationFileName);
        }

        public static async Task<GenerationReturn> WriteSelectedFileToMediaArchive(FileInfo selectedFile,
            bool replaceExisting)
        {
            var userSettings = UserSettingsSingleton.CurrentSettings();
            var destinationFileName = Path.Combine(userSettings.LocalMediaArchivePhotoDirectory().FullName,
                selectedFile.Name);
            if (destinationFileName == selectedFile.FullName && !replaceExisting)
                return await GenerationReturn.Success("Photo is already in Media Archive");

            var destinationFile = new FileInfo(destinationFileName);

            if (destinationFile.Exists) destinationFile.Delete();

            selectedFile.CopyTo(destinationFileName);

            return await GenerationReturn.Success("Photo is copied to Media Archive");
        }
    }
}