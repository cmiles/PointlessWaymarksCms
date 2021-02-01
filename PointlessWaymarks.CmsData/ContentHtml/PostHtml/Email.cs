﻿using System;
using System.Threading.Tasks;
using PointlessWaymarks.CmsData.CommonHtml;
using PointlessWaymarks.CmsData.Database.Models;

namespace PointlessWaymarks.CmsData.ContentHtml.PostHtml
{
    public static class Email
    {
        public static async Task<string> ToHtmlEmail(PostContent? content, IProgress<string>? progress = null)
        {
            if (content == null) return string.Empty;

            var preprocessResults = BracketCodeCommon.ProcessCodesForEmail(content.BodyContent, progress);
            var bodyHtmlString = ContentProcessing.ProcessContent(preprocessResults, content.BodyContentFormat);

            var tags = Tags.TagListTextLinkList(content);
            tags.Style("text-align", "center");

            var createdUpdated = $"<p style=\"text-align: center;\">{Tags.CreatedByAndUpdatedOnString(content)}</p>";

            var innerContent = HtmlEmail.ChildrenIntoTableCells(
                $"{await HtmlEmail.EmailSimpleTitle(content)}{bodyHtmlString}{tags}{createdUpdated}{HtmlEmail.EmailSimpleFooter()}");

            var emailHtml = HtmlEmail.WrapInNestedCenteringTable(innerContent);

            return emailHtml;
        }
    }
}