﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using MvvmHelpers.Commands;
using PointlessWaymarks.CmsData;
using PointlessWaymarks.CmsData.Database;
using PointlessWaymarks.CmsData.Html.CommonHtml;
using PointlessWaymarks.CmsData.Html.MapComponentData;
using PointlessWaymarks.CmsWpfControls.ContentHistoryView;
using PointlessWaymarks.CmsWpfControls.MapComponentEditor;
using PointlessWaymarks.CmsWpfControls.Utility;
using PointlessWaymarks.WpfCommon.Status;
using PointlessWaymarks.WpfCommon.ThreadSwitcher;
using PointlessWaymarks.WpfCommon.Utility;

namespace PointlessWaymarks.CmsWpfControls.MapComponentList
{
    public class MapComponentListWithActionsContext : INotifyPropertyChanged
    {
        private Command _deleteSelectedCommand;
        private Command _editSelectedContentCommand;
        private Command _extractNewLinksInSelectedCommand;
        private Command _generateSelectedHtmlCommand;
        private Command _importFromExcelCommand;
        private MapComponentListContext _listContext;
        private Command _newContentCommand;
        private Command _postCodesToClipboardForSelectedCommand;
        private Command _refreshDataCommand;
        private Command _selectedToExcelCommand;
        private StatusControlContext _statusContext;
        private Command _viewHistoryCommand;

        public MapComponentListWithActionsContext(StatusControlContext statusContext)
        {
            StatusContext = statusContext ?? new StatusControlContext();

            StatusContext.RunFireAndForgetBlockingTaskWithUiMessageReturn(LoadData);
        }

        public Command DeleteSelectedCommand
        {
            get => _deleteSelectedCommand;
            set
            {
                if (Equals(value, _deleteSelectedCommand)) return;
                _deleteSelectedCommand = value;
                OnPropertyChanged();
            }
        }

        public Command EditSelectedContentCommand
        {
            get => _editSelectedContentCommand;
            set
            {
                if (Equals(value, _editSelectedContentCommand)) return;
                _editSelectedContentCommand = value;
                OnPropertyChanged();
            }
        }

        public Command ExtractNewLinksInSelectedCommand
        {
            get => _extractNewLinksInSelectedCommand;
            set
            {
                if (Equals(value, _extractNewLinksInSelectedCommand)) return;
                _extractNewLinksInSelectedCommand = value;
                OnPropertyChanged();
            }
        }

        public Command GenerateSelectedHtmlCommand
        {
            get => _generateSelectedHtmlCommand;
            set
            {
                if (Equals(value, _generateSelectedHtmlCommand)) return;
                _generateSelectedHtmlCommand = value;
                OnPropertyChanged();
            }
        }

        public Command ImportFromExcelCommand
        {
            get => _importFromExcelCommand;
            set
            {
                if (Equals(value, _importFromExcelCommand)) return;
                _importFromExcelCommand = value;
                OnPropertyChanged();
            }
        }

        public MapComponentListContext ListContext
        {
            get => _listContext;
            set
            {
                if (Equals(value, _listContext)) return;
                _listContext = value;
                OnPropertyChanged();
            }
        }

        public Command MapComponentCodesToClipboardForSelectedCommand
        {
            get => _postCodesToClipboardForSelectedCommand;
            set
            {
                if (Equals(value, _postCodesToClipboardForSelectedCommand)) return;
                _postCodesToClipboardForSelectedCommand = value;
                OnPropertyChanged();
            }
        }

        public Command NewContentCommand
        {
            get => _newContentCommand;
            set
            {
                if (Equals(value, _newContentCommand)) return;
                _newContentCommand = value;
                OnPropertyChanged();
            }
        }

        public Command RefreshDataCommand
        {
            get => _refreshDataCommand;
            set
            {
                if (Equals(value, _refreshDataCommand)) return;
                _refreshDataCommand = value;
                OnPropertyChanged();
            }
        }

        public Command SelectedToExcelCommand
        {
            get => _selectedToExcelCommand;
            set
            {
                if (Equals(value, _selectedToExcelCommand)) return;
                _selectedToExcelCommand = value;
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

        public Command ViewHistoryCommand
        {
            get => _viewHistoryCommand;
            set
            {
                if (Equals(value, _viewHistoryCommand)) return;
                _viewHistoryCommand = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task BracketCodesToClipboardForSelected()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (ListContext.SelectedItems == null || !ListContext.SelectedItems.Any())
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            var finalString = ListContext.SelectedItems.Aggregate(string.Empty,
                (current, loopSelected) =>
                    current + @$"{BracketCodeMapComponents.Create(loopSelected.DbEntry)}{Environment.NewLine}");

            await ThreadSwitcher.ResumeForegroundAsync();

            Clipboard.SetText(finalString);

            StatusContext.ToastSuccess($"To Clipboard {finalString}");
        }

        private async Task Delete()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var selected = ListContext?.SelectedItems?.OrderBy(x => x.DbEntry.Title).ToList();

            if (selected == null || !selected.Any())
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            if (selected.Count > 1)
                if (await StatusContext.ShowMessage("Delete Multiple Items",
                    $"You are about to delete {selected.Count} items - do you really want to delete all of these items?" +
                    $"{Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine, selected.Select(x => x.DbEntry.Title))}",
                    new List<string> {"Yes", "No"}) == "No")
                    return;

            var selectedItems = selected.ToList();

            foreach (var loopSelected in selectedItems)
            {
                if (loopSelected.DbEntry == null || loopSelected.DbEntry.Id < 1)
                {
                    StatusContext.ToastError("Entry is not saved - Skipping?");
                    return;
                }

                await Db.DeleteMapComponent(loopSelected.DbEntry.ContentId, StatusContext.ProgressTracker());

                //Todo: Delete The Related Local Files
            }
        }

        private async Task EditSelectedContent()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (ListContext.SelectedItems == null || !ListContext.SelectedItems.Any())
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            var context = await Db.Context();
            var frozenList = ListContext.SelectedItems;

            foreach (var loopSelected in frozenList)
            {
                var refreshedData =
                    context.MapComponents.SingleOrDefault(x => x.ContentId == loopSelected.DbEntry.ContentId);

                if (refreshedData == null)
                {
                    StatusContext.ToastError(
                        $"{loopSelected.DbEntry.Title} is no longer active in the database? Can not edit - " +
                        "look for a historic version...");
                    continue;
                }

                await ThreadSwitcher.ResumeForegroundAsync();

                var newContentWindow = new MapComponentEditorWindow(refreshedData);

                newContentWindow.PositionWindowAndShow();

                await ThreadSwitcher.ResumeBackgroundAsync();
            }
        }

        private async Task ExtractNewLinksInSelected()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (ListContext.SelectedItems == null || !ListContext.SelectedItems.Any())
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            var context = await Db.Context();
            var frozenList = ListContext.SelectedItems;

            foreach (var loopSelected in frozenList)
            {
                var refreshedData =
                    context.MapComponents.SingleOrDefault(x => x.ContentId == loopSelected.DbEntry.ContentId);

                if (refreshedData == null) continue;

                await LinkExtraction.ExtractNewAndShowLinkContentEditors($"{refreshedData.UpdateNotes}",
                    StatusContext.ProgressTracker());
            }
        }

        private async Task GenerateSelectedHtml()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (ListContext.SelectedItems == null || !ListContext.SelectedItems.Any())
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            var loopCount = 1;
            var totalCount = ListContext.SelectedItems.Count;

            foreach (var loopSelected in ListContext.SelectedItems)
            {
                StatusContext.Progress(
                    $"Generating Html for {loopSelected.DbEntry.Title}, {loopCount} of {totalCount}");

                await MapData.WriteJsonData(loopSelected.DbEntry.ContentId);

                StatusContext.ToastSuccess("Generated Map Data");

                loopCount++;
            }
        }

        private async Task LoadData()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            ListContext = new MapComponentListContext(StatusContext);

            GenerateSelectedHtmlCommand = StatusContext.RunBlockingTaskCommand(GenerateSelectedHtml);
            EditSelectedContentCommand = StatusContext.RunBlockingTaskCommand(EditSelectedContent);
            MapComponentCodesToClipboardForSelectedCommand =
                StatusContext.RunBlockingTaskCommand(BracketCodesToClipboardForSelected);
            NewContentCommand = StatusContext.RunBlockingTaskCommand(NewContent);
            RefreshDataCommand = StatusContext.RunBlockingTaskCommand(ListContext.LoadData);
            DeleteSelectedCommand = StatusContext.RunBlockingTaskCommand(Delete);
            ExtractNewLinksInSelectedCommand = StatusContext.RunBlockingTaskCommand(ExtractNewLinksInSelected);
            ViewHistoryCommand = StatusContext.RunNonBlockingTaskCommand(ViewHistory);

            ImportFromExcelCommand =
                StatusContext.RunBlockingTaskCommand(async () => await ExcelHelpers.ImportFromExcel(StatusContext));
            SelectedToExcelCommand = StatusContext.RunNonBlockingTaskCommand(async () =>
                await ExcelHelpers.SelectedToExcel(ListContext.SelectedItems?.Cast<dynamic>().ToList(), StatusContext));
        }

        private async Task NewContent()
        {
            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new MapComponentEditorWindow(null);

            newContentWindow.PositionWindowAndShow();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task ViewHistory()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            var selected = ListContext.SelectedItems;

            if (selected == null || !selected.Any())
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            if (selected.Count > 1)
            {
                StatusContext.ToastError("Please Select a Single Item");
                return;
            }

            var singleSelected = selected.Single();

            if (singleSelected.DbEntry == null || singleSelected.DbEntry.ContentId == Guid.Empty)
            {
                StatusContext.ToastWarning("No History - New/Unsaved Entry?");
                return;
            }

            var db = await Db.Context();

            StatusContext.Progress($"Looking up Historic Entries for {singleSelected.DbEntry.Title}");

            var historicItems = await db.HistoricMapComponents
                .Where(x => x.ContentId == singleSelected.DbEntry.ContentId).ToListAsync();

            StatusContext.Progress($"Found {historicItems.Count} Historic Entries");

            if (historicItems.Count < 1)
            {
                StatusContext.ToastWarning("No History to Show...");
                return;
            }

            var historicView = new ContentViewHistoryPage($"Historic Entries - {singleSelected.DbEntry.Title}",
                UserSettingsSingleton.CurrentSettings().SiteName, $"Historic Entries - {singleSelected.DbEntry.Title}",
                historicItems.OrderByDescending(x => x.LastUpdatedOn.HasValue).ThenByDescending(x => x.LastUpdatedOn)
                    .Select(ObjectDumper.Dump).ToList());

            historicView.WriteHtmlToTempFolderAndShow(StatusContext.ProgressTracker());
        }
    }
}