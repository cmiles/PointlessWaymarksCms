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
    internal class FeaturePointDetailContext : IHasChanges, IHasValidationIssues, INotifyPropertyChanged,
        IPointDetailEditor
    {
        private CreatedAndUpdatedByAndOnDisplayContext _createdUpdatedDisplay;
        private PointDetail _dbEntry;
        private Feature _detailData;
        private bool _hasChanges;
        private bool _hasValidationIssues;
        private StringDataEntryContext _noteEditor;
        private ContentFormatChooserContext _noteFormatEditor;
        private StatusControlContext _statusContext;
        private StringDataEntryContext _titleEditor;

        private FeaturePointDetailContext(StatusControlContext statusContext)
        {
            StatusContext = statusContext ?? new StatusControlContext();
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

        public Feature DetailData
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

        public StringDataEntryContext TitleEditor
        {
            get => _titleEditor;
            set
            {
                if (Equals(value, _titleEditor)) return;
                _titleEditor = value;
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

            var detailData = new Feature
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

        public static async Task<FeaturePointDetailContext> CreateInstance(PointDetail detail,
            StatusControlContext statusContext)
        {
            var newControl = new FeaturePointDetailContext(statusContext);
            await newControl.LoadData(detail);
            return newControl;
        }

        public async Task LoadData(PointDetail toLoad)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            DbEntry = toLoad ?? new PointDetail
            {
                CreatedBy = UserSettingsSingleton.CurrentSettings().DefaultCreatedBy,
                DataType = ((dynamic) DetailData).DataTypeIdentifier,
            };

            CreatedUpdatedDisplay = await CreatedAndUpdatedByAndOnDisplayContext.CreateInstance(StatusContext, DbEntry);

            if (!string.IsNullOrWhiteSpace(DbEntry.StructuredDataAsJson))
                DetailData = JsonSerializer.Deserialize<Feature>(DbEntry.StructuredDataAsJson);

            DetailData ??= new Feature {NotesContentFormat = UserSettingsUtilities.DefaultContentFormatChoice()};

            NoteEditor = StringDataEntryContext.CreateInstance();
            NoteEditor.Title = "Notes";
            NoteEditor.HelpText = "Notes";
            NoteEditor.ReferenceValue = DetailData.Notes ?? string.Empty;
            NoteEditor.UserValue = DetailData.Notes.TrimNullToEmpty();

            NoteFormatEditor = ContentFormatChooserContext.CreateInstance(StatusContext);
            NoteFormatEditor.InitialValue = DetailData.NotesContentFormat;

            TitleEditor = StringDataEntryContext.CreateInstance();
            TitleEditor.Title = "Title";
            TitleEditor.HelpText =
                "The title for this feature - this could be something unique or something recorded for many points";
            TitleEditor.ReferenceValue = DetailData.Notes ?? string.Empty;
            TitleEditor.UserValue = DetailData.Notes.TrimNullToEmpty();
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