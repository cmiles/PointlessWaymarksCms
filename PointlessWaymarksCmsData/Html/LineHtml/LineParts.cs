﻿using System;
using PointlessWaymarksCmsData.Database.Models;

namespace PointlessWaymarksCmsData.Html.LineHtml
{
    public static class LineParts
    {
        public static string LineDivAndScript(LineContent content)
        {
            var divScriptGuidConnector = Guid.NewGuid();

            var tag =
                $"<div id=\"Line-{divScriptGuidConnector}\" class=\"leaflet-container leaflet-retina leaflet-fade-anim leaflet-grab leaflet-touch-drag point-content-map\"></div>";

            var script =
                $"<script>lazyInit(document.querySelector(\"#Line-{divScriptGuidConnector}\"), () => singleLineMapInit(document.querySelector(\"#Line-{divScriptGuidConnector}\"), \"{content.ContentId}\"))</script>";

            return tag + script;
        }

        public static string LineDivAndScriptWithCaption(LineContent content)
        {
            var titleCaption =
                $"<a class=\"map-figure-title-caption\" href=\"{UserSettingsSingleton.CurrentSettings().LinePageUrl(content)}\">{content.Title}</a>";

            return $"<figure class=\"map-figure\">{LineDivAndScript(content)}{titleCaption}</figure>";
        }
    }
}