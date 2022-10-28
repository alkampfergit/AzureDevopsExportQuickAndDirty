using System;

namespace AzureDevopsExportQuickAndDirty
{
    internal static class WorkItemHelper
    {
        public static bool IsLinkToCode(string relationUri)
        {
            return relationUri.IndexOf("Changeset", StringComparison.OrdinalIgnoreCase) > 0
                || relationUri.IndexOf("Commit", StringComparison.OrdinalIgnoreCase) > 0;
        }

        public static bool IsLinkToWorkItem(string relationUri)
        {
            return relationUri.IndexOf("_apis/wit/workItems", StringComparison.OrdinalIgnoreCase) > 0;
        }

        public static bool IsLinkToPullRequest(string relationUri)
        {
            return relationUri.IndexOf("PullRequest", StringComparison.OrdinalIgnoreCase) > 0;
        }
    }
}
