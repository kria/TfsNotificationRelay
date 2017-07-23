using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.MsTeams.Models
{
    class Section
    {
        public string Title { get; set; }

        public string ActivityTitle { get; set; }

        public string ActivitySubtitle { get; set; }

        [JsonProperty(PropertyName = "activityImage")]
        public string ActivityImageUrl { get; set; }

        public string ActivityText { get; set; }

        public IEnumerable<Fact> Facts { get; set; }
    }
}
