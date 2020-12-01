using System;
using System.Collections.Generic;

namespace RemoteCameraControl.File
{
    public static class FileTypeResolver
    {
        private const string UnKnownFileMimeType = "application/octet-stream";

        private static readonly IReadOnlyDictionary<string, string> _mimeTypeMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"png", "image/png"},

            {"jpg", "image/jpeg"},
            {"jpeg", "image/jpeg"},

            {"tif", "image/tiff"},
            {"tiff", "image/tiff"},

            {"gif", "image/gif"},

            {"bmp", "image/bmp"},

            {"pdf", "application/pdf"},
            {"txt", "text/plain"},
            {"doc", "application/msword"},
            {"docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            {"xls", "application/vnd.ms-excel"},
            {"xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {"html", "text/html"},
            {"htm", "text/html"},
        };

        internal static string GetFileMimeType(string fileExtension)
        {
            return _mimeTypeMap.ContainsKey(fileExtension) ? _mimeTypeMap[fileExtension] : UnKnownFileMimeType;
        }
    }
}
