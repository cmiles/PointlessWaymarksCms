﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PointlessWaymarksCmsData;
using PointlessWaymarksCmsData.Database;
using PointlessWaymarksCmsData.Database.Models;
using PointlessWaymarksCmsData.Database.PointDetailModels;
using PointlessWaymarksCmsWpfControls.ContentFormat;
using PointlessWaymarksCmsWpfControls.CreatedAndUpdatedByAndOnDisplay;
using PointlessWaymarksCmsWpfControls.Status;
using PointlessWaymarksCmsWpfControls.StringDataEntry;
using PointlessWaymarksCmsWpfControls.Utility;

namespace PointlessWaymarksCmsWpfControls.PointDetailEditor
{
    public class RestRoomPointDetailContext : IHasChanges, IHasValidationIssues, INotifyPropertyChanged,
        IPointDetailEditor
    {
        private CreatedAndUpdatedByAndOnDisplayContext _createdUpdatedDisplay;
        private PointDetail _dbEntry;
        private RestRoom _detailData;
        private bool _hasChanges;
        private bool _hasValidationIssues;
        private StringDataEntryContext _noteEditor;
        private ContentFormatChooserContext _noteFormatEditor;
        private StatusControlContext _statusContext;

        public RestRoomPointDetailContext(PointDetail detail, StatusControlContext statusContext)
        {
            StatusContext = statusContext ?? new StatusControlContext();
            StatusContext.RunFireAndForgetTaskWithUiToastErrorReturn(async () => await LoadData(detail));
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

        public PointDetail DbEntry
        {
            get => _dbEntry;
            set
            {
                if (Equals(value, _dbEntry)) return;
                _dbEntry = value;
                OnPropertyChanged();
            }
        }

        public RestRoom DetailData
        {
            get => _detailData;
            set
            {
                if (Equals(value, _detailData)) return;
                _detailData = value;
                OnPropertyChanged();
            }
        }

        public bool HasChanges
        {
            get => _hasChanges;
            set
            {
                if (value == _hasChanges) return;
                _hasChanges = value;
                OnPropertyChanged();
            }
        }

        public bool HasValidationIssues
        {
            get => _hasValidationIssues;
            set
            {
                if (value == _hasValidationIssues) return;
                _hasValidationIssues = value;
                OnPropertyChanged();
            }
        }

        public StringDataEntryContext NoteEditor
        {
            get => _noteEditor;
            set
            {
                if (Equals(value, _noteEditor)) return;
                _noteEditor = value;
                OnPropertyChanged();
            }
        }

        public ContentFormatChooserContext NoteFormatEditor
        {
            get => _noteFormatEditor;
            set
            {
                if (Equals(value, _noteFormatEditor)) return;
                _noteFormatEditor = value;
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

        public PointDetail CurrentPointDetail()
        {
            var newEntry = new PointDetail();

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

            var detailData = new RestRoom
            {
                Notes = NoteEditor.UserValue.TrimNullToEmpty(),
                NotesContentFormat = NoteFormatEditor.SelectedContentFormatAsString
            };

            Db.DefaultPropertyCleanup(detailData);

            newEntry.StructuredDataAsJson = JsonSerializer.Serialize(detailData);

            return newEntry;
        }

        private void CheckForChangesAndValidate()
        {
            HasChanges = PropertyScanners.ChildPropertiesHaveChanges(this);
            HasValidationIssues = PropertyScanners.ChildPropertiesHaveValidationIssues(this);
        }

        public async Task LoadData(PointDetail toLoad)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            DbEntry = toLoad ?? new PointDetail
            {
                CreatedBy = UserSettingsSingleton.CurrentSettings().DefaultCreatedBy,
                DataType = DetailData.DataTypeIdentifier
            };

            CreatedUpdatedDisplay = new CreatedAndUpdatedByAndOnDisplayContext(StatusContext, DbEntry);

            if (!string.IsNullOrWhiteSpace(DbEntry.StructuredDataAsJson))
                DetailData = JsonSerializer.Deserialize<RestRoom>(DbEntry.StructuredDataAsJson);

            DetailData = new RestRoom {NotesContentFormat = UserSettingsUtilities.DefaultContentFormatChoice()};

            NoteEditor = new StringDataEntryContext
            {
                Title = "Notes",
                HelpText = "Notes",
                ReferenceValue = DetailData.Notes ?? string.Empty,
                UserValue = DetailData.Notes.TrimNullToEmpty()
            };

            NoteFormatEditor =
                new ContentFormatChooserContext(StatusContext) {InitialValue = DetailData.NotesContentFormat};
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (string.IsNullOrWhiteSpace(propertyName)) return;

            if (!propertyName.Contains("HasChanges") && !propertyName.Contains("Validation"))
                CheckForChangesAndValidate();
        }
    }
}