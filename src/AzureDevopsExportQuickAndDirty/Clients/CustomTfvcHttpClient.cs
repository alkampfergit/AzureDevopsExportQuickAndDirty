using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Serilog;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureDevopsExportQuickAndDirty.Clients
{
    public class CustomTfvcHttpClient : VssHttpClientBase
    {
        public CustomTfvcHttpClient(Uri baseUrl, VssCredentials credentials) : base(baseUrl, credentials)
        {
        }

        public CustomTfvcHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings) : base(baseUrl, credentials, settings)
        {
        }

        public CustomTfvcHttpClient(Uri baseUrl, VssCredentials credentials, params DelegatingHandler[] handlers) : base(baseUrl, credentials, handlers)
        {
        }

        public CustomTfvcHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler) : base(baseUrl, pipeline, disposeHandler)
        {
        }

        public CustomTfvcHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings, params DelegatingHandler[] handlers) : base(baseUrl, credentials, settings, handlers)
        {
        }

        public virtual async Task<TfvcItem> GetAllTeamProjectItemAsync(string teamProject) 
        {
            var url = $"{BaseAddress}/{teamProject}/_apis/tfvc/items?recursionLevel=full&api-version=7.1-preview.1";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var result = await SendAsync<TfvcItem>(request);

            return result;
        }

        public async Task<string> DownloadPathAsZipAsync(string teamProject, string fullPath)
        {
            var path = WebUtility.UrlEncode(fullPath);
            var url = $"{BaseAddress}/{teamProject}/_apis/tfvc/items?path={path}&versionDescriptor%5BversionOptions%5D=0&versionDescriptor%5BversionType%5D=5&versionDescriptor%5Bversion%5D=&%24format=zip&api-version=5.0&download=true";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            base.Client.Timeout = TimeSpan.FromMinutes(20);
            var responseMessage = await base.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (!responseMessage.IsSuccessStatusCode)
            {
                //TODO: Handle the error
            }
            var tempZipFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".zip");

            using (var responseStream = await responseMessage.Content.ReadAsStreamAsync())
            using (var fs = new FileStream(tempZipFile, FileMode.Create, FileAccess.Write))
            {
                await responseStream.CopyToAsync(fs);
            }

            return tempZipFile;
        }
    }
}
