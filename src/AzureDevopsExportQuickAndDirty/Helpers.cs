using System.Collections.Generic;

namespace AzureDevopsExportQuickAndDirty
{
    internal static class Helpers
    {
        public static T GetFieldValue<T>(this IDictionary<string, object> dic, string fieldValue)
        {
            if (dic.TryGetValue(fieldValue, out var value))
            {
                return (T)value;
            }

            return default;
        }
    }
}
