using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureDevopsExportQuickAndDirty.Clients
{
    public class PipelineHttpClient : VssHttpClientBase
    {
        public PipelineHttpClient(Uri baseUrl, VssCredentials credentials) : base(baseUrl, credentials)
        {
        }

        public PipelineHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings) : base(baseUrl, credentials, settings)
        {
        }

        public PipelineHttpClient(Uri baseUrl, VssCredentials credentials, params DelegatingHandler[] handlers) : base(baseUrl, credentials, handlers)
        {
        }

        public PipelineHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler) : base(baseUrl, pipeline, disposeHandler)
        {
        }

        public PipelineHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings, params DelegatingHandler[] handlers) : base(baseUrl, credentials, settings, handlers)
        {
        }

        public async Task<PipelineListCollectionWrapper> ListAsync(string teamProject)
        {
            var url = $"{BaseAddress}/{teamProject}/_apis/pipelines?api-version=7.1-preview.1";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var responseMessage = await SendAsync(request);
            if (!responseMessage.IsSuccessStatusCode)
            {
                //TODO: Handle the error
            }

            var response = await responseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PipelineListCollectionWrapper>(response);
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.11.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class PipelineListCollectionWrapper : VssJsonCollectionWrapperBase
    {
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public CustomPipelineReference[] Value { get; set; }
    }

    public class CustomPipelineReference : PipelineReference 
    {
        [Newtonsoft.Json.JsonProperty("_links", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Links _links { get; set; }
    }
}
