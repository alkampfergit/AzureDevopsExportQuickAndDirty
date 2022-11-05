using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureDevopsExportQuickAndDirty.Support
{
    internal static class FileSystemUtils
    {
        internal static void SafeDelete(string folderToDelete)
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (Directory.Exists(folderToDelete))
                        {
                            foreach (var file in Directory.GetFiles(folderToDelete, "*.*", SearchOption.AllDirectories))
                            {
                                File.SetAttributes(file, FileAttributes.Normal);
                                File.Delete(file);
                            }
                            Directory.Delete(folderToDelete, true);
                        }
                        break;
                    }
                    catch (Exception)
                    {
                        await Task.Delay(1000);
                    }
                }
            });
        }
    }
}
