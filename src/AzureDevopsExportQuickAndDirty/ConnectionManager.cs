using AzureDevopsExportQuickAndDirty.Clients;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Users.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Serilog;
using System;
using System.Threading.Tasks;

namespace AzureDevopsExportQuickAndDirty
{
    public class ConnectionManager
    {
        public ConnectionManager()
        {
        }

        /// <summary>
        /// Perform a connection with an access token, simplest way to give permission to a program
        /// to access your account.
        /// </summary>
        /// <param name="accessToken"></param>
        public async Task<bool> ConnectAsync(String accountUri, String accessToken)
        {
            var connected = await ConnectoToAccountAsync(accountUri, accessToken);
            if (connected)
            {
                InitBaseServices();
            }
            return connected;
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


        private CustomTfvcHttpClient _customTfvcHttpClient;
        public CustomTfvcHttpClient CustomTfvcHttpClient => _customTfvcHttpClient;

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
                _customTfvcHttpClient = _vssConnection.GetClient<CustomTfvcHttpClient>();
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

        private async Task<Boolean> ConnectoToAccountAsync(String accountUri, String accessToken)
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
            try
            {
                await _vssConnection.ConnectAsync();
                
                //Can try to force login to the server.
                //var client = _vssConnection.GetClient<Microsoft.TeamFoundation.Core.WebApi.ProjectHttpClient>();
                //var allprojects = await client.GetProjects();
                return !"anonymous".Equals(_vssConnection.AuthorizedIdentity.DisplayName, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error logging into the account");
            }
            return false;
        }
    }
}
