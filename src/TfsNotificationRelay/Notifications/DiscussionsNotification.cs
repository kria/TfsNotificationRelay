using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevCore.TfsNotificationRelay.Configuration;

namespace DevCore.TfsNotificationRelay.Notifications
{
    class DiscussionsNotification : BaseNotification
    {
        public override IList<string> ToMessage(BotElement bot, Func<string, string> transform)
        {
            throw new NotImplementedException();
        }

        public override EventRuleElement GetRuleMatch(string collection, IEnumerable<EventRuleElement> eventRules)
        {
            var rule = eventRules.FirstOrDefault(r => collection.IsMatchOrNoPattern(r.TeamProjectCollection));

            return rule;
        }
    }
}
