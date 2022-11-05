using Serilog;
using System;

namespace AzureDevopsExportQuickAndDirty.Support
{
    internal static class ConsoleUtils
    {
        public static void ErrorAndExit(string error)
        {
            Log.Error(error);
            if (Environment.UserInteractive)
            {
                Console.Write("Press a key to continue");
                Console.ReadKey();
            }
        }
    }
}
