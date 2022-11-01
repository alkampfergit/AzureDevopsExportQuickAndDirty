using CommandLine;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using OfficeOpenXml;
using Serilog;
using Serilog.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

                //await ExtractAllWorkItemsInfo(conn, excel);

                //await ExtractPipelineInformations(conn, excel);

                var ws = excel.Workbook.Worksheets.Add("Source");
                ws.Cells["A1"].Value = "Id";
                ws.Cells["B1"].Value = "Type";
                ws.Cells["C1"].Value = "Name";
                ws.Cells["D1"].Value = "Commit/changeset";

                List<TfvcChangesetRef> allChangesets = new List<TfvcChangesetRef>(1000);
                List<TfvcChangesetRef> block;
                var searchCriteria = new TfvcChangesetSearchCriteria();
                searchCriteria.ItemPath = $"$/{_options.TeamProject}";
                block = await conn.TfvcHttpClient.GetChangesetsAsync(searchCriteria: searchCriteria);
                
                while (block.Count > 0)
                {
                    Log.Information("Retrieved a block of TFVC changeset of size {size} - latest {latest}", block.Count, block[block.Count -1].ChangesetId);
                    allChangesets.AddRange(block);
                    searchCriteria.ToId = block[block.Count - 1].ChangesetId - 1;
                    
                    //search again
                    block = await conn.TfvcHttpClient.GetChangesetsAsync(searchCriteria: searchCriteria);
                };

                ws.Cells["A2"].Value = "TFVC";
                ws.Cells["B2"].Value = "TFVC";
                ws.Cells["C2"].Value = "TFVC";
                ws.Cells["D2"].Value = allChangesets.Count;

                excel.Save();
            }

            System.Diagnostics.Process.Start(newFile.FullName);
            Console.ReadKey();
        }

        private static async Task ExtractPipelineInformations(ConnectionManager conn, ExcelPackage excel)
        {
            var builds = await conn.BuildHttpClient.GetDefinitionsAsync2(project: _options.TeamProject);
            Log.Information("Found {count} pipelines", builds.Count);

            //now we need to export all data in excel file.
            var ws = excel.Workbook.Worksheets.Add("Pipelines");
            ws.Cells["A1"].Value = "Id";
            ws.Cells["B1"].Value = "Name";
            ws.Cells["C1"].Value = "Folder";
            ws.Cells["D1"].Value = "Url";
            ws.Cells["E1"].Value = "LatestSuccessfulBuild";
            ws.Cells["F1"].Value = "Repository";
            ws.Cells["G1"].Value = "TotalRuns";

            int row = 2;

            foreach (var pipeline in builds)
            {
                Log.Information("Getting information details for pipeline {pipeline}", pipeline.Name);
                var details = await conn.BuildHttpClient.GetDefinitionAsync(_options.TeamProject, pipeline.Id);

                var buildResults = await conn.BuildHttpClient.GetBuildsAsync2(
                    project: _options.TeamProject,
                    definitions: new[] { pipeline.Id });

                ws.Cells[$"A{row}"].Value = pipeline.Id;
                ws.Cells[$"B{row}"].Value = pipeline.Name;
                ws.Cells[$"C{row}"].Value = details.Path;
                ws.Cells[$"D{row}"].Value = pipeline.Url;

                var latestGoodResult = buildResults
                    .Where(br => br.Status == Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed
                    && br.Result == Microsoft.TeamFoundation.Build.WebApi.BuildResult.Succeeded)
                    .OrderByDescending(r => r.FinishTime)
                    .FirstOrDefault();

                ws.Cells[$"E{row}"].Value = latestGoodResult?.FinishTime?.ToString("yyyy/MM/dd");
                ws.Cells[$"F{row}"].Value = details.Repository.Name;

                row++;
            }
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
                var pageSize = 200;

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

            ////build a list of the fields we want to see
            //string[] fields = new string[]
            //{
            //            "System.CreatedBy",
            //            "System.CreatedDate",
            //            "System.State",
            //            "System.CreatedBy",
            //            "System.AssignedTo",
            //            "System.WorkItemType"
            //};

            ////get work items for the id's found in query
            //var workItems = await conn.WorkItemTrackingHttpClient.GetWorkItemsAsync(
            //    list,
            //    fields,
            //    workItemQueryResult.AsOf);

            // var workItemsRelations = await conn.WorkItemTrackingHttpClient.GetWorkItemsAsync(
            //    list,
            //    fields: new[] { "System.Id"},
            //    expand: WorkItemExpand.Relations);

            var workItems = await conn.WorkItemTrackingHttpClient.GetWorkItemsAsync(
                list,
                expand: WorkItemExpand.Relations);
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

                if (workItem.Relations != null)
                {
                    ws.Cells[$"G{row}"].Value = workItem.Relations.Count(r => WorkItemHelper.IsLinkToWorkItem(r.Url));
                    ws.Cells[$"H{row}"].Value = workItem.Relations.Count(r => WorkItemHelper.IsLinkToCode(r.Url));
                    ws.Cells[$"I{row}"].Value = workItem.Relations.Count(r => WorkItemHelper.IsLinkToPullRequest(r.Url));
                }
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
