﻿using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using JetBrains.Annotations;
using Omu.ValueInjecter;
using PointlessWaymarks.CmsData;
using PointlessWaymarks.CmsWpfControls.Utility.Aws;
using PointlessWaymarks.WpfCommon.Commands;
using PointlessWaymarks.WpfCommon.Status;
using PointlessWaymarks.WpfCommon.ThreadSwitcher;

namespace PointlessWaymarks.CmsWpfControls.UserSettingsEditor
{
    public class UserSettingsEditorContext : INotifyPropertyChanged
    {
        private Command _deleteAwsCredentials;
        private UserSettings _editorSettings;
        private Command _enterAwsCredentials;
        private List<string> _regionChoices;
        private Command _saveSettingsCommand;
        private StatusControlContext _statusContext;

        public UserSettingsEditorContext(StatusControlContext statusContext, UserSettings toLoad)
        {
            StatusContext = statusContext ?? new StatusControlContext();

            StatusContext.RunFireAndForgetTaskWithUiToastErrorReturn(async () => await LoadData(toLoad));
        }

        public Command DeleteAwsCredentials
        {
            get => _deleteAwsCredentials;
            set
            {
                if (Equals(value, _deleteAwsCredentials)) return;
                _deleteAwsCredentials = value;
                OnPropertyChanged();
            }
        }

        public UserSettings EditorSettings
        {
            get => _editorSettings;
            set
            {
                if (Equals(value, _editorSettings)) return;
                _editorSettings = value;
                OnPropertyChanged();
            }
        }

        public Command EnterAwsCredentials
        {
            get => _enterAwsCredentials;
            set
            {
                if (Equals(value, _enterAwsCredentials)) return;
                _enterAwsCredentials = value;
                OnPropertyChanged();
            }
        }

        public string HelpMarkdownBingMapsApiKey =>
            "If you have a Bing Maps API key you can enter it here - this will allow access to some Bing layers in the maps. This is NOT required for maps to be functional.";

        public string HelpMarkdownCalTopoMapsApiKey =>
            "If you have a CalTopo Maps API key you can enter it here - this will allow access to some CalTopo layers in the maps. This is NOT required for maps to be functional.";

        public string HelpMarkdownDefaultCreatedByName =>
            "Set this to fill in a Default Created By when creating new content. Example 'Charles Miles'.";

        public string HelpMarkdownDefaultLatitudeLongitude =>
            "The default Latitude and Longitude (in dd.dddd format) are used as the default starting point for maps. Example Latitude '32.4432', Longitude '-110.7577'.";

        public string HelpMarkdownDomain =>
            "This is the subdomain + domain and optionally port - for example 'PointlessWaymarks.com'. This software will " +
            "prepend protocol and append paths to this.";

        public string HelpMarkdownLocalMediaArchive =>
            "The original/source media files are stored separately from the generated site - this (local) directory is very " +
            "important because the generating the site depends on the settings file, database and the contents of this " +
            "directory. Ideally you should backup this directory.";

        public string HelpMarkdownLocalSiteRootDirectory =>
            "This is the directory where the local generated site will be placed - this should be a local directory, the " +
            "intention is that this program will create a local generated site to this directory and provide tools " +
            "to help you sync that to a server if you want to publish a public version of the site.";

        public string HelpMarkdownPdfToCairo =>
            "pdftocairo transforms Pdf files to other formats including jpg files - this program " +
            "can use it to automatically generate an image of a page of a pdf, very " +
            "useful when adding PDFs to the File Content. However pdftocariro is not included with this " +
            "program... On Windows the easiest way to get pdftocairo is to install [MiKTeX](https://miktex.org/download). " +
            "Once installed the setting above should be the folder where pdftocairo.exe is located - " +
            "for example C:\\MiKTeX 2.9\\miktex\\bin";

        public string HelpMarkdownPinboardApiKey =>
            "Sites, pages and links on the internet are constantly disappearing - [Pinboard](https://pinboard.in/) is a bookmarking site that has options to archive links for your personal use and this software has some functions that help you send links to Pinboard if you enter your Api Key. This is OPTIONAL - nothing in this software requires Pinboard.";

        public string HelpMarkdownS3Information =>
            "This is NOT required. Amazon S3 - especially behind a service like Cloudflare - can be an excellent way to host a static site like this program generates. This program can help you upload files and maintain files on S3, but to do so you must provide some information - S3 Bucket Name (this will often match your domain name), S3 Bucket Region and AWS Site Credentials (these are not shown and are stored securely by windows - these are NOT stored in the database or in the settings file).";


        public string HelpMarkdownSiteAuthors =>
            "A value for the site creators/authors - for example " +
            "'Pointless Waymarks Team'.";

        public string HelpMarkdownSiteDirAttribute =>
            "Dir attribute indicating text direction for the site - see the [dir attribute on MDN](https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/dir) for more information.";

        public string HelpMarkdownSiteEmailTo =>
            "An Email To for the site - example 'PointlessWaymarks@gmail.com'.";

        public string HelpMarkdownSiteKeywords =>
            "Used in as the tags for the overall/entire site - for example " +
            "'outdoors,hiking,running,landscape,photography,history'.";

        public string HelpMarkdownSiteLangAttribute =>
            "Lang attribute indicating the default language for the site - see [lang attribute on MDN](https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/lang) for more information.";

        public string HelpMarkdownSiteName =>
            "The 'human readable' Site Name - for example 'Pointless Waymarks'.";

        public string HelpMarkdownSubtitleSummary =>
            "Used as a sub-title and site summary - example 'Ramblings, Questionable Geographics, Photographic Half-truths'.";

        public List<string> RegionChoices
        {
            get => _regionChoices;
            set
            {
                if (Equals(value, _regionChoices)) return;
                _regionChoices = value;
                OnPropertyChanged();
            }
        }

        public Command SaveSettingsCommand
        {
            get => _saveSettingsCommand;
            set
            {
                if (Equals(value, _saveSettingsCommand)) return;
                _saveSettingsCommand = value;
                OnPropertyChanged();
            }
        }

        public StatusControlContext StatusContext
        {
            get => _statusContext;
            set
            {
                if (Equals(value, _statusContext)) return;
                _statusContext = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task LoadData(UserSettings toLoad)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            RegionChoices = RegionEndpoint.EnumerableAllRegions.Select(x => x.SystemName).ToList();

            SaveSettingsCommand = StatusContext.RunBlockingTaskCommand(SaveSettings);
            EnterAwsCredentials = StatusContext.RunBlockingTaskCommand(UserAwsKeyAndSecretEntry);
            DeleteAwsCredentials = StatusContext.RunBlockingActionCommand(AwsCredentials.RemoveAwsSiteCredentials);

            EditorSettings = toLoad;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task SaveSettings()
        {
            var currentFile = UserSettingsUtilities.SettingsFile();

            if (currentFile.Exists) currentFile.Delete();

            currentFile.Refresh();

            var writeStream = File.Create(currentFile.FullName);

            await JsonSerializer.SerializeAsync(writeStream, EditorSettings, null, CancellationToken.None);

            UserSettingsSingleton.CurrentSettings().InjectFrom(EditorSettings);
        }

        public async Task UserAwsKeyAndSecretEntry()
        {
            var newKeyEntry = await StatusContext.ShowStringEntry("AWS Access Key",
                "Enter the AWS Access Key", string.Empty);

            if (!newKeyEntry.Item1)
            {
                StatusContext.ToastWarning("AWS Credential Entry Canceled");
                return;
            }

            var cleanedKey = newKeyEntry.Item2.TrimNullToEmpty();

            if (string.IsNullOrWhiteSpace(cleanedKey))
            {
                StatusContext.ToastError("AWS Credential Entry Canceled - key can not be blank");
                return;
            }

            var newSecretEntry = await StatusContext.ShowStringEntry("AWS Secret Access Key",
                "Enter the AWS Secret Access Key", string.Empty);

            if (!newSecretEntry.Item1)
            {
                StatusContext.ToastWarning("AWS Credential Entry Canceled");
                return;
            }

            var cleanedSecret = newSecretEntry.Item2.TrimNullToEmpty();

            if (string.IsNullOrWhiteSpace(cleanedSecret))
            {
                StatusContext.ToastError("AWS Credential Entry Canceled - secret can not be blank");
                return;
            }

            AwsCredentials.SaveAwsSiteCredential(cleanedKey, cleanedSecret);
        }
    }
}