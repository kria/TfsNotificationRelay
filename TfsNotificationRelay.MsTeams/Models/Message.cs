using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.MsTeams.Models
{
    class Message
    {
        public string Summary { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public string ThemeColor { get; set; }

        public IEnumerable<Section> Sections { get; set; }
    }
}
