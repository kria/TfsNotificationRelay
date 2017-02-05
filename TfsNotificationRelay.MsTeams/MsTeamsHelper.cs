using System.Linq;
using System.Text;
using DevCore.TfsNotificationRelay.Configuration;
using DevCore.TfsNotificationRelay.MsTeams.Models;
using DevCore.TfsNotificationRelay.Notifications;

namespace DevCore.TfsNotificationRelay.MsTeams
{
    class MsTeamsHelper
    {
        internal static Message CreateMsTeamsMessage(INotification notification, BotElement bot, string color)
        {
            Message message = null;
            var lines = notification.ToMessage(bot, s => s);

            if (lines?.Any() ?? false)
            {
                var sb = new StringBuilder();
                sb.Append("#####"); // Not using message.Title for now. It's displayed too small, doesn't handle markdown and is not shown at all on Android client.
                sb.AppendLine(lines.First());
                foreach (var line in lines.Skip(1))
                {
                    sb.Append("* ");
                    sb.AppendLine(line);
                }

                message = new Message();
                message.ThemeColor = color?.Substring(1);
                message.Text = sb.ToString();
            }
            return message;
        }
    }
}
