using Microsoft.TeamFoundation.Build.WebApi;
using System.IO;

namespace AzureDevopsExportQuickAndDirty.Exporters.Models
{
    public class PipelineInformations
    {
        public PipelineInformations(BuildDefinitionReference pipeline)
        {
            Id = pipeline.Id;
            Name = pipeline.Name;
            Url = pipeline.Url;
        }

        public int Id { get; }
        public string Name { get; }
        public string Url { get; }
        public string Path { get; internal set; }
        public string LastGoodResult { get; internal set; }
        public string RepositoryName { get; internal set; }
        public string RepositoryId { get; internal set; }
        public int ActiveBuildCount { get; internal set; }
    }
}
