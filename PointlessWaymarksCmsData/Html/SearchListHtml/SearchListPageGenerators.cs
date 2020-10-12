﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AngleSharp.Html;
using AngleSharp.Html.Parser;
using PointlessWaymarksCmsData.Database;
using PointlessWaymarksCmsData.Database.Models;
using PointlessWaymarksCmsData.Html.TagListHtml;
using PointlessWaymarksCmsData.Rss;

namespace PointlessWaymarksCmsData.Html.SearchListHtml
{
    public static class SearchListPageGenerators
    {
        public static void WriteAllContentCommonSearchListHtml(DateTime? generationVersion)
        {
            List<object> ContentList()
            {
                var db = Db.Context().Result;
                var fileContent = db.FileContents.Cast<object>().ToList();
                var photoContent = db.PhotoContents.Cast<object>().ToList();
                var imageContent = db.ImageContents.Where(x => x.ShowInSearch).Cast<object>().ToList();
                var postContent = db.PostContents.Cast<object>().ToList();
                var noteContent = db.NoteContents.Cast<object>().ToList();

                return fileContent.Concat(photoContent).Concat(imageContent).Concat(postContent).Concat(noteContent)
                    .OrderBy(x => ((IContentCommon) x).Title).ToList();
            }

            var fileInfo = UserSettingsSingleton.CurrentSettings().LocalSiteAllContentListFile();

            WriteSearchListHtml(ContentList, fileInfo, "All Content",
                UserSettingsSingleton.CurrentSettings().AllContentRssUrl(), generationVersion);
            RssBuilder.WriteContentCommonListRss(ContentList().Cast<IContentCommon>().ToList(),
                UserSettingsSingleton.CurrentSettings().LocalSiteAllContentRssFile(), "All Content");
        }

        public static void WriteFileContentListHtml(DateTime? generationVersion)
        {
            List<object> ContentList()
            {
                var db = Db.Context().Result;
                return db.FileContents.OrderBy(x => x.Title).Cast<object>().ToList();
            }

            var fileInfo = UserSettingsSingleton.CurrentSettings().LocalSiteFileListFile();

            WriteSearchListHtml(ContentList, fileInfo, "Files", UserSettingsSingleton.CurrentSettings().FileRssUrl(),
                generationVersion);
            RssBuilder.WriteContentCommonListRss(ContentList().Cast<IContentCommon>().ToList(),
                UserSettingsSingleton.CurrentSettings().LocalSiteFileRssFile(), "Files");
        }


        public static void WriteImageContentListHtml(DateTime? generationVersion)
        {
            List<object> ContentList()
            {
                var db = Db.Context().Result;
                return db.ImageContents.Where(x => x.ShowInSearch).OrderBy(x => x.Title).Cast<object>().ToList();
            }

            var fileInfo = UserSettingsSingleton.CurrentSettings().LocalSiteImageListFile();

            WriteSearchListHtml(ContentList, fileInfo, "Images", UserSettingsSingleton.CurrentSettings().ImageRssUrl(),
                generationVersion);
            RssBuilder.WriteContentCommonListRss(ContentList().Cast<IContentCommon>().ToList(),
                UserSettingsSingleton.CurrentSettings().LocalSiteImageRssFile(), "Images");
        }

        public static void WriteNoteContentListHtml(DateTime? generationVersion)
        {
            List<object> ContentList()
            {
                var db = Db.Context().Result;
                return db.NoteContents.ToList().OrderByDescending(x => x.Title).Cast<object>().ToList();
            }

            var fileInfo = UserSettingsSingleton.CurrentSettings().LocalSiteNoteListFile();

            WriteSearchListHtml(ContentList, fileInfo, "Notes", UserSettingsSingleton.CurrentSettings().NoteRssUrl(),
                generationVersion);
            RssBuilder.WriteContentCommonListRss(ContentList().Cast<IContentCommon>().ToList(),
                UserSettingsSingleton.CurrentSettings().LocalSiteNoteRssFile(), "Notes");
        }

        public static void WritePhotoContentListHtml(DateTime? generationVersion)
        {
            List<object> ContentList()
            {
                var db = Db.Context().Result;
                return db.PhotoContents.OrderBy(x => x.Title).Cast<object>().ToList();
            }

            var fileInfo = UserSettingsSingleton.CurrentSettings().LocalSitePhotoListFile();

            WriteSearchListHtml(ContentList, fileInfo, "Photos", UserSettingsSingleton.CurrentSettings().PhotoRssUrl(),
                generationVersion);
            RssBuilder.WriteContentCommonListRss(ContentList().Cast<IContentCommon>().ToList(),
                UserSettingsSingleton.CurrentSettings().LocalSitePhotoRssFile(), "Photos");
        }

        public static void WritePostContentListHtml(DateTime? generationVersion)
        {
            List<object> ContentList()
            {
                var db = Db.Context().Result;
                return db.PostContents.OrderBy(x => x.Title).Cast<object>().ToList();
            }

            var fileInfo = UserSettingsSingleton.CurrentSettings().LocalSitePostListFile();

            WriteSearchListHtml(ContentList, fileInfo, "Posts", UserSettingsSingleton.CurrentSettings().PostsRssUrl(),
                generationVersion);
            RssBuilder.WriteContentCommonListRss(ContentList().Cast<IContentCommon>().ToList(),
                UserSettingsSingleton.CurrentSettings().LocalSitePostRssFile(), "Posts");
        }

        public static void WriteSearchListHtml(Func<List<object>> dbFunc, FileInfo fileInfo, string titleAdd,
            string rssUrl, DateTime? generationVersion)
        {
            var htmlModel = new SearchListPage(rssUrl)
            {
                ContentFunction = dbFunc, ListTitle = titleAdd, GenerationVersion = generationVersion
            };

            var htmlTransform = htmlModel.TransformText();

            var parser = new HtmlParser();
            var htmlDoc = parser.ParseDocument(htmlTransform);

            var stringWriter = new StringWriter();
            htmlDoc.ToHtml(stringWriter, new PrettyMarkupFormatter());

            var htmlString = stringWriter.ToString();

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
                fileInfo.Refresh();
            }

            File.WriteAllText(fileInfo.FullName, htmlString);
        }

        public static void WriteTagList(DateTime generationVersion, IProgress<string> progress)
        {
            progress?.Report("Tag Pages - Getting Tag Data For Search");
            var tags = Db.TagSlugsAndContentList(false, true, progress).Result;

            var allTags = new TagListPage {ContentFunction = () => tags, GenerationVersion = generationVersion};

            progress?.Report("Tag Pages - Writing All Tag Data");
            allTags.WriteLocalHtml();
        }

        public static void WriteTagListAndTagPages(DateTime? generationVersion, IProgress<string> progress)
        {
            progress?.Report("Tag Pages - Getting Tag Data For Search");
            var tags = Db.TagSlugsAndContentList(false, true, progress).Result;

            var allTags = new TagListPage {ContentFunction = () => tags, GenerationVersion = generationVersion};

            progress?.Report("Tag Pages - Writing All Tag Data");
            allTags.WriteLocalHtml();

            progress?.Report("Tag Pages - Getting Tag Data For Page Generation");
            //Tags is reset - above for tag search we don't include tags from pages that are hidden from search - but to
            //ensure all tags have a page we generate pages from all tags (if an image excluded from search had a unique
            //tag we need a page for the links on that page, excluded from search does not mean 'unreachable'...)
            var pageTags = Db.TagSlugsAndContentList(true, false, progress).Result;

            var loopCount = 0;

            foreach (var loopTags in pageTags)
            {
                loopCount++;

                if (loopCount % 30 == 0)
                    progress?.Report($"Generating Tag Page {loopTags.tag} - {loopCount} of {tags.Count}");

                WriteSearchListHtml(() => loopTags.contentObjects,
                    UserSettingsSingleton.CurrentSettings().LocalSiteTagListFileInfo(loopTags.tag),
                    $"Tag - {loopTags.tag}", string.Empty, generationVersion);
            }
        }

        public static void WriteTagPage(string tag, List<dynamic> content, DateTime? generationVersion)
        {
            WriteSearchListHtml(() => content, UserSettingsSingleton.CurrentSettings().LocalSiteTagListFileInfo(tag),
                $"Tag - {tag}", string.Empty, generationVersion);
        }
    }
}