using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AzureDevopsExportQuickAndDirty.Support
{
    public static class StringUtils
    {
        private static HashSet<char> _invalidChars = new HashSet<char>(Path.GetInvalidFileNameChars());

        public static string SanitizeForFileSystem(this string sourceString)
        {
            StringBuilder sb = new StringBuilder(sourceString.Length);
            foreach (var c in sourceString)
            {
                if (!_invalidChars.Contains(c)) sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
