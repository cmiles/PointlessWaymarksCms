﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PointlessWaymarks.CmsData;
using PointlessWaymarks.CmsData.Database;
using PointlessWaymarks.CmsWpfControls.ContentList;

namespace PointlessWaymarks.CmsWpfControls.PostList
{
    public class PostListLoader : ContentListLoaderBase
    {
        public PostListLoader(int? partialLoadQuantity) : base("Posts", partialLoadQuantity)
        {
            DataNotificationTypesToRespondTo = new List<DataNotificationContentType> {DataNotificationContentType.Post};
        }

        public override async Task<bool> CheckAllItemsAreLoaded()
        {
            if (PartialLoadQuantity == null) return true;

            var db = await Db.Context();

            return !(await db.PostContents.CountAsync() > PartialLoadQuantity);
        }

        public override async Task<List<object>> LoadItems(IProgress<string> progress = null)
        {
            var listItems = new List<object>();

            var db = await Db.Context();

            if (PartialLoadQuantity != null)
            {
                progress?.Report($"Loading Post Content from DB - Max {PartialLoadQuantity} Items");
                listItems.AddRange(await db.PostContents.OrderByDescending(x => x.LastUpdatedOn ?? x.CreatedOn)
                    .Take(PartialLoadQuantity.Value).ToListAsync());

                AllItemsLoaded = await CheckAllItemsAreLoaded();

                return listItems;
            }

            progress?.Report("Loading Post Content from DB");
            listItems.AddRange(await db.PostContents.OrderByDescending(x => x.LastUpdatedOn ?? x.CreatedOn)
                .ToListAsync());

            return listItems;
        }
    }
}