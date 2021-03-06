﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using PointlessWaymarks.CmsData;
using PointlessWaymarks.CmsData.CommonHtml;
using PointlessWaymarks.CmsData.ContentHtml.FileHtml;
using PointlessWaymarks.CmsData.Database;
using PointlessWaymarks.CmsData.Database.Models;
using PointlessWaymarks.CmsWpfControls.ContentHistoryView;
using PointlessWaymarks.CmsWpfControls.ContentList;
using PointlessWaymarks.CmsWpfControls.FileContentEditor;
using PointlessWaymarks.CmsWpfControls.Utility;
using PointlessWaymarks.WpfCommon.Commands;
using PointlessWaymarks.WpfCommon.Status;
using PointlessWaymarks.WpfCommon.ThreadSwitcher;
using PointlessWaymarks.WpfCommon.Utility;

namespace PointlessWaymarks.CmsWpfControls.FileList
{
    public class FileContentActions : IContentActions<FileContent>
    {
        private Command<FileContent> _deleteCommand;
        private Command<FileContent> _editCommand;
        private Command<FileContent> _extractNewLinksCommand;
        private Command<FileContent> _generateHtmlCommand;
        private Command<FileContent> _linkCodeToClipboardCommand;
        private Command<FileContent> _openUrlCommand;
        private StatusControlContext _statusContext;
        private Command<FileContent> _viewFileCommand;
        private Command<FileContent> _viewHistoryCommand;

        public FileContentActions(StatusControlContext statusContext)
        {
            StatusContext = statusContext;
            DeleteCommand = StatusContext.RunBlockingTaskCommand<FileContent>(Delete);
            EditCommand = StatusContext.RunNonBlockingTaskCommand<FileContent>(Edit);
            ExtractNewLinksCommand = StatusContext.RunBlockingTaskCommand<FileContent>(ExtractNewLinks);
            GenerateHtmlCommand = StatusContext.RunBlockingTaskCommand<FileContent>(GenerateHtml);
            LinkCodeToClipboardCommand =
                StatusContext.RunBlockingTaskCommand<FileContent>(DefaultBracketCodeToClipboard);
            OpenUrlCommand = StatusContext.RunBlockingTaskCommand<FileContent>(OpenUrl);
            ViewFileCommand = StatusContext.RunNonBlockingTaskCommand<FileContent>(ViewFile);
            ViewHistoryCommand = StatusContext.RunNonBlockingTaskCommand<FileContent>(ViewHistory);
        }

        public Command<FileContent> ViewFileCommand
        {
            get => _viewFileCommand;
            set
            {
                if (Equals(value, _viewFileCommand)) return;
                _viewFileCommand = value;
                OnPropertyChanged();
            }
        }

        public string DefaultBracketCode(FileContent content)
        {
            if (content?.ContentId == null) return string.Empty;
            return content.MainPicture != null
                ? @$"{BracketCodeFileImage.Create(content)}"
                : @$"{BracketCodeFiles.Create(content)}";
        }

        public async Task DefaultBracketCodeToClipboard(FileContent content)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (content == null)
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            var finalString = content.MainPicture != null
                ? @$"{BracketCodeFileImage.Create(content)}{Environment.NewLine}"
                : @$"{BracketCodeFiles.Create(content)}{Environment.NewLine}";

            await ThreadSwitcher.ResumeForegroundAsync();

            Clipboard.SetText(finalString);

            StatusContext.ToastSuccess($"To Clipboard {finalString}");
        }

        public async Task Delete(FileContent content)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (content == null)
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            if (content.Id < 1)
            {
                StatusContext.ToastError($"File {content.Title} - Entry is not saved - Skipping?");
                return;
            }

            var settings = UserSettingsSingleton.CurrentSettings();

            await Db.DeleteFileContent(content.ContentId, StatusContext.ProgressTracker());

            var possibleContentDirectory = settings.LocalSiteFileContentDirectory(content, false);
            if (possibleContentDirectory.Exists)
            {
                StatusContext.Progress($"Deleting Generated Folder {possibleContentDirectory.FullName}");
                possibleContentDirectory.Delete(true);
            }
        }

        public Command<FileContent> DeleteCommand
        {
            get => _deleteCommand;
            set
            {
                if (Equals(value, _deleteCommand)) return;
                _deleteCommand = value;
                OnPropertyChanged();
            }
        }

        public async Task Edit(FileContent content)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (content == null) return;

            var context = await Db.Context();

            var refreshedData = context.FileContents.SingleOrDefault(x => x.ContentId == content.ContentId);

            if (refreshedData == null)
                StatusContext.ToastError(
                    $"{content.Title} is no longer active in the database? Can not edit - look for a historic version...");

            await ThreadSwitcher.ResumeForegroundAsync();

            var newContentWindow = new FileContentEditorWindow(refreshedData);

            newContentWindow.PositionWindowAndShow();

            await ThreadSwitcher.ResumeBackgroundAsync();
        }

        public Command<FileContent> EditCommand
        {
            get => _editCommand;
            set
            {
                if (Equals(value, _editCommand)) return;
                _editCommand = value;
                OnPropertyChanged();
            }
        }

        public async Task ExtractNewLinks(FileContent content)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (content == null)
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            var context = await Db.Context();
            var refreshedData = context.FileContents.SingleOrDefault(x => x.ContentId == content.ContentId);

            if (refreshedData == null) return;

            await LinkExtraction.ExtractNewAndShowLinkContentEditors(
                $"{refreshedData.BodyContent} {refreshedData.UpdateNotes}", StatusContext.ProgressTracker());
        }

        public Command<FileContent> ExtractNewLinksCommand
        {
            get => _extractNewLinksCommand;
            set
            {
                if (Equals(value, _extractNewLinksCommand)) return;
                _extractNewLinksCommand = value;
                OnPropertyChanged();
            }
        }


        public async Task GenerateHtml(FileContent content)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (content == null)
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            StatusContext.Progress($"Generating Html for {content.Title}");

            var htmlContext = new SingleFilePage(content);

            await htmlContext.WriteLocalHtml();

            StatusContext.ToastSuccess($"Generated {htmlContext.PageUrl}");
        }

        public Command<FileContent> GenerateHtmlCommand
        {
            get => _generateHtmlCommand;
            set
            {
                if (Equals(value, _generateHtmlCommand)) return;
                _generateHtmlCommand = value;
                OnPropertyChanged();
            }
        }

        public Command<FileContent> LinkCodeToClipboardCommand
        {
            get => _linkCodeToClipboardCommand;
            set
            {
                if (Equals(value, _linkCodeToClipboardCommand)) return;
                _linkCodeToClipboardCommand = value;
                OnPropertyChanged();
            }
        }

        public async Task OpenUrl(FileContent content)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (content == null)
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            var settings = UserSettingsSingleton.CurrentSettings();

            var url = $@"http://{settings.FilePageUrl(content)}";

            var ps = new ProcessStartInfo(url) {UseShellExecute = true, Verb = "open"};
            Process.Start(ps);
        }

        public Command<FileContent> OpenUrlCommand
        {
            get => _openUrlCommand;
            set
            {
                if (Equals(value, _openUrlCommand)) return;
                _openUrlCommand = value;
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

        public async Task ViewHistory(FileContent content)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (content == null)
            {
                StatusContext.ToastError("Nothing Selected?");
                return;
            }

            var db = await Db.Context();

            StatusContext.Progress($"Looking up Historic Entries for {content.Title}");

            var historicItems = await db.HistoricFileContents
                .Where(x => x.ContentId == content.ContentId).ToListAsync();

            StatusContext.Progress($"Found {historicItems.Count} Historic Entries");

            if (historicItems.Count < 1)
            {
                StatusContext.ToastWarning("No History to Show...");
                return;
            }

            var historicView = new ContentViewHistoryPage($"Historic Entries - {content.Title}",
                UserSettingsSingleton.CurrentSettings().SiteName, $"Historic Entries - {content.Title}",
                historicItems.OrderByDescending(x => x.LastUpdatedOn.HasValue).ThenByDescending(x => x.LastUpdatedOn)
                    .Select(ObjectDumper.Dump).ToList());

            historicView.WriteHtmlToTempFolderAndShow(StatusContext.ProgressTracker());
        }

        public Command<FileContent> ViewHistoryCommand
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

        public static FileListListItem ListItemFromDbItem(FileContent content, FileContentActions itemActions,
            bool showType)
        {
            return new()
            {
                DbEntry = content,
                SmallImageUrl = ContentListContext.GetSmallImageUrl(content),
                ItemActions = itemActions,
                ShowType = showType
            };
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public async Task ViewFile(FileContent listItem)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (listItem == null)
            {
                StatusContext.ToastError("Nothing Items to Open?");
                return;
            }

            if (string.IsNullOrWhiteSpace(listItem.OriginalFileName))
            {
                StatusContext.ToastError("No File?");
                return;
            }

            var toOpen = UserSettingsSingleton.CurrentSettings().LocalSiteFileContentFile(listItem);

            if (toOpen is not {Exists: true})
            {
                StatusContext.ToastError("File doesn't exist?");
                return;
            }

            var url = toOpen.FullName;

            var ps = new ProcessStartInfo(url) {UseShellExecute = true, Verb = "open"};
            Process.Start(ps);
        }
    }
}