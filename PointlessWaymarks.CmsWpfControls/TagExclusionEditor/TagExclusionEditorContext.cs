﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Omu.ValueInjecter;
using PointlessWaymarks.CmsData;
using PointlessWaymarks.CmsData.Content;
using PointlessWaymarks.CmsData.Database;
using PointlessWaymarks.CmsData.Database.Models;
using PointlessWaymarks.CmsWpfControls.HelpDisplay;
using PointlessWaymarks.WpfCommon.Commands;
using PointlessWaymarks.WpfCommon.Status;
using PointlessWaymarks.WpfCommon.ThreadSwitcher;
using Serilog;
using TinyIpc.Messaging;

namespace PointlessWaymarks.CmsWpfControls.TagExclusionEditor
{
    public class TagExclusionEditorContext : INotifyPropertyChanged
    {
        private string _helpMarkdown;
        private ObservableCollection<TagExclusionEditorListItem> _items;
        private StatusControlContext _statusContext;

        public TagExclusionEditorContext(StatusControlContext statusContext)
        {
            StatusContext = statusContext ?? new StatusControlContext();

            DataNotificationsProcessor = new DataNotificationsWorkQueue {Processor = DataNotificationReceived};

            HelpMarkdown = TagExclusionHelpMarkdown.HelpBlock;
            AddNewItemCommand = StatusContext.RunBlockingTaskCommand(async () => await AddNewItem());
            SaveItemCommand = StatusContext.RunNonBlockingTaskCommand<TagExclusionEditorListItem>(SaveItem);
            DeleteItemCommand = StatusContext.RunNonBlockingTaskCommand<TagExclusionEditorListItem>(DeleteItem);

            StatusContext.RunFireAndForgetBlockingTaskWithUiMessageReturn(LoadData);
        }

        public Command AddNewItemCommand { get; set; }

        public DataNotificationsWorkQueue DataNotificationsProcessor { get; set; }

        public Command<TagExclusionEditorListItem> DeleteItemCommand { get; set; }

        public string HelpMarkdown
        {
            get => _helpMarkdown;
            set
            {
                if (value == _helpMarkdown) return;
                _helpMarkdown = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TagExclusionEditorListItem> Items
        {
            get => _items;
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                OnPropertyChanged();
            }
        }

        public Command<TagExclusionEditorListItem> SaveItemCommand { get; set; }

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

        public async Task AddNewItem()
        {
            await ThreadSwitcher.ResumeForegroundAsync();

            Items.Add(new TagExclusionEditorListItem {DbEntry = new TagExclusion()});
        }

        private async Task DataNotificationReceived(TinyMessageReceivedEventArgs e)
        {
            var translatedMessage = DataNotifications.TranslateDataNotification(e.Message);

            if (translatedMessage.HasError)
            {
                Log.Error("Data Notification Failure. Error Note {0}. Status Control Context Id {1}",
                    translatedMessage.ErrorNote, StatusContext.StatusControlContextId);
                return;
            }

            await LoadData();
        }

        private async Task DeleteItem(TagExclusionEditorListItem tagItem)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            if (tagItem.DbEntry == null || tagItem.DbEntry.Id < 1)
            {
                await ThreadSwitcher.ResumeForegroundAsync();
                Items.Remove(tagItem);
                return;
            }

            await Db.DeleteTagExclusion(tagItem.DbEntry.Id);

            await ThreadSwitcher.ResumeForegroundAsync();

            Items.Remove(tagItem);
        }

        public async Task LoadData()
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            DataNotifications.NewDataNotificationChannel().MessageReceived -= OnDataNotificationReceived;

            var dbItems = await Db.TagExclusions();

            var listItems = dbItems.Select(x => new TagExclusionEditorListItem {DbEntry = x, TagValue = x.Tag})
                .OrderBy(x => x.TagValue).ToList();

            if (Items == null)
            {
                await ThreadSwitcher.ResumeForegroundAsync();
                Items = new ObservableCollection<TagExclusionEditorListItem>(listItems);
                await ThreadSwitcher.ResumeBackgroundAsync();
                return;
            }

            var currentItems = Items.ToList();

            foreach (var loopListItem in listItems)
            {
                var possibleItem = currentItems.SingleOrDefault(x => x.DbEntry?.Id == loopListItem.DbEntry.Id);

                if (possibleItem == null)
                {
                    await ThreadSwitcher.ResumeForegroundAsync();
                    Items.Add(loopListItem);
                    await ThreadSwitcher.ResumeBackgroundAsync();
                }
                else
                {
                    possibleItem.DbEntry = loopListItem.DbEntry;
                }
            }

            var currentItemsWithDbEntry = currentItems.Where(x => x.DbEntry is {Id: >= 0}).ToList();
            var newItemIds = listItems.Select(x => x.DbEntry.Id);

            var deletedItems = currentItemsWithDbEntry.Where(x => !newItemIds.Contains(x.DbEntry.Id)).ToList();

            await ThreadSwitcher.ResumeForegroundAsync();
            foreach (var loopDeleted in deletedItems)
                try
                {
                    Items.Remove(loopDeleted);
                }
                catch (Exception e)
                {
                    Log.ForContext("loopDeleted", loopDeleted.SafeObjectDump()).Error(e,
                        "Suppressed error in Tag Exclusion Editor Context - delete item while Loading failed");
                    Console.WriteLine(e);
                }
        }

        private void OnDataNotificationReceived(object sender, TinyMessageReceivedEventArgs e)
        {
            DataNotificationsProcessor.Enqueue(e);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task SaveItem(TagExclusionEditorListItem tagItem)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            tagItem.TagValue = tagItem.TagValue.TrimNullToEmpty();

            var toSave = new TagExclusion();
            toSave.InjectFrom(tagItem.DbEntry);
            toSave.Tag = tagItem.TagValue.TrimNullToEmpty();

            var validation = await TagExclusionGenerator.Validate(toSave);

            if (validation.HasError)
            {
                StatusContext.ToastError($"Tag is not valid - {validation.GenerationNote}");
                return;
            }

            var saveReturn = await TagExclusionGenerator.Save(toSave);

            if (saveReturn.generationReturn.HasError)
            {
                StatusContext.ToastError($"Error Saving - {validation.GenerationNote}");
                return;
            }

            tagItem.DbEntry = saveReturn.returnContent;
        }
    }
}