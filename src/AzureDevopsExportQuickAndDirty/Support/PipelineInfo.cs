using Microsoft.TeamFoundation.Build.WebApi;

namespace AzureDevopsExportQuickAndDirty.Support
{
    internal class PipelineInfo
    {
        public PipelineInfo(
            BuildDefinitionReference reference,
            string repoId,
            int buildCount)
        {
            BuildReference = reference;
            RepoId = repoId;
            BuildCount = buildCount;
        }

        public string RepoId { get; }

        public int BuildCount { get;  }

        public BuildDefinitionReference BuildReference { get; }
    }
}
