using CommandLine;
using System;

namespace AzureDevopsExportQuickAndDirty
{
    public class Options
    {
        [Option(
            "address",
            Required = true,
            HelpText = "Service address, ex https://dev.azure.com/organization")]
        public String ServiceAddress { get; set; }

        [Option(
             "accesstoken",
             Required = false,
             HelpText = "Access token")]
        public String AccessToken { get; set; }

        [Option(
            "teamproject",
            Required = true,
            HelpText = "Name of the teamproject")]
        public String TeamProject { get; set; }

        [Option(
            "outfolder",
            Required = false,
            HelpText = "name of folder where excel files will be generated")]
        public String OutFolder { get; set; }

        [Option(
            "limit",
            Required = false,
            HelpText = "Data Limit es. 01-01-2022 GMT")]

        public String Limit { get; set; }


        internal string GetOuputFolder()
        {
            if (String.IsNullOrEmpty(OutFolder))
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }

            return OutFolder;
        }

        //[Option(
        //    "sprints",
        //    Required = false,
        //    HelpText = "Sprints comma separated")]
        //public String Sprints { get; set; } = String.Empty;
    }
}
