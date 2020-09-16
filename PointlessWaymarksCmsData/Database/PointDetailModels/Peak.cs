﻿namespace PointlessWaymarksCmsData.Database.PointDetailModels
{
    public class Peak : IPointDetail
    {
        public string DataTypeIdentifier => "Peak";
        public string Notes { get; set; }
        public string NotesContentFormat { get; set; }

        public (bool isValid, string validationMessage) Validate()
        {
            return (true, string.Empty);
        }
    }
}