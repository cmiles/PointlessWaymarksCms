﻿using System;
using System.Linq;

namespace PointlessWaymarksCmsData
{
    public static class DateTimeHelpers
    {
        /// <summary>
        ///     Uses reflection to truncate all DateTime and DateTime? properties to the second
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toProcess"></param>
        /// <returns></returns>
        public static T TrimDateTimesToSecond<T>(T toProcess)
        {
            var dateTimeProperties = typeof(T).GetProperties()
                .Where(x => x.PropertyType == typeof(DateTime) && x.GetSetMethod() != null).ToList();

            foreach (var loopProperty in dateTimeProperties)
            {
                var current = (DateTime) loopProperty.GetValue(toProcess);
                loopProperty.SetValue(toProcess,
                    new DateTime(current.Year, current.Month, current.Day, current.Hour, current.Minute,
                        current.Second));
            }

            var nullableDateTimeProperties = typeof(T).GetProperties()
                .Where(x => x.PropertyType == typeof(DateTime?) && x.GetSetMethod() != null).ToList();

            foreach (var loopProperties in nullableDateTimeProperties)
            {
                var current = (DateTime?) loopProperties.GetValue(toProcess);
                if (current == null) continue;
                DateTime? newDateTime = new DateTime(current.Value.Year, current.Value.Month, current.Value.Day,
                    current.Value.Hour, current.Value.Minute, current.Value.Second);
                loopProperties.SetValue(toProcess, newDateTime);
            }

            return toProcess;
        }
    }
}