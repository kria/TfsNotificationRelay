using DevCore.Tfs2Slack;
using DevCore.Tfs2Slack.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevCore.Tfs2Slack.Configuration;
using DevCore.Tfs2Slack.Notifications.GitPush;

namespace DevCore.TfsRelay.Slack
{
    public class SlackNotifier : INotifier
    {
        public void Notify(INotification notification, DevCore.Tfs2Slack.Configuration.BotElement bot)
        {
            var channels = bot.GetSetting("slackChannels").Split(',').Select(chan => chan.Trim());

            foreach (string channel in channels)
            {
                var slackMessage = ToSlackMessage((dynamic)notification, bot, channel);
                if (slackMessage != null)
                    SendToSlack(slackMessage, bot.GetSetting("slackWebhookUrl"));
            }
        }

        private async void SendToSlack(Slack.Message message, string webhookUrl)
        {
            try
            {
                using (var slackClient = new SlackClient())
                {
                    var result = await slackClient.SendMessageAsync(message, webhookUrl);
                    result.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public Message ToSlackMessage(INotification notification, BotElement bot, string channel)
        {
            var lines = notification.ToMessage(bot);

            return SlackHelper.CreateSlackMessage(lines, bot, channel, bot.GetSetting("slackColor"));
        }

        public Message ToSlackMessage(BuildCompletionNotification notification, BotElement bot, string channel)
        {
            var lines = notification.ToMessage(bot);
            var color = notification.IsSuccessful ? bot.GetSetting("successColor") : bot.GetSetting("errorColor");

            return SlackHelper.CreateSlackMessage(lines, bot, channel, color);
        }

        public Message ToSlackMessage(WorkItemChangedNotification notification, BotElement bot, string channel)
        {
            string header = notification.ToMessage(bot).First();
            var fields = new[] { 
                new AttachmentField(bot.Text.State, notification.State, true), 
                new AttachmentField(bot.Text.AssignedTo, notification.AssignedTo, true) 
            };

            return SlackHelper.CreateSlackMessage(header, fields, bot, channel, bot.GetSetting("slackColor"));
        }


    }
}
