﻿using PointlessWaymarks.CmsData.Content;

namespace PointlessWaymarks.CmsData.Database.PointDetailDataModels
{
    public class Peak : IPointDetailData
    {
        public string Notes { get; set; }
        public string NotesContentFormat { get; set; } = ContentFormatDefaults.Content.ToString();
        public string DataTypeIdentifier => "Peak";

        public (bool isValid, string validationMessage) Validate()
        {
            var formatValidation = CommonContentValidation.ValidateBodyContentFormat(NotesContentFormat);
            if (!formatValidation.isValid) return (false, formatValidation.explanation);

            return (true, string.Empty);
        }
    }
}