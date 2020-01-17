﻿using System;
using System.Text.RegularExpressions;

namespace PointlessWaymarksCmsData.CommonHtml
{
    public static class BracketCodeCommon
    {
        /// <summary>
        ///     Extracts the Guid from the first {{(photo|image) guid;human_identifier}} in the string.
        /// </summary>
        /// <param name="toProcess"></param>
        /// <returns></returns>
        public static Guid? PhotoOrImageCodeFirstIdInContent(string toProcess)
        {
            if (string.IsNullOrWhiteSpace(toProcess)) return null;

            var regexObj = new Regex(@"{{(?:photo|image) (?<siteGuid>[\dA-Za-z-]*);[^}]*}}", RegexOptions.Singleline);
            var matchResult = regexObj.Match(toProcess);
            if (matchResult.Success) return Guid.Parse(matchResult.Groups["siteGuid"].Value);

            return null;
        }

        public static string ProcessCodesForLocalDisplay(string input)
        {
            input = BracketCodePhotos.PhotoCodeProcessForDirectLocalAccess(input);
            input = BracketCodeImages.ImageCodeProcessForDirectLocalAccess(input);

            return input;
        }

        public static string ProcessCodesForSite(string input)
        {
            input = BracketCodePhotos.PhotoCodeProcessToFigureWithLink(input);
            input = BracketCodeImages.ImageCodeProcessToFigureWithLink(input);

            return input;
        }
    }
}