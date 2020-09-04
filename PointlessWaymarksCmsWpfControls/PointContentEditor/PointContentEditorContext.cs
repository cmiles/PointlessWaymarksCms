﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MvvmHelpers.Commands;
using PointlessWaymarksCmsData;
using PointlessWaymarksCmsData.Content;
using PointlessWaymarksCmsData.Database;
using PointlessWaymarksCmsData.Database.Models;
using PointlessWaymarksCmsData.Spatial;
using PointlessWaymarksCmsData.Spatial.Elevation;
using PointlessWaymarksCmsWpfControls.BodyContentEditor;
using PointlessWaymarksCmsWpfControls.ContentIdViewer;
using PointlessWaymarksCmsWpfControls.ConversionDataEntry;
using PointlessWaymarksCmsWpfControls.CreatedAndUpdatedByAndOnDisplay;
using PointlessWaymarksCmsWpfControls.HelpDisplay;
using PointlessWaymarksCmsWpfControls.PointDetailEditor;
using PointlessWaymarksCmsWpfControls.ShowInMainSiteFeedEditor;
using PointlessWaymarksCmsWpfControls.Status;
using PointlessWaymarksCmsWpfControls.TagsEditor;
using PointlessWaymarksCmsWpfControls.TitleSummarySlugFolderEditor;
using PointlessWaymarksCmsWpfControls.UpdateNotesEditor;
using PointlessWaymarksCmsWpfControls.Utility;

namespace PointlessWaymarksCmsWpfControls.PointContentEditor
{
    public class PointContentEditorContext : INotifyPropertyChanged, IHasChanges
    {
        private BodyContentEditorContext _bodyContent;
        private bool _broadcastLatLongChange = true;
        private ContentIdViewerControlContext _contentId;
        private CreatedAndUpdatedByAndOnDisplayContext _createdUpdatedDisplay;
        private PointContent _dbEntry;
        private ConversionDataEntryContext<double?> _elevationEntry;
        private Command _extractNewLinksCommand;
        private Command _getElevationCommand;
        private HelpDisplayContext _helpContext;
        private ConversionDataEntryContext<double> _latitudeEntry;
        private ConversionDataEntryContext<double> _longitudeEntry;
        private PointDetailListContext _pointDetails;
        private Command _saveAndGenerateHtmlCommand;
        private ShowInMainSiteFeedEditorContext _showInSiteFeed;
        private TagsEditorContext _tagEdit;
        private TitleSummarySlugEditorContext _titleSummarySlugFolder;
        private UpdateNotesEditorContext _updateNotes;
        private Command _viewOnSiteCommand;

        public PointContentEditorContext(StatusControlContext statusContext, PointContent pointContent)
        {
            StatusContext = statusContext ?? new StatusControlContext();

            HelpContext =
                new HelpDisplayContext(CommonFields.TitleSlugFolderSummary + BracketCodeHelpMarkdown.HelpBlock);

            SaveAndGenerateHtmlCommand = StatusContext.RunBlockingTaskCommand(SaveAndGenerateHtml);
            ViewOnSiteCommand = StatusContext.RunBlockingTaskCommand(ViewOnSite);
            ExtractNewLinksCommand = StatusContext.RunBlockingTaskCommand(() =>
                LinkExtraction.ExtractNewAndShowLinkContentEditors(
                    $"{BodyContent.BodyContent} {UpdateNotes.UpdateNotes}", StatusContext.ProgressTracker()));
            GetElevationCommand = StatusContext.RunBlockingTaskCommand(GetElevation);

            StatusContext.RunFireAndForgetTaskWithUiToastErrorReturn(async () => await LoadData(pointContent));
        }

        public BodyContentEditorContext BodyContent
        {
            get => _bodyContent;
            set
            {
                if (Equals(value, _bodyContent)) return;
                _bodyContent = value;
                OnPropertyChanged();
            }
        }

        public ContentIdViewerControlContext ContentId
        {
            get => _contentId;
            set
            {
                if (Equals(value, _contentId)) return;
                _contentId = value;
                OnPropertyChanged();
            }
        }

        public CreatedAndUpdatedByAndOnDisplayContext CreatedUpdatedDisplay
        {
            get => _createdUpdatedDisplay;
            set
            {
                if (Equals(value, _createdUpdatedDisplay)) return;
                _createdUpdatedDisplay = value;
                OnPropertyChanged();
            }
        }

        public PointContent DbEntry
        {
            get => _dbEntry;
            set
            {
                if (Equals(value, _dbEntry)) return;
                _dbEntry = value;
                OnPropertyChanged();
            }
        }

        public ConversionDataEntryContext<double?> ElevationEntry
        {
            get => _elevationEntry;
            set
            {
                if (Equals(value, _elevationEntry)) return;
                _elevationEntry = value;
                OnPropertyChanged();
            }
        }

        public Command ExtractNewLinksCommand
        {
            get => _extractNewLinksCommand;
            set
            {
                if (Equals(value, _extractNewLinksCommand)) return;
                _extractNewLinksCommand = value;
                OnPropertyChanged();
            }
        }

        public Command GetElevationCommand
        {
            get => _getElevationCommand;
            set
            {
                if (Equals(value, _getElevationCommand)) return;
                _getElevationCommand = value;
                OnPropertyChanged();
            }
        }

        public bool HasChanges =>
            TitleSummarySlugFolder.HasChanges || CreatedUpdatedDisplay.HasChanges ||
            ShowInSiteFeed.ShowInMainSiteFeedHasChanges || BodyContent.HasChanges || UpdateNotes.HasChanges ||
            TagEdit.TagsHaveChanges || LatitudeEntry.HasChanges || LongitudeEntry.HasChanges ||
            ElevationEntry.HasChanges;

        public HelpDisplayContext HelpContext
        {
            get => _helpContext;
            set
            {
                if (Equals(value, _helpContext)) return;
                _helpContext = value;
                OnPropertyChanged();
            }
        }

        public ConversionDataEntryContext<double> LatitudeEntry
        {
            get => _latitudeEntry;
            set
            {
                if (Equals(value, _latitudeEntry)) return;
                _latitudeEntry = value;
                OnPropertyChanged();
            }
        }

        public ConversionDataEntryContext<double> LongitudeEntry
        {
            get => _longitudeEntry;
            set
            {
                if (Equals(value, _longitudeEntry)) return;
                _longitudeEntry = value;
                OnPropertyChanged();
            }
        }

        public PointDetailListContext PointDetails
        {
            get => _pointDetails;
            set
            {
                if (Equals(value, _pointDetails)) return;
                _pointDetails = value;
                OnPropertyChanged();
            }
        }

        public Command SaveAndGenerateHtmlCommand
        {
            get => _saveAndGenerateHtmlCommand;
            set
            {
                if (Equals(value, _saveAndGenerateHtmlCommand)) return;
                _saveAndGenerateHtmlCommand = value;
                OnPropertyChanged();
            }
        }

        public ShowInMainSiteFeedEditorContext ShowInSiteFeed
        {
            get => _showInSiteFeed;
            set
            {
                if (value == _showInSiteFeed) return;
                _showInSiteFeed = value;
                OnPropertyChanged();
            }
        }

        public StatusControlContext StatusContext { get; set; }

        public TagsEditorContext TagEdit
        {
            get => _tagEdit;
            set
            {
                if (Equals(value, _tagEdit)) return;
                _tagEdit = value;
                OnPropertyChanged();
            }
        }

        public TitleSummarySlugEditorContext TitleSummarySlugFolder
        {
            get => _titleSummarySlugFolder;
            set
            {
                if (Equals(value, _titleSummarySlugFolder)) return;
                _titleSummarySlugFolder = value;
                OnPropertyChanged();
            }
        }

        public UpdateNotesEditorContext UpdateNotes
        {
            get => _updateNotes;
            set
            {
                if (Equals(value, _updateNotes)) return;
                _updateNotes = value;
                OnPropertyChanged();
            }
        }

        public Command ViewOnSiteCommand
        {
            get => _viewOnSiteCommand;
            set
            {
                if (Equals(value, _viewOnSiteCommand)) return;
                _viewOnSiteCommand = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void CheckForChangesAndValidate(bool checkFromLatitudeLongitudePropertyChanges)
        {
            SpatialHelpers.RoundLatLongElevation(this);

            if (_broadcastLatLongChange && checkFromLatitudeLongitudePropertyChanges &&
                !LatitudeEntry.HasValidationIssues && !LongitudeEntry.HasValidationIssues)
                RaisePointLatitudeLongitudeChange?.Invoke(this,
                    new PointLatitudeLongitudeChange(LatitudeEntry.UserValue, LongitudeEntry.UserValue));
        }

        private PointContent CurrentStateToPointContent()
        {
            var newEntry = new PointContent();

            if (DbEntry == null || DbEntry.Id < 1)
            {
                newEntry.ContentId = Guid.NewGuid();
                newEntry.CreatedOn = DateTime.Now;
            }
            else
            {
                newEntry.ContentId = DbEntry.ContentId;
                newEntry.CreatedOn = DbEntry.CreatedOn;
                newEntry.LastUpdatedOn = DateTime.Now;
                newEntry.LastUpdatedBy = CreatedUpdatedDisplay.UpdatedByEntry.UserValue.TrimNullToEmpty();
            }

            newEntry.Folder = TitleSummarySlugFolder.Folder.TrimNullToEmpty();
            newEntry.Slug = TitleSummarySlugFolder.SlugEntry.UserValue.TrimNullToEmpty();
            newEntry.Summary = TitleSummarySlugFolder.SummaryEntry.UserValue.TrimNullToEmpty();
            newEntry.ShowInMainSiteFeed = ShowInSiteFeed.ShowInMainSiteFeed;
            newEntry.Tags = TagEdit.TagListString();
            newEntry.Title = TitleSummarySlugFolder.TitleEntry.UserValue.TrimNullToEmpty();
            newEntry.CreatedBy = CreatedUpdatedDisplay.CreatedByEntry.UserValue.TrimNullToEmpty();
            newEntry.UpdateNotes = UpdateNotes.UpdateNotes.TrimNullToEmpty();
            newEntry.UpdateNotesFormat = UpdateNotes.UpdateNotesFormat.SelectedContentFormatAsString;
            newEntry.BodyContent = BodyContent.BodyContent.TrimNullToEmpty();
            newEntry.BodyContentFormat = BodyContent.BodyContentFormat.SelectedContentFormatAsString;

            newEntry.Latitude = LatitudeEntry.UserValue;
            newEntry.Longitude = LongitudeEntry.UserValue;
            newEntry.Elevation = ElevationEntry.UserValue;

            return newEntry;
        }


        public async Task GetElevation()
        {
            if (LatitudeEntry.HasValidationIssues || LongitudeEntry.HasValidationIssues)
            {
                StatusContext.ToastError("Lat Long is not valid");
                return;
            }

            var httpClient = new HttpClient();

            try
            {
                var elevationResult = await ElevationService.OpenTopoNedElevation(httpClient, LatitudeEntry.UserValue,
                    LongitudeEntry.UserValue, StatusContext.ProgressTracker());

                if (elevationResult != null)
                {
                    ElevationEntry.UserValue = elevationResult.MetersToFeet();

                    StatusContext.ToastSuccess(
                        $"Set elevation of {ElevationEntry.UserValue} from Open Topo Data - www.opentopodata.org - NED data set");

                    return;
                }
            }
            catch (Exception e)
            {
                await EventLogContext.TryWriteExceptionToLog(e, StatusContext.StatusControlContextId.ToString(),
                    $"Open Topo Data NED Request for {LatitudeEntry.UserValue}, {LongitudeEntry.UserValue}");
            }

            try
            {
                var elevationResult = await ElevationService.OpenTopoMapZenElevation(httpClient,
                    LatitudeEntry.UserValue, LongitudeEntry.UserValue, StatusContext.ProgressTracker());

                if (elevationResult == null)
                {
                    await EventLogContext.TryWriteDiagnosticMessageToLog(
                        "Unexpected Null return from an Open Topo Data Mapzen Request to {LatitudeEntry.UserValue}, {LongitudeEntry.UserValue}",
                        StatusContext.StatusControlContextId.ToString());
                    StatusContext.ToastError("Elevation Exception - unexpected Null return...");
                    return;
                }

                ElevationEntry.UserValue = elevationResult.MetersToFeet();

                StatusContext.ToastSuccess(
                    $"Set elevation of {ElevationEntry.UserValue} from Open Topo Data - www.opentopodata.org - Mapzen data set");
            }
            catch (Exception e)
            {
                await EventLogContext.TryWriteExceptionToLog(e, StatusContext.StatusControlContextId.ToString(),
                    $"Open Topo Data Mapzen Request for {LatitudeEntry.UserValue}, {LongitudeEntry.UserValue}");
                StatusContext.ToastError($"Elevation Exception - {e.Message}");
            }
        }

        public async Task LoadData(PointContent toLoad)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            DbEntry = toLoad ?? new PointContent
            {
                BodyContentFormat = UserSettingsUtilities.DefaultContentFormatChoice(),
                UpdateNotesFormat = UserSettingsUtilities.DefaultContentFormatChoice(),
                CreatedBy = UserSettingsSingleton.CurrentSettings().DefaultCreatedBy,
                ShowInMainSiteFeed = true,
                Latitude = UserSettingsSingleton.CurrentSettings().LatitudeDefault,
                Longitude = UserSettingsSingleton.CurrentSettings().LongitudeDefault
            };

            TitleSummarySlugFolder = new TitleSummarySlugEditorContext(StatusContext, DbEntry);
            CreatedUpdatedDisplay = new CreatedAndUpdatedByAndOnDisplayContext(StatusContext, DbEntry);
            ShowInSiteFeed = new ShowInMainSiteFeedEditorContext(StatusContext, DbEntry, true);
            ContentId = new ContentIdViewerControlContext(StatusContext, DbEntry);
            UpdateNotes = new UpdateNotesEditorContext(StatusContext, DbEntry);
            TagEdit = new TagsEditorContext(StatusContext, DbEntry);
            BodyContent = new BodyContentEditorContext(StatusContext, DbEntry);

            ElevationEntry = new ConversionDataEntryContext<double?>
            {
                Title = "Elevation",
                HelpText = "Elevation in Feet",
                ReferenceValue = DbEntry.Elevation,
                UserValue = DbEntry.Elevation,
                Converter = ConversionDataEntryHelpers.DoubleNullableConversion,
                ValidationFunctions = new List<Func<double?, (bool passed, string validationMessage)>>
                {
                    CommonContentValidation.ElevationValidation
                }
            };

            LatitudeEntry = new ConversionDataEntryContext<double>
            {
                Title = "Latitude",
                HelpText = "In DDD.DDDDDD°",
                ReferenceValue = DbEntry.Latitude,
                UserValue = DbEntry.Latitude,
                Converter = ConversionDataEntryHelpers.DoubleConversion,
                ValidationFunctions =
                    new List<Func<double, (bool passed, string validationMessage)>>
                    {
                        CommonContentValidation.LatitudeValidation
                    },
                ComparisonFunction = (o, u) => o.IsApproximatelyEqualTo(u, .000001)
            };

            LongitudeEntry = new ConversionDataEntryContext<double>
            {
                Title = "Longitude",
                HelpText = "In DDD.DDDDDD°",
                ReferenceValue = DbEntry.Longitude,
                UserValue = DbEntry.Longitude,
                Converter = ConversionDataEntryHelpers.DoubleConversion,
                ValidationFunctions =
                    new List<Func<double, (bool passed, string validationMessage)>>
                    {
                        CommonContentValidation.LongitudeValidation
                    },
                ComparisonFunction = (o, u) => o.IsApproximatelyEqualTo(u, .000001)
            };

            PointDetails = new PointDetailListContext(StatusContext, DbEntry);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (string.IsNullOrWhiteSpace(propertyName)) return;

            if (!propertyName.Contains("HasChanges") && !propertyName.Contains("Validation"))
                CheckForChangesAndValidate(propertyName == "Latitude" || propertyName == "Longitude");
        }

        public void OnRaisePointLatitudeLongitudeChange(object sender, PointLatitudeLongitudeChange e)
        {
            _broadcastLatLongChange = false;

            LatitudeEntry.UserValue = e.Latitude;
            LongitudeEntry.UserValue = e.Longitude;

            _broadcastLatLongChange = true;
        }

        public event EventHandler<PointLatitudeLongitudeChange> RaisePointLatitudeLongitudeChange;

        public async Task SaveAndGenerateHtml()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var (generationReturn, newContent) = await PointGenerator.SaveAndGenerateHtml(CurrentStateToPointContent(),
                null, StatusContext.ProgressTracker());

            if (generationReturn.HasError || newContent == null)
            {
                await StatusContext.ShowMessageWithOkButton("Problem Saving and Generating Html",
                    generationReturn.GenerationNote);
                return;
            }

            await LoadData(newContent);
        }


        private async Task ViewOnSite()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (DbEntry == null || DbEntry.Id < 1)
            {
                StatusContext.ToastError("Please save the content first...");
                return;
            }

            var settings = UserSettingsSingleton.CurrentSettings();

            var url = $@"http://{settings.PointPageUrl(DbEntry)}";

            var ps = new ProcessStartInfo(url) {UseShellExecute = true, Verb = "open"};
            Process.Start(ps);
        }
    }
}