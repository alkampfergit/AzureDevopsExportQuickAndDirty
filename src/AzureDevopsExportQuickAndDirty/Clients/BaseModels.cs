using Newtonsoft.Json;

namespace AzureDevopsExportQuickAndDirty.Clients
{
    public class Links
    {
        [JsonProperty("self")]
        public Self Self { get; set; }

        [JsonProperty("web")]
        public Web Web { get; set; }
    }

    public class Self
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }

    public class Web
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }

}
