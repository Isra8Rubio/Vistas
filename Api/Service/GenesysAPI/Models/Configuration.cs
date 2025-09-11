using PureCloudPlatform.Client.V2.Client;

namespace Api.Service.GenesysAPI.Models
{
    public class Configuration
    {
        public PureCloudRegionHosts GenesysRegion { get; set; } = PureCloudRegionHosts.us_east_1;

        public string GenesysClient { get; set; } = string.Empty;
        public string GenesysSecret { get; set; } = string.Empty;

        // Propiedad auxiliar para configurar la región desde string
        public string GenesysRegionString
        {
            get => GenesysRegion.ToString();
            set
            {
                // Convertir string a enum
                GenesysRegion = value.ToLowerInvariant() switch
                {
                    "us_east_1" or "mypurecloud.com" => PureCloudRegionHosts.us_east_1,
                    "us_east_2" or "use2.pure.cloud" => PureCloudRegionHosts.us_east_2,
                    "us_west_2" or "usw2.pure.cloud" => PureCloudRegionHosts.us_west_2,
                    "eu_west_1" or "mypurecloud.ie" => PureCloudRegionHosts.eu_west_1,
                    "eu_west_2" or "euw2.pure.cloud" => PureCloudRegionHosts.eu_west_2,
                    "eu_central_1" or "mypurecloud.de" => PureCloudRegionHosts.eu_central_1,
                    "ap_southeast_2" or "mypurecloud.com.au" => PureCloudRegionHosts.ap_southeast_2,
                    "ap_northeast_1" or "mypurecloud.jp" => PureCloudRegionHosts.ap_northeast_1,
                    "ca_central_1" or "cac1.pure.cloud" => PureCloudRegionHosts.ca_central_1,
                    "ap_northeast_2" or "apne2.pure.cloud" => PureCloudRegionHosts.ap_northeast_2,
                    _ => PureCloudRegionHosts.us_east_1 // Default
                };
            }
        }
    }
}