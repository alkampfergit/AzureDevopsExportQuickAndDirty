using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevopsExportQuickAndDirty.Models
{
    public class Self
    {
        public string href { get; set; }
    }

    public class Web
    {
        public string href { get; set; }
    }

    public class Links
    {
        public Self self { get; set; }
        public Web web { get; set; }
    }

    public class PipelineListItem
    {
        public Links _links { get; set; }
        public string url { get; set; }
        public int id { get; set; }
        public int revision { get; set; }
        public string name { get; set; }
        public string folder { get; set; }
    }

    public class PipelineListResult
    {
        public int count { get; set; }
        public IList<PipelineListItem> value { get; set; }
    }
}
