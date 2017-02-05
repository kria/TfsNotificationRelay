using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevCore.TfsNotificationRelay.Configuration;
using DevCore.TfsNotificationRelay.MsTeams.Models;
using DevCore.TfsNotificationRelay.Notifications;
using Microsoft.TeamFoundation.Framework.Server;

namespace DevCore.TfsNotificationRelay.MsTeams
{
    public class MsTeamsNotifier : INotifier
    {
        public virtual Task NotifyAsync(IVssRequestContext requestContext, INotification notification, BotElement bot, EventRuleElement matchingRule)
        {
            string webHookUrl = bot.GetSetting("webhookUrl");

            var msTeamsClient = new MsTeamsClient(webHookUrl);

            Message message = MsTeamsHelper.CreateMsTeamsMessage((dynamic)notification, bot);

            return msTeamsClient.SendWebhookMessageAsync(message).ContinueWith(t => t.Result.EnsureSuccessStatusCode());
        }
    }
}
