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

        public async Task<IReadOnlyCollection<WorkItemInformations>> ExtractAllWorkItemsInfo(ExcelPackage excel, string teamProject, string limit = null)
        {
            List<WorkItemInformations> workItemInformations = new List<WorkItemInformations>();
            var results = new List<WorkItemReference>();
            var counter = 10000;
            Log.Information("About to query all work items");
            var moreResults = true;
            if (limit != null)
                limit = " AND System.ChangedDate >= '" + limit + "'";
            else
                limit = String.Empty;
            while (moreResults)
            {
                var query = $@"Select
               [State],[Title]
            from 
                WorkItems 
            where 
                [System.TeamProject] = '{teamProject}'{limit} AND System.ID >= {counter - 10000} AND System.ID < {counter} ORDER BY [System.ChangedDate] DESC";

                var wiql = new Wiql() { Query = query };
                //execute the query to get the list of work items to 10000 results
                WorkItemQueryResult workItemQueryResult = await _connection.WorkItemTrackingHttpClient.QueryByWiqlAsync(wiql);
                if (workItemQueryResult.WorkItems.Count() == 0)
                {
                    try
                    {
                        query = $@"Select
               [State],[Title]
            from 
                WorkItems 
            where 
                [System.TeamProject] = '{teamProject}'{limit} AND System.ID >= {counter} ORDER BY [System.ChangedDate] DESC";
                        wiql = new Wiql() { Query = query };
                        //execute the query to get the list of all the others work items (results > 10000)
                        workItemQueryResult = await _connection.WorkItemTrackingHttpClient.QueryByWiqlAsync(wiql);

                        results.AddRange(workItemQueryResult.WorkItems.ToList());

                        moreResults = false;
                    }
                    catch (Exception e)
                    {
                        if (e.ToString().Contains("VS402337"))
                        {
                            // There are still more results, so increment and try again.
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    results.AddRange(workItemQueryResult.WorkItems);
                }

                counter += 10000;
            }
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
            Int32 row = 2;

            //now get the result.             
            if (results.Any())
            {
                //need to get the list of our work item id's paginated and get work item in blocks
                var count = results.Count();
                var current = 0;
                var pageSize = 200;

                while (current < count)
                {
                    List<WorkItem> workItems = await RetrievePageOfWorkItem(_connection, results, current, pageSize);

                    row = DumpPageOfWorkItems(ws, row, workItems, workItemInformations);

                    current += pageSize;
                }
            }

            return workItemInformations;
        }


        private async Task<List<WorkItem>> RetrievePageOfWorkItem(ConnectionManager conn, List<WorkItemReference> workItemRefs, int current, int pageSize)
        {
            List<int> list = workItemRefs
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
