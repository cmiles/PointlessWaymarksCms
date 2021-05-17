﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using PointlessWaymarks.CmsData;

namespace PointlessWaymarks.CmsWpfControls.ContentList
{
    public interface IContentListLoader : INotifyPropertyChanged
    {
        bool AddNewItemsFromDataNotifications { get; set; }
        bool AllItemsLoaded { get; set; }
        List<DataNotificationContentType> DataNotificationTypesToRespondTo { get; set; }
        int? PartialLoadQuantity { get; set; }
        Task<bool> CheckAllItemsAreLoaded();
        Task<List<object>> LoadItems(IProgress<string> progress = null);
    }
}