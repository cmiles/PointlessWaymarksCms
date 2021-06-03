﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PointlessWaymarks.CmsData.CommonHtml;
using PointlessWaymarks.CmsData.Database;
using PointlessWaymarks.CmsData.Database.Models;

namespace PointlessWaymarks.CmsData.Content
{
    public static class RelatedContentReference
    {
        public static async Task ExtractAndWriteRelatedContentDbReferences(DateTime generationVersion,
            List<IContentCommon> content, PointlessWaymarksContext db, IProgress<string>? progress = null)
        {
            if (content == null || !content.Any()) return;

            foreach (var loopContent in content)
                await ExtractAndWriteRelatedContentDbReferences(generationVersion, loopContent, db, progress);
        }


        public static async Task ExtractAndWriteRelatedContentDbReferences(DateTime generationVersion,
            IContentCommon content, PointlessWaymarksContext db, IProgress<string>? progress = null)
        {
            var toAdd = new List<Guid>();

            if (content.MainPicture != null && content.MainPicture != content.ContentId)
                toAdd.Add(content.MainPicture.Value);

            var toSearch = string.Empty;

            toSearch += content.BodyContent + content.Summary;

            if (content is GeoJsonContent geoContent) toSearch += geoContent.GeoJson;

            if (content is IUpdateNotes updateContent) toSearch += updateContent.UpdateNotes;

            if (string.IsNullOrWhiteSpace(toSearch) && !toAdd.Any()) return;

            toAdd.AddRange(BracketCodeCommon.BracketCodeContentIds(toSearch));

            if (!toAdd.Any()) return;

            var dbEntries = toAdd.Distinct().Select(x => new GenerationRelatedContent
            {
                ContentOne = content.ContentId, ContentTwo = x, GenerationVersion = generationVersion
            });

            await db.GenerationRelatedContents.AddRangeAsync(dbEntries);

            await db.SaveChangesAsync();
        }

        public static async Task ExtractAndWriteRelatedContentDbReferencesFromString(Guid sourceGuid, string toSearch,
            PointlessWaymarksContext db, IProgress<string>? progress = null)
        {
            if (string.IsNullOrWhiteSpace(toSearch)) return;

            var toAdd = BracketCodeCommon.BracketCodeContentIds(toSearch);

            if (!toAdd.Any()) return;

            var dbEntries = toAdd.Select(x => new GenerationRelatedContent {ContentOne = sourceGuid, ContentTwo = x});

            await db.GenerationRelatedContents.AddRangeAsync(dbEntries);
        }

        public static async Task GenerateRelatedContentDbTable(DateTime generationVersion,
            IProgress<string>? progress = null)
        {
            //!!Content Type List!!
            var taskFunctionList = new List<Func<Task>>
            {
                async () =>
                {
                    var db = await Db.Context();
                    var files = (await db.FileContents.ToListAsync()).Cast<IContentCommon>().ToList();
                    progress?.Report($"Processing {files.Count} File Content Entries for Related Content");
                    await ExtractAndWriteRelatedContentDbReferences(generationVersion, files, db, progress);
                },
                async () =>
                {
                    var db = await Db.Context();
                    var geoJson = (await db.GeoJsonContents.ToListAsync()).Cast<IContentCommon>().ToList();
                    progress?.Report($"Processing {geoJson.Count} GeoJson Content Entries for Related Content");
                    await ExtractAndWriteRelatedContentDbReferences(generationVersion, geoJson, db, progress);
                },
                async () =>
                {
                    var db = await Db.Context();
                    var images = (await db.ImageContents.ToListAsync()).Cast<IContentCommon>().ToList();
                    progress?.Report($"Processing {images.Count} Image Content Entries for Related Content");
                    await ExtractAndWriteRelatedContentDbReferences(generationVersion, images, db, progress);
                },
                async () =>
                {
                    var db = await Db.Context();
                    var lines = (await db.LineContents.ToListAsync()).Cast<IContentCommon>().ToList();
                    progress?.Report($"Processing {lines.Count} Line Content Entries for Related Content");
                    await ExtractAndWriteRelatedContentDbReferences(generationVersion, lines, db, progress);
                },
                async () =>
                {
                    var db = await Db.Context();
                    var links = await db.LinkContents
                        .Select(x => new {x.ContentId, toCheck = x.Comments + x.Description})
                        .ToListAsync();
                    progress?.Report($"Processing {links.Count} Link Content Entries for Related Content");
                    foreach (var loopLink in links)
                        await ExtractAndWriteRelatedContentDbReferencesFromString(loopLink.ContentId, loopLink.toCheck,
                            db,
                            progress);
                },
                async () =>
                {
                    var db = await Db.Context();
                    var notes = (await db.NoteContents.ToListAsync()).Cast<IContentCommon>().ToList();
                    progress?.Report($"Processing {notes.Count} Note Content Entries for Related Content");
                    await ExtractAndWriteRelatedContentDbReferences(generationVersion, notes, db, progress);
                },
                async () =>
                {
                    var db = await Db.Context();
                    var photos = (await db.PhotoContents.ToListAsync()).Cast<IContentCommon>().ToList();
                    progress?.Report($"Processing {photos.Count} Photo Content Entries for Related Content");
                    await ExtractAndWriteRelatedContentDbReferences(generationVersion, photos, db, progress);
                },
                async () =>
                {
                    var db = await Db.Context();
                    var points = (await db.PointContents.ToListAsync()).Cast<IContentCommon>().ToList();
                    progress?.Report($"Processing {points.Count} Point Content Entries for Related Content");
                    await ExtractAndWriteRelatedContentDbReferences(generationVersion, points, db, progress);
                },
                async () =>
                {
                    var db = await Db.Context();
                    var posts = (await db.PostContents.ToListAsync()).Cast<IContentCommon>().ToList();
                    progress?.Report($"Processing {posts.Count} Post Content Entries for Related Content");
                    await ExtractAndWriteRelatedContentDbReferences(generationVersion, posts, db, progress);
                },
            };

            var taskList = taskFunctionList.Select(Task.Run).ToList();

            await Task.WhenAll(taskList);
        }
    }
}