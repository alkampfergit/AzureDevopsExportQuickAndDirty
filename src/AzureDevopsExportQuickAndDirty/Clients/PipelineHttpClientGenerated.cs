﻿//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.10.8.0 (NJsonSchema v10.3.11.0 (Newtonsoft.Json v11.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

#pragma warning disable 108 // Disable "CS0108 '{derivedDto}.ToJson()' hides inherited member '{dtoBase}.ToJson()'. Use the new keyword if hiding was intended."
#pragma warning disable 114 // Disable "CS0114 '{derivedDto}.RaisePropertyChanged(String)' hides inherited member 'dtoBase.RaisePropertyChanged(String)'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword."
#pragma warning disable 472 // Disable "CS0472 The result of the expression is always 'false' since a value of type 'Int32' is never equal to 'null' of type 'Int32?'
#pragma warning disable 1573 // Disable "CS1573 Parameter '...' has no matching param tag in the XML comment for ...
#pragma warning disable 1591 // Disable "CS1591 Missing XML comment for publicly visible type or member ..."
#pragma warning disable 8073 // Disable "CS8073 The result of the expression is always 'false' since a value of type 'T' is never equal to 'null' of type 'T?'"

namespace AzureDevopsExportQuickAndDirty.Clients
{
    using System = global::System;



    /// <summary>Artifacts are collections of files produced by a pipeline. Use artifacts to share files between stages in a pipeline or between different pipelines.</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class Artifact
    {
        /// <summary>The name of the artifact.</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>Signed url for downloading this artifact</summary>
        [Newtonsoft.Json.JsonProperty("signedContent", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public SignedUrl SignedContent { get; set; }

        /// <summary>Self-referential url</summary>
        [Newtonsoft.Json.JsonProperty("url", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Url { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class BuildResourceParameters
    {
        [Newtonsoft.Json.JsonProperty("version", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Version { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ContainerResourceParameters
    {
        [Newtonsoft.Json.JsonProperty("version", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Version { get; set; }


    }

    /// <summary>Configuration parameters of the pipeline.</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class CreatePipelineConfigurationParameters
    {
        /// <summary>Type of configuration.</summary>
        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public CreatePipelineConfigurationParametersType? Type { get; set; }


    }

    /// <summary>Parameters to create a pipeline.</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class CreatePipelineParameters
    {
        /// <summary>Configuration parameters of the pipeline.</summary>
        [Newtonsoft.Json.JsonProperty("configuration", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public CreatePipelineConfigurationParameters Configuration { get; set; }

        /// <summary>Folder of the pipeline.</summary>
        [Newtonsoft.Json.JsonProperty("folder", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Folder { get; set; }

        /// <summary>Name of the pipeline.</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }


    }

    /// <summary>Log for a pipeline.</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class LogClass
    {
        /// <summary>The date and time the log was created.</summary>
        [Newtonsoft.Json.JsonProperty("createdOn", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? CreatedOn { get; set; }

        /// <summary>The ID of the log.</summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? Id { get; set; }

        /// <summary>The date and time the log was last changed.</summary>
        [Newtonsoft.Json.JsonProperty("lastChangedOn", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? LastChangedOn { get; set; }

        /// <summary>The number of lines in the log.</summary>
        [Newtonsoft.Json.JsonProperty("lineCount", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long? LineCount { get; set; }

        [Newtonsoft.Json.JsonProperty("signedContent", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public SignedUrl SignedContent { get; set; }

        [Newtonsoft.Json.JsonProperty("url", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Url { get; set; }


    }

    /// <summary>A collection of logs.</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class LogCollection
    {
        /// <summary>The list of logs.</summary>
        [Newtonsoft.Json.JsonProperty("logs", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<LogClass> Logs { get; set; }

        [Newtonsoft.Json.JsonProperty("signedContent", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public SignedUrl SignedContent { get; set; }

        /// <summary>URL of the log.</summary>
        [Newtonsoft.Json.JsonProperty("url", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Url { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class PackageResourceParameters
    {
        [Newtonsoft.Json.JsonProperty("version", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Version { get; set; }


    }

    /// <summary>Definition of a pipeline.</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class Pipeline : PipelineBase
    {
        [Newtonsoft.Json.JsonProperty("_links", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ReferenceLinks _links { get; set; }

        [Newtonsoft.Json.JsonProperty("configuration", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public PipelineConfiguration Configuration { get; set; }

        /// <summary>URL of the pipeline</summary>
        [Newtonsoft.Json.JsonProperty("url", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Url { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class PipelineBase
    {
        /// <summary>Pipeline folder</summary>
        [Newtonsoft.Json.JsonProperty("folder", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Folder { get; set; }

        /// <summary>Pipeline ID</summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? Id { get; set; }

        /// <summary>Pipeline name</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>Revision number</summary>
        [Newtonsoft.Json.JsonProperty("revision", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? Revision { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class PipelineConfiguration
    {
        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public PipelineConfigurationType? Type { get; set; }


    }

    /// <summary>A reference to a Pipeline.</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class PipelineReference : PipelineBase
    {
        [Newtonsoft.Json.JsonProperty("url", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Url { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class PipelineResourceParameters
    {
        [Newtonsoft.Json.JsonProperty("version", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Version { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class PreviewRun
    {
        [Newtonsoft.Json.JsonProperty("finalYaml", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string FinalYaml { get; set; }


    }

    /// <summary>The class to represent a collection of REST reference links.</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ReferenceLinks
    {
        /// <summary>The readonly view of the links.  Because Reference links are readonly, we only want to expose them as read only.</summary>
        [Newtonsoft.Json.JsonProperty("links", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IDictionary<string, object> Links { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class Repository
    {
        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public RepositoryType? Type { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class RepositoryResource
    {
        [Newtonsoft.Json.JsonProperty("refName", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string RefName { get; set; }

        [Newtonsoft.Json.JsonProperty("repository", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Repository Repository { get; set; }

        [Newtonsoft.Json.JsonProperty("version", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Version { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class RepositoryResourceParameters
    {
        [Newtonsoft.Json.JsonProperty("refName", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string RefName { get; set; }

        /// <summary>This is the security token to use when connecting to the repository.</summary>
        [Newtonsoft.Json.JsonProperty("token", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Token { get; set; }

        /// <summary>Optional. This is the type of the token given. If not provided, a type of "Bearer" is assumed. Note: Use "Basic" for a PAT token.</summary>
        [Newtonsoft.Json.JsonProperty("tokenType", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string TokenType { get; set; }

        [Newtonsoft.Json.JsonProperty("version", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Version { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class Run : RunReference
    {
        [Newtonsoft.Json.JsonProperty("_links", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ReferenceLinks _links { get; set; }

        [Newtonsoft.Json.JsonProperty("createdDate", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? CreatedDate { get; set; }

        [Newtonsoft.Json.JsonProperty("finalYaml", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string FinalYaml { get; set; }

        [Newtonsoft.Json.JsonProperty("finishedDate", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? FinishedDate { get; set; }

        [Newtonsoft.Json.JsonProperty("pipeline", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public PipelineReference Pipeline { get; set; }

        [Newtonsoft.Json.JsonProperty("resources", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public RunResources Resources { get; set; }

        [Newtonsoft.Json.JsonProperty("result", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public RunResult? Result { get; set; }

        [Newtonsoft.Json.JsonProperty("state", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public RunState? State { get; set; }

        [Newtonsoft.Json.JsonProperty("url", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Url { get; set; }

        [Newtonsoft.Json.JsonProperty("variables", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IDictionary<string, Variable> Variables { get; set; }


    }

    /// <summary>Settings which influence pipeline runs.</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class RunPipelineParameters
    {
        /// <summary>If true, don't actually create a new run. Instead, return the final YAML document after parsing templates.</summary>
        [Newtonsoft.Json.JsonProperty("previewRun", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? PreviewRun { get; set; }

        /// <summary>The resources the run requires.</summary>
        [Newtonsoft.Json.JsonProperty("resources", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public RunResourcesParameters Resources { get; set; }

        [Newtonsoft.Json.JsonProperty("stagesToSkip", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<string> StagesToSkip { get; set; }

        [Newtonsoft.Json.JsonProperty("templateParameters", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IDictionary<string, string> TemplateParameters { get; set; }

        [Newtonsoft.Json.JsonProperty("variables", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IDictionary<string, Variable> Variables { get; set; }

        /// <summary>If you use the preview run option, you may optionally supply different YAML. This allows you to preview the final YAML document without committing a changed file.</summary>
        [Newtonsoft.Json.JsonProperty("yamlOverride", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string YamlOverride { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class RunReference
    {
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? Id { get; set; }

        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class RunResources
    {
        [Newtonsoft.Json.JsonProperty("repositories", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IDictionary<string, RepositoryResource> Repositories { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class RunResourcesParameters
    {
        [Newtonsoft.Json.JsonProperty("builds", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IDictionary<string, BuildResourceParameters> Builds { get; set; }

        [Newtonsoft.Json.JsonProperty("containers", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IDictionary<string, ContainerResourceParameters> Containers { get; set; }

        [Newtonsoft.Json.JsonProperty("packages", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IDictionary<string, PackageResourceParameters> Packages { get; set; }

        [Newtonsoft.Json.JsonProperty("pipelines", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IDictionary<string, PipelineResourceParameters> Pipelines { get; set; }

        [Newtonsoft.Json.JsonProperty("repositories", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IDictionary<string, RepositoryResourceParameters> Repositories { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SignalRConnection
    {
        [Newtonsoft.Json.JsonProperty("signedContent", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public SignedUrl SignedContent { get; set; }


    }

    /// <summary>A signed url allowing limited-time anonymous access to private resources.</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SignedUrl
    {
        /// <summary>Timestamp when access expires.</summary>
        [Newtonsoft.Json.JsonProperty("signatureExpires", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset? SignatureExpires { get; set; }

        /// <summary>The URL to allow access to.</summary>
        [Newtonsoft.Json.JsonProperty("url", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Url { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class Variable
    {
        [Newtonsoft.Json.JsonProperty("isSecret", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? IsSecret { get; set; }

        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Value { get; set; }


    }

    /// <summary>This class is used to serialized collections as a single JSON object on the wire, to avoid serializing JSON arrays directly to the client, which can be a security hole</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class VssJsonCollectionWrapper : VssJsonCollectionWrapperBase
    {
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Value { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class VssJsonCollectionWrapperBase
    {
        [Newtonsoft.Json.JsonProperty("count", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? Count { get; set; }


    }

    /// <summary>Expand options. Default is None.</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public enum _expand
    {
        [System.Runtime.Serialization.EnumMember(Value = @"none")]
        None = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"signedContent")]
        SignedContent = 1,

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public enum CreatePipelineConfigurationParametersType
    {
        [System.Runtime.Serialization.EnumMember(Value = @"unknown")]
        Unknown = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"yaml")]
        Yaml = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"designerJson")]
        DesignerJson = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"justInTime")]
        JustInTime = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"designerHyphenJson")]
        DesignerHyphenJson = 4,

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public enum PipelineConfigurationType
    {
        [System.Runtime.Serialization.EnumMember(Value = @"unknown")]
        Unknown = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"yaml")]
        Yaml = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"designerJson")]
        DesignerJson = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"justInTime")]
        JustInTime = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"designerHyphenJson")]
        DesignerHyphenJson = 4,

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public enum RepositoryType
    {
        [System.Runtime.Serialization.EnumMember(Value = @"unknown")]
        Unknown = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"gitHub")]
        GitHub = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"azureReposGit")]
        AzureReposGit = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"gitHubEnterprise")]
        GitHubEnterprise = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"azureReposGitHyphenated")]
        AzureReposGitHyphenated = 4,

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public enum RunResult
    {
        [System.Runtime.Serialization.EnumMember(Value = @"unknown")]
        Unknown = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"succeeded")]
        Succeeded = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"failed")]
        Failed = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"canceled")]
        Canceled = 3,

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public enum RunState
    {
        [System.Runtime.Serialization.EnumMember(Value = @"unknown")]
        Unknown = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"inProgress")]
        InProgress = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"canceling")]
        Canceling = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"completed")]
        Completed = 3,

    }

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "13.10.8.0 (NJsonSchema v10.3.11.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class ApiException : System.Exception
    {
        public int StatusCode { get; private set; }

        public string Response { get; private set; }

        public System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> Headers { get; private set; }

        public ApiException(string message, int statusCode, string response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Exception innerException)
            : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + ((response == null) ? "(null)" : response.Substring(0, response.Length >= 512 ? 512 : response.Length)), innerException)
        {
            StatusCode = statusCode;
            Response = response;
            Headers = headers;
        }

        public override string ToString()
        {
            return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "13.10.8.0 (NJsonSchema v10.3.11.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class ApiException<TResult> : ApiException
    {
        public TResult Result { get; private set; }

        public ApiException(string message, int statusCode, string response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, TResult result, System.Exception innerException)
            : base(message, statusCode, response, headers, innerException)
        {
            Result = result;
        }
    }

}

#pragma warning restore 1591
#pragma warning restore 1573
#pragma warning restore 472
#pragma warning restore 114
#pragma warning restore 108