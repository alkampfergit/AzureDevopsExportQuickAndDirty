using AzureDevopsExportQuickAndDirty.Clients;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevopsExportQuickAndDirty
{
    public class ConnectionManager
    {
        public static ConnectionManager Instance { get; private set; }

        public ConnectionManager()
        {
            Instance = this;
        }

        /// <summary>
        /// Perform a connection with an access token, simplest way to give permission to a program
        /// to access your account.
        /// </summary>
        /// <param name="accessToken"></param>
        public ConnectionManager(String accountUri, String accessToken) : this()
        {
            connectoToAccount(accountUri, accessToken);
            InitBaseServices();
        }

        private VssConnection _vssConnection;

        private WorkItemTrackingHttpClient _workItemTrackingHttpClient;
        public WorkItemTrackingHttpClient WorkItemTrackingHttpClient => _workItemTrackingHttpClient;

        private BuildHttpClient _buildHttpClient;
        public BuildHttpClient BuildHttpClient => _buildHttpClient;

        private PipelineHttpClient _pipelineHttpClient;
        public PipelineHttpClient PipelineHttpClient => _pipelineHttpClient;

        private TfvcHttpClient _tfvcHttpClient;
        public TfvcHttpClient TfvcHttpClient => _tfvcHttpClient;

        private GitHttpClient _gitHttpClient;
        public GitHttpClient GitHttpClient => _gitHttpClient;

        private void InitBaseServices()
        {
            try
            {
                _buildHttpClient = _vssConnection.GetClient<BuildHttpClient>(); 
                 _workItemTrackingHttpClient = _vssConnection.GetClient<WorkItemTrackingHttpClient>();
                 _pipelineHttpClient = _vssConnection.GetClient<PipelineHttpClient>();
                 _tfvcHttpClient = _vssConnection.GetClient<TfvcHttpClient>();
                _gitHttpClient = _vssConnection.GetClient<GitHttpClient>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error trying to connect to the service {0}", ex.Message);
                Exception innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Log.Error(innerEx, "inner exception connecting to the service {0}", innerEx);
                    innerEx = innerEx.InnerException;
                }
                throw;
            }
        }

        public async Task ConnectAsync(string accountUri)
        {
            Uri uri = new Uri(accountUri);

            var creds = new VssClientCredentials(
                new Microsoft.VisualStudio.Services.Common.WindowsCredential(false),
                new VssFederatedCredential(true),
                CredentialPromptType.PromptIfNeeded);

            _vssConnection = new VssConnection(uri, creds);
            await _vssConnection.ConnectAsync().ConfigureAwait(false);

            InitBaseServices();
        }

        private Boolean connectoToAccount(String accountUri, String accessToken)
        {
            //login for VSTS
            VssCredentials creds;
            if (String.IsNullOrEmpty(accessToken))
            {
                creds = new VssClientCredentials();
            }
            else
            {
                creds = new VssBasicCredential(
                   String.Empty,
                   accessToken);
            }
            creds.Storage = new VssClientCredentialStorage();

            _vssConnection = new VssConnection(new Uri(accountUri), creds);
            _vssConnection.ConnectAsync().Wait();
            return true;
        }


        public T GetClient<T>() where T : VssHttpClientBase
        {
            return _vssConnection.GetClient<T>();
        }
    }
}
