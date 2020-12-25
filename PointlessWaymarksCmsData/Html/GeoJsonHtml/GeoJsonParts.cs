﻿using System;
using PointlessWaymarksCmsData.Database.Models;

namespace PointlessWaymarksCmsData.Html.GeoJsonHtml
{
    public static class GeoJsonParts
    {
        public static string GeoJsonDivAndScript(GeoJsonContent content)
        {
            var divScriptGuidConnector = Guid.NewGuid();

            var tag =
                $"<div id=\"GeoJson-{divScriptGuidConnector}\" class=\"leaflet-container leaflet-retina leaflet-fade-anim leaflet-grab leaflet-touch-drag point-content-map\"></div>";

            var script =
                $"<script>lazyInit(document.querySelector(\"#GeoJson-{divScriptGuidConnector}\"), () => singleGeoJsonMapInit(document.querySelector(\"#GeoJson-{divScriptGuidConnector}\"), \"{content.ContentId}\"))</script>";

            return tag + script;
        }

        public static string GeoJsonDivAndScriptWithCaption(GeoJsonContent content)
        {
            var titleCaption = $"<a class=\"map-figure-title-caption\" href=\"{UserSettingsSingleton.CurrentSettings().GeoJsonPageUrl(content)}\">{content.Title}</a>";

            return $"<figure class=\"map-figure\">{GeoJsonDivAndScript(content)}{titleCaption}</figure>";
        }
    }
}