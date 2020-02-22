﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PointlessWaymarksCmsData;
using PointlessWaymarksCmsData.Models;
using PointlessWaymarksCmsWpfControls.LinkStreamEditor;

namespace PointlessWaymarksCmsWpfControls.Utility
{
    public static class LinkExtraction
    {
        public static async Task ExtractNewAndShowLinkStreamEditors(string toExtractFrom,
            IProgress<string> progressTracker)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (string.IsNullOrWhiteSpace(toExtractFrom))
            {
                progressTracker?.Report("Nothing to Extract From");
                return;
            }

            progressTracker?.Report("Looking for URLs");

            var allMatches = StringHelper.UrlsFromText(toExtractFrom);

            progressTracker?.Report($"Found {allMatches.Count} Matches");

            var linksToShow = new List<string>();

            var db = await Db.Context();

            foreach (var loopMatches in allMatches)
            {
                progressTracker?.Report($"Checking to see if {loopMatches} exists in database...");

                var alreadyExists = await db.LinkStreams.AnyAsync(x => x.Url == loopMatches);
                if (alreadyExists)
                {
                    progressTracker?.Report($"{loopMatches} exists in database...");
                }
                else
                {
                    progressTracker?.Report($"Adding {loopMatches} to list to show...");
                    linksToShow.Add(loopMatches);
                }
            }

            await ThreadSwitcher.ResumeForegroundAsync();

            foreach (var loopLinks in linksToShow)
            {
                progressTracker?.Report($"Launching an editor for {loopLinks}...");

                var newWindow = new LinkStreamEditorWindow(
                    new LinkStream
                    {
                        ContentId = Guid.NewGuid(),
                        CreatedBy = UserSettingsSingleton.CurrentSettings().DefaultCreatedBy,
                        CreatedOn = DateTime.Now,
                        Url = loopLinks,
                        ShowInLinkRss = false
                    }, true);

                newWindow.Show();
            }
        }
    }
}