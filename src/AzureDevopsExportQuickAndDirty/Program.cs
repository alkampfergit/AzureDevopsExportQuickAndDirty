using CommandLine;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using OfficeOpenXml;
using Serilog;
using Serilog.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureDevopsExportQuickAndDirty
{
    public class Program
    {
        private static Options _options;

        static async Task Main(string[] args)
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

                await ExtractAllWorkItemsInfo(conn, excel);

                excel.Save();
            }

            System.Diagnostics.Process.Start(newFile.FullName);
            Console.ReadKey();
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach (var parseError in errs)
            {
                Log.Error("Error parsing arguments: {error}", parseError.Tag);
            }
        }

        private static async Task ExtractAllWorkItemsInfo(ConnectionManager conn, ExcelPackage excel)
        {
            Log.Information("About to query all work items");
            var query = $@"Select
               [State],[Title]
            from 
                WorkItems 
            where 
                [System.TeamProject] = '{_options.TeamProject}'";

            var wiql = new Wiql() { Query = query };
            //execute the query to get the list of work items in teh results
            WorkItemQueryResult workItemQueryResult = await conn.WorkItemTrackingHttpClient.QueryByWiqlAsync(wiql);

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

            //now get the result.             
            if (workItemQueryResult.WorkItems.Any())
            {
                //need to get the list of our work item id's paginated and get work item in blocks
                var count = workItemQueryResult.WorkItems.Count();
                var current = 0;
                var pageSize = 100;

                while (current < count)
                {
                    List<WorkItem> workItems = await RetrievePageOfWorkItem(conn, workItemQueryResult, current, pageSize);

                    row = DumpPageOfWorkItems(ws, row, workItems);

                    current += pageSize;
                }
            }
        }

        private static async Task<List<WorkItem>> RetrievePageOfWorkItem(ConnectionManager conn, WorkItemQueryResult workItemQueryResult, int current, int pageSize)
        {
            List<int> list = workItemQueryResult
                                    .WorkItems
                                    .Select(wi => wi.Id)
                                    .Skip(current)
                                    .Take(pageSize)
                                    .ToList();

            //build a list of the fields we want to see
            string[] fields = new string[]
            {
                        "System.CreatedBy",
                        "System.CreatedDate",
                        "System.State",
                        "System.CreatedBy",
                        "System.AssignedTo",
                        "System.WorkItemType"
            };

            //get work items for the id's found in query
            var workItems = await conn.WorkItemTrackingHttpClient.GetWorkItemsAsync(
                list,
                fields,
                workItemQueryResult.AsOf);

            Log.Information("Query Results: record from {from} to {to} retrieved", current, current + pageSize);
            return workItems;
        }

        private static int DumpPageOfWorkItems(ExcelWorksheet ws, int row, List<WorkItem> workItems)
        {
            foreach (WorkItem workItem in workItems)
            {
                Log.Debug("Loaded work item {id}.", workItem.Id);

                ws.Cells[$"A{row}"].Value = workItem.Id;
                ws.Cells[$"B{row}"].Value = workItem.Fields["System.WorkItemType"];
                ws.Cells[$"C{row}"].Value = workItem.Fields["System.State"];
                ws.Cells[$"D{row}"].Value = ((DateTime)workItem.Fields["System.CreatedDate"]).ToString("yyyy/MM/dd");
                ws.Cells[$"E{row}"].Value = workItem.Fields.GetFieldValue<IdentityRef>("System.CreatedBy")?.DisplayName ?? "";
                ws.Cells[$"F{row}"].Value = workItem.Fields.GetFieldValue<IdentityRef>("System.AssignedTo")?.DisplayName ?? "";

                //ws.Cells[$"G{row}"].Value = workItem.Links.OfType<RelatedLink>().Count();
                //ws.Cells[$"H{row}"].Value = workItem.Links.OfType<ExternalLink>().Where(el => el.ArtifactLinkType.Name.Contains("Commit")).Count();
                //ws.Cells[$"I{row}"].Value = workItem.Links.OfType<ExternalLink>().Where(el => el.ArtifactLinkType.Name.Contains("Pull Request")).Count();

                row++;
            }

            return row;
        }

        private static void ConfigureSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .MinimumLevel.Information()
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
