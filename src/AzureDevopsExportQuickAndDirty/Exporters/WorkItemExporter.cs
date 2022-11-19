using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common.CommandLine;
using Microsoft.VisualStudio.Services.WebApi;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Serilog;
using System.Linq;
using AzureDevopsExportQuickAndDirty.Exporters.Models;

namespace AzureDevopsExportQuickAndDirty.Exporters
{
    public class WorkItemExporter
    {
        private readonly ConnectionManager _connection;

        public WorkItemExporter(ConnectionManager connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Extract information about work item, paginate because you cannot ask for an arbitrary number of work items.
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="teamProject"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<WorkItemInformations>> ExtractAllWorkItemsInfo(ExcelPackage excel, string teamProject)
        {
            //now we need to export all data in excel file.
            var ws = excel.Workbook.Worksheets.Single(w => w.Name == "WorkItems");
            ws.Cells["A1"].Value = "Id";
            ws.Cells["B1"].Value = "Type";
            ws.Cells["C1"].Value = "State";
            ws.Cells["D1"].Value = "CreationDate";
            ws.Cells["E1"].Value = "CreatedBy";
            ws.Cells["F1"].Value = "AssignedTo";
            ws.Cells["G1"].Value = "RelatedWorkItems";
            ws.Cells["H1"].Value = "Code";
            ws.Cells["I1"].Value = "PullRequest";

            int row = 2;
            int lastId = 0;

            List<WorkItemInformations> workItemInformations = new List<WorkItemInformations>();
            do
            {
                (row, lastId) = await GetAPageOfWorkItems(
                    teamProject,
                    ws,
                    row,
                    workItemInformations,
                    lastId);

                Log.Information("Import work item: Total up to now {total}", workItemInformations.Count);
            } while (lastId > 0);

            return workItemInformations;
        }

        private async Task<(int Row, int LastId)> GetAPageOfWorkItems(
            string teamProject, 
            ExcelWorksheet ws, 
            int row,
            List<WorkItemInformations> workItemInformations, 
            int initialId)
        {
            Log.Information("About to query all work items");
            var query = $@"Select
               [State],[Title]
            from 
                WorkItems 
            where 
                [System.TeamProject] = '{teamProject}' and Id > {initialId}
            order by
                Id ASC
";

            int lastId = 0;
            var wiql = new Wiql() { Query = query };
            //execute the query to get the list of work items in teh results
            WorkItemQueryResult workItemQueryResult = await _connection.WorkItemTrackingHttpClient.QueryByWiqlAsync(wiql, top: 1000);

            //now get the result.             
            if (workItemQueryResult.WorkItems.Any())
            {
                Log.Information("Loaded a block of {count} WorkItems starting from {start}", workItemQueryResult.WorkItems.Count(), initialId);
                //need to get the list of our work item id's paginated and get work item in blocks
                var count = workItemQueryResult.WorkItems.Count();
                var current = 0;
                var pageSize = 200;

                while (current < count)
                {
                    List<WorkItem> workItems = await RetrievePageOfWorkItem(_connection, workItemQueryResult, current, pageSize);

                    row = DumpPageOfWorkItems(ws, row, workItems, workItemInformations);

                    current += pageSize;
                    lastId = workItems[workItems.Count - 1].Id.Value;
                }
            }

            return (row, lastId);
        }

        private async Task<List<WorkItem>> RetrievePageOfWorkItem(ConnectionManager conn, WorkItemQueryResult workItemQueryResult, int current, int pageSize)
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
            Log.Information("Work item DETAILS result: record from {from} to {to} retrieved", current, current + pageSize);
            return workItems;
        }

        private int DumpPageOfWorkItems(ExcelWorksheet ws, int row, List<WorkItem> workItems, List<WorkItemInformations> workItemInformations)
        {
            foreach (WorkItem workItem in workItems)
            {
                Log.Debug("Loaded work item {id}.", workItem.Id);

                var info = new WorkItemInformations()
                {
                    WorkItem = workItem,
                };
                workItemInformations.Add(info);

                ws.Cells[$"A{row}"].Value = workItem.Id;
                ws.Cells[$"B{row}"].Value = workItem.Fields["System.WorkItemType"];
                ws.Cells[$"C{row}"].Value = workItem.Fields["System.State"];
                ws.Cells[$"D{row}"].Value = ((DateTime)workItem.Fields["System.CreatedDate"]);
                ws.Cells[$"E{row}"].Value = workItem.Fields.GetFieldValue<IdentityRef>("System.CreatedBy")?.DisplayName ?? "";
                ws.Cells[$"F{row}"].Value = workItem.Fields.GetFieldValue<IdentityRef>("System.AssignedTo")?.DisplayName ?? "";

                if (workItem.Relations != null)
                {
                    ws.Cells[$"G{row}"].Value = info.NumOfRelations = workItem.Relations.Count(r => WorkItemHelper.IsLinkToWorkItem(r.Url));
                    ws.Cells[$"H{row}"].Value = info.NumOfCodeRelations = workItem.Relations.Count(r => WorkItemHelper.IsLinkToCode(r.Url));
                    ws.Cells[$"I{row}"].Value = info.NumOfPullRequests = workItem.Relations.Count(r => WorkItemHelper.IsLinkToPullRequest(r.Url));
                }
                else
                {
                    ws.Cells[$"G{row}"].Value = 0;
                    ws.Cells[$"H{row}"].Value = 0;
                    ws.Cells[$"I{row}"].Value = 0;
                }
                row++;
            }

            return row;
        }
    }
}
