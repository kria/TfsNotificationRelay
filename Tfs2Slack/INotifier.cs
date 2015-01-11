using DevCore.Tfs2Slack.Configuration;
using DevCore.Tfs2Slack.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack
{
    public interface INotifier
    {
        void Notify(INotification notification, BotElement bot);
    }
}
