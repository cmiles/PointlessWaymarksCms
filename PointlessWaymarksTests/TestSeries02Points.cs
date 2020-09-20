﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using PointlessWaymarksCmsData;
using PointlessWaymarksCmsData.Database;
using PointlessWaymarksCmsData.Spatial;
using PointlessWaymarksCmsData.Spatial.Elevation;

namespace PointlessWaymarksTests
{
    public class TestSeries02Points
    {
        public const string TestSiteName = "Grand Canyon Rim Notes";
        public const string TestDefaultCreatedBy = "GC Ghost Writer";
        public const string TestSiteAuthors = "Pointless Waymarks Grand Canyon 'Testers'";
        public const string TestSiteEmailTo = "Grand@Canyon.Fake";

        public const string TestSiteKeywords = "grand canyon, points, places";

        public const string TestSummary = "'Testing' in the beautiful Grand Canyon";

        public static UserSettings TestSiteSettings;


        [OneTimeSetUp]
        public async Task A00_CreateTestSite()
        {
            var outSettings = await UserSettingsUtilities.SetupNewSite(
                $"GrandCanyonTestSite-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}", DebugTrackers.DebugProgressTracker());
            TestSiteSettings = outSettings;
            TestSiteSettings.SiteName = TestSiteName;
            TestSiteSettings.DefaultCreatedBy = TestDefaultCreatedBy;
            TestSiteSettings.SiteAuthors = TestSiteAuthors;
            TestSiteSettings.SiteEmailTo = TestSiteEmailTo;
            TestSiteSettings.SiteKeywords = TestSiteKeywords;
            TestSiteSettings.SiteSummary = TestSummary;
            TestSiteSettings.SiteUrl = "GrandCanyonRimNotes.com";
            await TestSiteSettings.EnsureDbIsPresent(DebugTrackers.DebugProgressTracker());
            await TestSiteSettings.WriteSettings();
        }

        [Test]
        public void A01_TestSiteBasicStructureCheck()
        {
            Assert.True(TestSiteSettings.LocalMediaArchiveFileDirectory().Exists);
            Assert.True(TestSiteSettings.LocalMediaArchiveImageDirectory().Exists);
            Assert.True(TestSiteSettings.LocalMediaArchiveFileDirectory().Exists);
            Assert.True(TestSiteSettings.LocalMediaArchivePhotoDirectory().Exists);
            Assert.True(TestSiteSettings.LocalSiteDirectory().Exists);
        }

        [Test]
        public async Task B10_YumaPointLoadTest()
        {
            await GrandCanyonPointInfo.PointTest(GrandCanyonPointInfo.YumaPointContent01);
        }

        [Test]
        public async Task B11_YumaPointElevationServiceTest()
        {
            var httpClient = new HttpClient();

            var elevation = await ElevationService.OpenTopoNedElevation(httpClient,
                GrandCanyonPointInfo.YumaPointContent02.Latitude, GrandCanyonPointInfo.YumaPointContent02.Longitude,
                DebugTrackers.DebugProgressTracker());

            Assert.NotNull(elevation, "Elevation returned null");

            var concreteElevation = Math.Round(elevation.Value.MetersToFeet(), 0);

            Assert.AreEqual(GrandCanyonPointInfo.YumaPointContent02.Elevation, concreteElevation,
                "Service Elevation does not match");
        }

        [Test]
        public async Task B12_YumaPointUpdateTest()
        {
            var db = await Db.Context();
            var currentYumaPoint = db.PointContents.Single(x => x.Slug == "yuma-point");

            var yumaPointUpdate = GrandCanyonPointInfo.YumaPointContent02;
            yumaPointUpdate.ContentId = currentYumaPoint.ContentId;

            await GrandCanyonPointInfo.PointTest(yumaPointUpdate);
        }
    }
}