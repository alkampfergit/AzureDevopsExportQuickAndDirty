using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevopsExportQuickAndDirty.Exporters.Models
{
    public class WorkItemInformations
    {
        public WorkItem WorkItem { get; set; }

        public int NumOfRelations { get; set; }

        public int NumOfCodeRelations { get; set; }

        public int NumOfPullRequests { get; set; }
    }
}
