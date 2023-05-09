﻿using Newtonsoft.Json;

namespace Axendo.Umbraco.Forms.Pipedrive.Core.Models.Responses.LeadResponses
{
    public class LeadField
    {
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } = string.Empty;
    }
}
