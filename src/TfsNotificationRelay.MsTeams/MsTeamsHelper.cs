using System.Linq;
using System.Text;
using DevCore.TfsNotificationRelay.Configuration;
using DevCore.TfsNotificationRelay.MsTeams.Models;
using DevCore.TfsNotificationRelay.Notifications;
using System.Collections.Generic;

namespace DevCore.TfsNotificationRelay.MsTeams
{
    class MsTeamsHelper
    {
        internal static Message CreateMsTeamsMessage(INotification notification, BotElement bot)
        {
            var lines = notification.ToMessage(bot, s => s);
            if (lines == null || !lines.Any()) return null;

            var sb = new StringBuilder();
            sb.Append("#####"); // Not using message.Title for now. It's displayed too small, doesn't handle markdown and is not shown at all on Android client.
            sb.AppendLine(lines.First());
            foreach (var line in lines.Skip(1))
            {
                sb.Append("* ");
                sb.AppendLine(line);
            }
            var color = bot.GetSetting("standardColor").Substring(1);
            Message message = new Message() { Text = sb.ToString(), ThemeColor = color };

            return message;
        }

        private static Message CreateMsTeamsMessage(string heading, IEnumerable<Fact> facts, string color)
        {
            var message = new Message() { Text = "#####" + heading, ThemeColor = color };
            message.Sections = new[] { new Section() { Facts = facts } };

            return message;
        }

        internal static Message CreateMsTeamsMessage(BuildCompletionNotification notification, BotElement bot)
        {
            var lines = notification.ToMessage(bot, s => s);
            if (lines == null || !lines.Any()) return null;

            string color = notification.IsPartiallySucceeded ? bot.GetSetting("partiallySucceededColor") : (notification.IsSuccessful ? bot.GetSetting("successColor") : bot.GetSetting("errorColor"));

            return CreateMsTeamsMessage(lines[0], new[] { new Fact(bot.Text.BuildStatus, $"**{lines[1]}**") }, color);
        }

        internal static Message CreateMsTeamsMessage(WorkItemChangedNotification notification, BotElement bot)
        {
            string heading = notification.ToMessage(bot, s => s).First();

            var facts = new List<Fact>();

            var searchType = notification.IsNew ? SearchFieldsType.Core : SearchFieldsType.Changed;
            var displayFieldsKey = notification.IsNew ? "wiCreatedDisplayFields" : "wiChangedDisplayFields";

            foreach (var fieldId in bot.GetCsvSetting(displayFieldsKey, Defaults.WorkItemFields))
            {
                var field = notification.GetUnifiedField(fieldId, searchType);
                if (field != null)
                {
                    var fieldrep = notification.IsNew ? field.NewValue : bot.Text.WorkItemFieldTransitionFormat.FormatWith(field);
                    facts.Add(new Fact(field.Name, fieldrep));
                }
            }

            return CreateMsTeamsMessage(heading, facts, bot.GetSetting("standardColor"));
        }

        internal static Message CreateMsTeamsMessage(ICommentNotification notification, BotElement bot)
        {
            string heading = notification.ToMessage(bot, s => s).First();

            return CreateMsTeamsMessage(heading, new[] { new Fact(bot.Text.Comment, notification.Comment) }, bot.GetSetting("standardColor"));
        }
    }
}
