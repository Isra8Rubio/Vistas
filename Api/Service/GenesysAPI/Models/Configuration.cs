using PureCloudPlatform.Client.V2.Client;

namespace Api.Service.GenesysAPI.Models
{
    public class Configuration
    {
        public PureCloudRegionHosts GenesysRegion { get; set; }
        public string GenesysClient { get; set; } = "";
        public string GenesysSecret { get; set; } = "";
    }
}
