﻿using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
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
            ConnectToTfs(accountUri, accessToken);
            InitBaseServices();
        }

        private void InitBaseServices()
        {
            try
            {
                _workItemStore = _tfsCollection.GetService<WorkItemStore>();
                _commonStructureService = _tfsCollection.GetService<ICommonStructureService>();
                _commonStructureService4 = _tfsCollection.GetService<ICommonStructureService4>();

                _workItemTrackingHttpClient = _vssConnection.GetClient<WorkItemTrackingHttpClient>();
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

        /// <summary>
        /// Create an instance where the TFS Project collection was already passed by the 
        /// calleer. 
        /// </summary>
        /// <param name="accessToken"></param>
        public ConnectionManager(TfsTeamProjectCollection tfsTeamProjectCollection) : this()
        {
            _tfsCollection = tfsTeamProjectCollection;
            tfsTeamProjectCollection.Authenticate();
            InitBaseServices();
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

            _tfsCollection = new TfsTeamProjectCollection(uri, creds);
            _tfsCollection.EnsureAuthenticated();
            InitBaseServices();
        }

        private TfsTeamProjectCollection _tfsCollection;
        private VssConnection _vssConnection;
        private WorkItemStore _workItemStore;
        private ICommonStructureService _commonStructureService;
        private ICommonStructureService4 _commonStructureService4;
        private WorkItemTrackingHttpClient _workItemTrackingHttpClient;

        public WorkItemStore WorkItemStore => _workItemStore;
        public ICommonStructureService CommonStructureService => _commonStructureService;
        public ICommonStructureService4 CommonStructureService4 => _commonStructureService4;

        public WorkItemTrackingHttpClient WorkItemTrackingHttpClient => _workItemTrackingHttpClient;

        private Boolean ConnectToTfs(String accountUri, String accessToken)
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

            // Connect to VSTS
            _tfsCollection = new TfsTeamProjectCollection(new Uri(accountUri), creds);
            _tfsCollection.Authenticate();

            _vssConnection = new VssConnection(new Uri(accountUri), creds);
            _vssConnection.ConnectAsync().Wait();
            return true;
        }

        /// <summary>
        /// Returns a list of all team projects names.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<String> GetTeamProjectsNames()
        {
            return _workItemStore.Projects.OfType<Project>().Select(_ => _.Name);
        }

        public Project GetTeamProject(String name)
        {
            return _workItemStore.Projects
                .OfType<Project>()
                .Where(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
        }

        public T GetClient<T>() where T : VssHttpClientBase
        {
            return _vssConnection.GetClient<T>();
        }

        public object GetAuthenticatedUser()
        {
            return _tfsCollection.AuthorizedIdentity.DisplayName;
        }

        public ICredentials GetCredentials()
        {
            return _tfsCollection.Credentials;
        }
    }
}
