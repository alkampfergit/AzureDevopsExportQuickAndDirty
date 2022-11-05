using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevopsExportQuickAndDirty.Exporters.Models
{
    internal class RepositoryInformations
    {
        public Guid Id { get; internal set; }
        public string Type { get; internal set; }
        public string Name { get; internal set; }
        public int PipelineCount { get; internal set; }
        public int BranchesCount { get; internal set; }
        public int CommitCount { get; internal set; }
        public int FileCount { get; internal set; }
    }
}
