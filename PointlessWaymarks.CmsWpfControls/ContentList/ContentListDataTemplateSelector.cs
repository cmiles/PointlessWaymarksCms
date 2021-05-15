﻿using System.Windows;
using System.Windows.Controls;
using PointlessWaymarks.CmsWpfControls.FileList;
using PointlessWaymarks.CmsWpfControls.GeoJsonList;
using PointlessWaymarks.CmsWpfControls.ImageList;
using PointlessWaymarks.CmsWpfControls.LineList;
using PointlessWaymarks.CmsWpfControls.MapComponentList;
using PointlessWaymarks.CmsWpfControls.NoteList;
using PointlessWaymarks.CmsWpfControls.PhotoList;
using PointlessWaymarks.CmsWpfControls.PointList;
using PointlessWaymarks.CmsWpfControls.PostList;

namespace PointlessWaymarks.CmsWpfControls.ContentList
{
    public class ContentListDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FileTemplate { get; set; }
        public DataTemplate GeoJsonTemplate { get; set; }
        public DataTemplate ImageTemplate { get; set; }
        public DataTemplate LineTemplate { get; set; }
        public DataTemplate MapComponentTemplate { get; set; }
        public DataTemplate NoteTemplate { get; set; }
        public DataTemplate PhotoTemplate { get; set; }
        public DataTemplate PointTemplate { get; set; }
        public DataTemplate PostTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                FileListListItem => FileTemplate,
                GeoJsonListListItem => GeoJsonTemplate,
                ImageListListItem => ImageTemplate,
                LineListListItem => LineTemplate,
                MapComponentListListItem => MapComponentTemplate,
                NoteListListItem => NoteTemplate,
                PhotoListListItem => PhotoTemplate,
                PointListListItem => PointTemplate,
                PostListListItem => PostTemplate,
                _ => null
            };
        }
    }
}