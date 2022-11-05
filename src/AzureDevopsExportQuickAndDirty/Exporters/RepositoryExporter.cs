using AzureDevopsExportQuickAndDirty.Exporters.Models;
using AzureDevopsExportQuickAndDirty.Support;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureDevopsExportQuickAndDirty.Exporters
{
    internal class RepositoryExporter
    {
        private readonly ConnectionManager _connection;

        public RepositoryExporter(ConnectionManager connection)
        {
            _connection = connection;
        }

        public async Task<IReadOnlyCollection<RepositoryInformations>> ExtractSourceCodeInformation(
            OfficeOpenXml.ExcelPackage excel,
            string teamProject,
            IReadOnlyCollection<PipelineInformations> pipelineInfo)
        {
            List<RepositoryInformations> result = new List<RepositoryInformations>();
            var ws = excel.Workbook.Worksheets.Single(w => w.Name == "Source");
            ws.Cells["A1"].Value = "Id";
            ws.Cells["B1"].Value = "Type";
            ws.Cells["C1"].Value = "Name";
            ws.Cells["D1"].Value = "Commit/changeset";
            ws.Cells["E1"].Value = "Branches";
            ws.Cells["F1"].Value = "Files in main branch";
            ws.Cells["G1"].Value = "Pipelines";

            //TODO: Remove in a specific class.
            await DumpTfVcInformation(teamProject, ws);

            int row = 3;

            var repositories = await _connection.GitHttpClient.GetRepositoriesAsync(
                project: teamProject
            );
            Log.Information("Get information about {count} git repositories", repositories.Count);
            foreach (var repo in repositories)
            {
                var info = new RepositoryInformations()
                {
                    Id = repo.Id,
                    Type = "Git",
                    Name = repo.Name,
                };
                result.Add(info);
                ws.Cells[$"A{row}"].Value = repo.Id;
                ws.Cells[$"B{row}"].Value = "Git";
                ws.Cells[$"C{row}"].Value = repo.Name;
                ws.Cells[$"G{row}"].Value = info.PipelineCount = pipelineInfo.Count(p => p.RepositoryId == repo.Id.ToString());

                //retrieve commits
                //var allCommits = new Dictionary<string, GitCommitRef>(1000);
                //List<GitCommitRef> pageOfCommits;
                //int page = 0;
                //int pageSize = 100;
                //GitQueryCommitsCriteria criteria = new GitQueryCommitsCriteria()
                //{
                //    Skip = 0,
                //    Top = pageSize
                //};
                //do
                //{
                //    pageOfCommits = await _connection.GitHttpClient.GetCommitsAsync(repo.Id, criteria);
                //    foreach (var commit in pageOfCommits)
                //    {
                //        allCommits[commit.CommitId] = commit;
                //    }

                //    Log.Information("Loaded block of {count} commits for repo {repo} running total {rt}", pageOfCommits.Count, repo.Name, allCommits.Count);
                //    page++;
                //    criteria.Skip = page * pageSize;
                //} while (pageOfCommits.Count > 0 && allCommits.Count < 10000);

                FillInformationWithClone(ws, row, repo, info);

                Log.Information("Get details for repo {repo}", repo.Name);
                var branches = await _connection.GitHttpClient.GetBranchesAsync(repo.Id);

                ws.Cells[$"E{row}"].Value = info.BranchesCount = branches.Count;
                row++;
            }

            return result;
        }

        private async Task DumpTfVcInformation(string teamProject, OfficeOpenXml.ExcelWorksheet ws)
        {
            List<TfvcChangesetRef> allChangesets = new List<TfvcChangesetRef>(1000);
            List<TfvcChangesetRef> block;
            var searchCriteria = new TfvcChangesetSearchCriteria();
            searchCriteria.ItemPath = $"$/{teamProject}";
            block = await _connection.TfvcHttpClient.GetChangesetsAsync(searchCriteria: searchCriteria);

            while (block.Count > 0)
            {
                Log.Information("Retrieved a block of TFVC changeset of size {size} - latest {latest}", block.Count, block[block.Count - 1].ChangesetId);
                allChangesets.AddRange(block);
                searchCriteria.ToId = block[block.Count - 1].ChangesetId - 1;

                //search again
                block = await _connection.TfvcHttpClient.GetChangesetsAsync(searchCriteria: searchCriteria);
            };

            ws.Cells["A2"].Value = "TFVC";
            ws.Cells["B2"].Value = "TFVC";
            ws.Cells["C2"].Value = "TFVC";
            ws.Cells["D2"].Value = allChangesets.Count;
        }

        private static void FillInformationWithClone(OfficeOpenXml.ExcelWorksheet ws, int row, GitRepository repo, RepositoryInformations info)
        {
            try
            {
                var cloneTempFolder = Path.GetTempPath() + Guid.NewGuid().ToString();
                Directory.CreateDirectory(cloneTempFolder);

                Log.Information("Starting to clone git repository {repo} into {folder}", repo.Name, cloneTempFolder);
                var gitCommandResult = ExecuteGitCommand(Path.GetTempPath(), $"clone {repo.RemoteUrl} {cloneTempFolder}");
                Log.Debug("Cloned git repository {repo} into {folder}", repo.Name, cloneTempFolder);

                gitCommandResult = ExecuteGitCommand(cloneTempFolder, "rev-list --all --count");

                int commitCount = int.Parse(gitCommandResult.TrimCommandResponse());
                ws.Cells[$"D{row}"].Value = info.CommitCount = commitCount;
                int fileCount = Directory.GetFiles(cloneTempFolder, "*.*", SearchOption.AllDirectories).Length;
                ws.Cells[$"F{row}"].Value = info.FileCount = fileCount;

                FileSystemUtils.SafeDelete(cloneTempFolder);
                Log.Debug("Deleted Cloned git repository {folder}", cloneTempFolder);
                Log.Information("Get detailed information for repo {repo} commit {commit} files {files}", repo.Name, commitCount, fileCount);
            }
            catch (Exception ex)
            {
                //error getting information
                Log.Error(ex, "Error cloning repository {repo}", repo.RemoteUrl);
            }
        }


        private static string ExecuteGitCommand(string workingFolder, string arguments)
        {
            var pi = new ProcessStartInfo()
            {
                UseShellExecute = false,
                WorkingDirectory = workingFolder,
                FileName = "git",
                Arguments = arguments,
                RedirectStandardOutput = true,
                //RedirectStandardError = true,
                //RedirectStandardInput = true
            };
            var process = Process.Start(pi);
            var output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();
            return output;
        }

    }
}
