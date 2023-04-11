
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NewsWebsite.ViewModels.Dashboard
{
    public class NumberOfVisitChartViewModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("y")]
        public int Value { get; set; }
    }
}
