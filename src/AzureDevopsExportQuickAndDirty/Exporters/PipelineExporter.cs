using AzureDevopsExportQuickAndDirty.Support;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Common.CommandLine;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Serilog;
using AzureDevopsExportQuickAndDirty.Exporters.Models;

namespace AzureDevopsExportQuickAndDirty.Exporters
{
    public class PipelineExporter
    {
        private readonly ConnectionManager _connection;

        public PipelineExporter(ConnectionManager connection)
        {
            _connection = connection;
        }

        public async Task<IReadOnlyCollection<PipelineInformations>> ExtractPipelineInformations(
            ExcelPackage excel,
            string teamProject)
        {
            List<PipelineInformations> pipelineInformations = new List<PipelineInformations>(100);
            var order = DefinitionQueryOrder.LastModifiedDescending;

            //now we need to export all data in excel file.
            var ws = excel.Workbook.Worksheets.Single(w => w.Name == "Pipelines");
            ws.Cells["A1"].Value = "Id";
            ws.Cells["B1"].Value = "Name";
            ws.Cells["C1"].Value = "Folder";
            ws.Cells["D1"].Value = "Url";
            ws.Cells["E1"].Value = "LatestSuccessfulBuild";
            ws.Cells["F1"].Value = "Repository";
            ws.Cells["G1"].Value = "Repository Id";
            ws.Cells["H1"].Value = "TotalRuns";

            int row = 2;

            var builds = await _connection.BuildHttpClient.GetDefinitionsAsync2(project: teamProject, queryOrder: order);
            Log.Information("Found {count} pipelines", builds.Count);

            while (builds.Count > 0)
            {
                foreach (var pipeline in builds)
                {
                    PipelineInformations info = new PipelineInformations(pipeline);
                    pipelineInformations.Add(info);
                    Log.Information("Getting information details for pipeline {pipeline}", pipeline.Name);
                    var details = await _connection.BuildHttpClient.GetDefinitionAsync(teamProject, pipeline.Id);

                    var buildResults = await _connection.BuildHttpClient.GetBuildsAsync2(
                        project: teamProject,
                        definitions: new[] { pipeline.Id });

                    ws.Cells[$"A{row}"].Value = pipeline.Id;
                    ws.Cells[$"B{row}"].Value = pipeline.Name;
                    ws.Cells[$"C{row}"].Value = details.Path;
                    ws.Cells[$"D{row}"].Value = pipeline.Url;

                    var latestGoodResult = buildResults
                        .Where(br => br.Status == BuildStatus.Completed
                        && br.Result == BuildResult.Succeeded)
                        .OrderByDescending(r => r.FinishTime)
                        .FirstOrDefault();

                    ws.Cells[$"E{row}"].Value = info.LastGoodResult = latestGoodResult?.FinishTime?.ToString("yyyy/MM/dd");
                    ws.Cells[$"F{row}"].Value = info.RepositoryName =  details.Repository.Name;
                    ws.Cells[$"G{row}"].Value = info.RepositoryId = details.Repository.Id;

                    var stats = await _connection.BuildHttpClient.GetBuildsAsync2(
                        project: teamProject,
                        definitions: new[] { details.Id });

                    ws.Cells[$"H{row}"].Value =  info.ActiveBuildCount = stats.Count;

                    row++;
                }

                if (!String.IsNullOrEmpty(builds.ContinuationToken))
                {
                    builds = await _connection.BuildHttpClient.GetDefinitionsAsync2(
                        project: teamProject,
                        queryOrder: order,
                        continuationToken: builds.ContinuationToken);
                }
                else
                {
                    break; //finished cycle
                }
            }

            return pipelineInformations;
        }
    }
}
