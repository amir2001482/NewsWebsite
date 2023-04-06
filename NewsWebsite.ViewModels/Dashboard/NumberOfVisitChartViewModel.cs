using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.ViewModels.Dashboard
{
    public class NumberOfVisitChartViewModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("y")]
        public int Value { get; set; }
    }
}
