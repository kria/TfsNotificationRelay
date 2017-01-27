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
                message = new Message();
                message.themeColor = color?.Substring(1);
                message.Text = lines.First();
                if (!string.IsNullOrWhiteSpace(color))
                {
                    message.Title = $@"<h1 style=""color: {color}""> {string.Join(System.Environment.NewLine, lines.Skip(1))}</h1>";

                } else
                {
                    message.Title = $@"{string.Join($"{System.Environment.NewLine}", string.Join("#", lines.Skip(1)))}"; ;
                }
            }
            return message;
        }
    }
}
