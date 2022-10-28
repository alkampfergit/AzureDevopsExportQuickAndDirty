using CommandLine;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using OfficeOpenXml;
using Serilog;
using Serilog.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AzureDevopsExportQuickAndDirty
{
    public class Program
    {
        private static Options _options;

        static void Main(string[] args)
        {
            ConfigureSerilog();

            var result = CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts => _options = opts)
                .WithNotParsed<Options>((errs) => HandleParseError(errs));

            ConnectionManager conn = new ConnectionManager(_options.ServiceAddress, _options.AccessToken);

            var fileName = Path.GetTempFileName() + ".xlsx";
            FileInfo newFile = new FileInfo(fileName);

            using (var excel = new ExcelPackage(newFile))
            {
                Log.Information("Created temporary excel file {file}", newFile);

                ExtractAllWorkItemsInfo(conn, excel);

                excel.Save();
            }

            System.Diagnostics.Process.Start(newFile.FullName);
            Console.ReadKey();
        }

        private static void ExtractAllWorkItemsInfo(ConnectionManager conn, ExcelPackage excel)
        {
            Log.Information("About to query all work items");
            var query = $@"Select
                System.CreatedBy,
                System.CreatedDate,
                System.State,
                System.CreatedBy,
                System.AssignedTo
            from 
                WorkItems 
            where 
                [System.TeamProject] = '{_options.TeamProject}'";

            var queryResult = conn.WorkItemStore.Query(query);
            var allWorkItems = queryResult.OfType<WorkItem>().ToList();

            Log.Information("Loaded In memory {count} work items", allWorkItems.Count);

            //now we need to export all data in excel file.
            var ws = excel.Workbook.Worksheets.Add("workitem");
            ws.Cells["A1"].Value = "Id";
            ws.Cells["B1"].Value = "Type";
            ws.Cells["C1"].Value = "State";
            ws.Cells["D1"].Value = "CreationDate";
            ws.Cells["E1"].Value = "CreatedBy";
            ws.Cells["F1"].Value = "AssignedTo";
            ws.Cells["G1"].Value = "RelatedWorkItems";
            ws.Cells["H1"].Value = "Code";
            ws.Cells["I1"].Value = "PullRequest";
            Int32 row = 2;
            foreach (WorkItem workItem in allWorkItems.Take(1000))
            {
                Log.Debug("Loaded work item {id}.", workItem.Id);

                ws.Cells[$"A{row}"].Value = workItem.Id;
                ws.Cells[$"B{row}"].Value = workItem.Type.Name;
                ws.Cells[$"C{row}"].Value = workItem.State;
                ws.Cells[$"D{row}"].Value = workItem.CreatedDate.ToString("yyyy/MM/dd");
                ws.Cells[$"E{row}"].Value = workItem.CreatedBy;
                ws.Cells[$"F{row}"].Value = workItem.Fields["system.assignedTo"].Value;

                ws.Cells[$"G{row}"].Value = workItem.Links.OfType<RelatedLink>().Count();
                ws.Cells[$"H{row}"].Value = workItem.Links.OfType<ExternalLink>().Where(el => el.ArtifactLinkType.Name.Contains("Commit")).Count();
                ws.Cells[$"I{row}"].Value = workItem.Links.OfType<ExternalLink>().Where(el => el.ArtifactLinkType.Name.Contains("Pull Request")).Count();

                row++;
            }
        }

        private static bool ShouldPrintSprint(List<String> sprints, WorkItem w)
        {
            foreach (var sprint in sprints)
            {
                if (w.IterationPath.IndexOf(sprint, StringComparison.OrdinalIgnoreCase) > -1)
                    return true;
            }

            return false;
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
        }

        private static void ConfigureSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(
                    "logs\\logs.txt",
                     rollingInterval: RollingInterval.Day
                )
                .WriteTo.File(
                    "logs\\errors.txt",
                     rollingInterval: RollingInterval.Day,
                     restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error
                )
                .CreateLogger();
        }
    }
}
