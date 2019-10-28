using CommandLine;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
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

            var query = $@"Select * from WorkItems 
            where 
                [System.WorkItemType] = 'Product Backlog Item' AND
                [System.TeamProject] = '{_options.TeamProject}'
";

            var queryResult = conn.WorkItemStore.Query(query);

            var fileName = Path.GetTempFileName() + ".xlsx";
            FileInfo newFile = new FileInfo(fileName);

            var sprints = _options.Sprints
                .Split(',')
                .Select(s => "\\sprint " + s)
                .ToList();

            var workItems = queryResult
                .OfType<WorkItem>()
                .Where(w => ShouldPrintSprint(sprints, w))
                .ToList();

            using (var p = new ExcelPackage(newFile))
            {
                var wiGrouped = workItems.GroupBy(w => w.IterationPath);

                foreach (var group in wiGrouped)
                {
                    var sprintName = group.Key.Split('\\', '/').Last();
                    //A workbook must have at least on cell, so lets add one... 
                    var ws = p.Workbook.Worksheets.Add(sprintName);

                    ws.Cells["A1"].Value = "Id";
                    ws.Cells["B1"].Value = "Titolo";
                    ws.Cells["C1"].Value = "Sprint";
                    ws.Cells["D1"].Value = "Stato";
                    ws.Cells["E1"].Value = "Status Change Date";
                    ws.Cells["F1"].Value = "Status Change User";
                    Int32 row = 2;

                    foreach (WorkItem workItem in group)
                    {
                        Log.Information("Loaded work item {id}.", workItem.Id);

                        ws.Cells[$"A{row}"].Value = workItem.Id;
                        ws.Cells[$"B{row}"].Value = workItem.Title;
                        ws.Cells[$"C{row}"].Value = workItem.IterationPath.Split('/', '\\').Last();
                        ws.Cells[$"D{row}"].Value = workItem.State;

                        var stateRevision = workItem.Revisions
                            .OfType<Revision>()
                            .OrderByDescending(r => r.Fields["System.ChangedDate"].Value as DateTime?)
                            .Where(r => r.Fields["System.State"].OriginalValue != r.Fields["System.State"].Value)
                            .FirstOrDefault();

                        if (stateRevision != null)
                        {
                            var changeDate = (DateTime)stateRevision.Fields["System.ChangedDate"].Value;
                            ws.Cells[$"E{row}"].Value = changeDate.ToString("dd/MM/yyyy");
                            ws.Cells[$"F{row}"].Value = stateRevision.Fields["System.ChangedBy"].Value;
                        }
                        //for (int i = 0; i < quoteBomReadModel.Rows.Count; i++)
                        //{
                        //    var row = quoteBomReadModel.Rows[i];
                        //    var excelrow = 9 + i + 1;
                        //    ws.Cells[9, 1, 9, 13].Copy(ws.Cells[excelrow, 1]);

                        //    if (!String.IsNullOrEmpty(row.PlmRelatedObject))
                        //    {
                        //        if (row.Type == ProductsCatalog.Shared.Model.QuoteBom.Events.QuoteBomRowType.Item)
                        //        {
                        //            ws.Cells[excelrow, 3].Value = "MACCHINA";
                        //            if (items.TryGetValue(row.PlmRelatedObject, out var rowItem))
                        //            {
                        //                ws.Cells[excelrow, 4].Value = rowItem.Code;
                        //                ws.Cells[excelrow, 5].Value = rowItem.GeneralDataReadModel.GetTitle("it", "en");
                        //            }
                        //        }
                        //        else
                        //        {
                        //            ws.Cells[excelrow, 3].Value = "accessorio";
                        //            if (parts.TryGetValue(row.PlmRelatedObject, out var rowPart))
                        //            {
                        //                ws.Cells[excelrow, 4].Value = rowPart.Code;
                        //                ws.Cells[excelrow, 5].Value = rowPart.GeneralDataReadModel.GetTitle("it", "en");
                        //            }
                        //        }
                        //    }

                        //    ws.Cells[excelrow, 6].Value = row.Quantity;
                        //}

                        //Save the new workbook. We haven't specified the filename so use the Save as method.
                        Log.Information("Exported Work Item {id}", workItem.Id);
                        row++;
                    }
                }
                p.Save();
            }

            System.Diagnostics.Process.Start(newFile.FullName);
            Console.ReadKey();
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
