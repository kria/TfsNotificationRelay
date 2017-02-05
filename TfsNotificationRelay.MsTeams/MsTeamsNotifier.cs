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
        public virtual Task NotifyAsync(TeamFoundationRequestContext requestContext, INotification notification, BotElement bot, EventRuleElement matchingRule)
        {
            string webHookUrl = bot.GetSetting("webhookUrl");

            var msTeamsClient = new MsTeamsClient(webHookUrl);

            Message message = ToMsTeamsMessage((dynamic)notification, bot);

            return msTeamsClient.SendWebhookMessageAsync(message).ContinueWith(t => t.Result.EnsureSuccessStatusCode());
        }


        private Message ToMsTeamsMessage(INotification notification, BotElement bot)
        {
            return MsTeamsHelper.CreateMsTeamsMessage(notification, bot, bot.GetSetting("standardColor"));
        }

        private Message ToMsTeamsMessage(BuildCompletionNotification notification, BotElement bot)
        {
            string color = notification.IsPartiallySucceeded ? bot.GetSetting("partiallySucceededColor") : (notification.IsSuccessful ? bot.GetSetting("successColor") : bot.GetSetting("errorColor"));

            return MsTeamsHelper.CreateMsTeamsMessage(notification, bot, color);
        }

    }
}
