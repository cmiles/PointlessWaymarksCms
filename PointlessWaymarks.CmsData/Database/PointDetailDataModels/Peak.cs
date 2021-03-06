﻿using PointlessWaymarks.CmsData.Content;

namespace PointlessWaymarks.CmsData.Database.PointDetailDataModels
{
    public class Peak : IPointDetailData
    {
        public string? Notes { get; set; }
        public string NotesContentFormat { get; set; } = ContentFormatDefaults.Content.ToString();
        public string DataTypeIdentifier => "Peak";

        public IsValid Validate()
        {
            var formatValidation = CommonContentValidation.ValidateBodyContentFormat(NotesContentFormat);
            if (!formatValidation.Valid) return new IsValid(false, formatValidation.Explanation);

            return new IsValid(true, string.Empty);
        }
    }
}