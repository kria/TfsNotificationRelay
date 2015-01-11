using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.Configuration
{
    public interface IKeyedConfigurationElement
    {
        object Key { get; }
    }
}
