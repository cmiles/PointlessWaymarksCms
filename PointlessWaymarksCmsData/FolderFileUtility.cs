﻿using System.IO;
using System.Text.RegularExpressions;

namespace PointlessWaymarksCmsData
{
    public static class FolderFileUtility
    {
        public static string InvalidFileNameCharsRegexPattern()
        {
            return $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]";
        }

        /// <summary>
        ///     This checks if a File Name 'Is Valid' which currently means only contains a limited subset of characters that do
        ///     not
        ///     need to be URL encoded.
        /// </summary>
        /// <param name="testName"></param>
        /// <returns></returns>
        public static bool IsNoUrlEncodingNeededFilename(string testName)
        {
            if (string.IsNullOrWhiteSpace(testName)) return false;

            return Regex.IsMatch(testName, @"^[a-zA-Z\d_\-\.]+$", RegexOptions.IgnoreCase);
        }

        public static bool IsValidWindowsFileSystemFilename(string testName)
        {
            //https://stackoverflow.com/questions/62771/how-do-i-check-if-a-given-string-is-a-legal-valid-file-name-under-windows
            var containsABadCharacter = new Regex(InvalidFileNameCharsRegexPattern());
            if (containsABadCharacter.IsMatch(testName)) return false;

            return true;
        }

        public static bool PhotoFileTypeIsSupported(FileInfo toCheck)
        {
            if (toCheck == null) return false;
            if (!toCheck.Exists) return false;

            return toCheck.Extension.ToLower().Contains("jpg") || toCheck.Extension.ToLower().Contains("jpeg");
        }

        public static string TryMakeFilenameValid(string filenameWithoutExtensionToTransform)
        {
            return string.IsNullOrWhiteSpace(filenameWithoutExtensionToTransform)
                ? string.Empty
                : Regex.Replace(filenameWithoutExtensionToTransform, InvalidFileNameCharsRegexPattern(), string.Empty);
        }
    }
}