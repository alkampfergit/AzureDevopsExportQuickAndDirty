using AzureDevopsExportQuickAndDirty.Exporters;
using AzureDevopsExportQuickAndDirty.Support;
using CommandLine;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using OfficeOpenXml;
using Serilog;
using Serilog.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureDevopsExportQuickAndDirty
{
    public static class Program
    {
        private static Options _options;

        static async Task Main(string[] args)
        {
            ConfigureSerilog();

            var result = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => _options = opts)
                .WithNotParsed((errs) => HandleParseError(errs));

            if (result.Errors.Any())
            {
                ConsoleUtils.ErrorAndExit("Error parsing arguments");
                return;
            }

            ConnectionManager connection = new ConnectionManager();
            var connected = await connection.ConnectAsync(_options.ServiceAddress, _options.AccessToken);

            if (!connected)
            {
                ConsoleUtils.ErrorAndExit("Login failed");
                return;
            }

            FileInfo newFile = GetExcelTemplateFileName();
            var workItemExporter = new WorkItemExporter(connection);
            var pipelineExporter = new PipelineExporter(connection);
            var repositoryExporter = new RepositoryExporter(connection);
            using (var excel = new ExcelPackage(newFile))
            {
                Log.Information("Created temporary excel file {file}", newFile);

                //await workItemExporter.ExtractAllWorkItemsInfo(excel, _options.TeamProject);

                var pipelineInfo = await pipelineExporter.ExtractPipelineInformations(excel, _options.TeamProject);

                await repositoryExporter.ExtractSourceCodeInformation(excel, _options.TeamProject, pipelineInfo);

                excel.Save();
            }

            Process.Start(newFile.FullName);
        }

        private static FileInfo GetExcelTemplateFileName()
        {
            var outDirectory = _options.GetOuputFolder();
            string fileName;
            int index = 0;
            do
            {
                fileName = Path.Combine(
                    outDirectory, 
                    _options.TeamProject.SanitizeForFileSystem() +
                    (index > 0 ? $" ({index})" : String.Empty)
                    + ".xlsx");
                index++;
            } while (File.Exists(fileName));
            var template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "BaseTemplate.xlsx");
            File.Copy(template, fileName);
            return new FileInfo(fileName);
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach (var parseError in errs)
            {
                Log.Error("Error parsing arguments: {error}", parseError.Tag);
            }
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
