using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.MsTeams.Models
{
    class Image
    {
        public string Title { get; set; }

        [JsonProperty(PropertyName = "image")]
        public string ImageUrl { get; set; }
    }
}
