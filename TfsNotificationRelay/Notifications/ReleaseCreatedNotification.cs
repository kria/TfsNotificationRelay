/*
 * TfsNotificationRelay - http://github.com/kria/TfsNotificationRelay
 * 
 * Copyright (C) 2016 Kristian Adrup
 * 
 * This file is part of TfsNotificationRelay.
 * 
 * TfsNotificationRelay is free software: you can redistribute it and/or 
 * modify it under the terms of the GNU General Public License as published 
 * by the Free Software Foundation, either version 3 of the License, or 
 * (at your option) any later version. See included file COPYING for details.
 */

using DevCore.TfsNotificationRelay.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevCore.TfsNotificationRelay.Notifications
{
    class ReleaseCreatedNotification : ReleaseNotification
    {
        public override IList<string> ToMessage(BotElement bot, TextElement text, Func<string, string> transform)
        {
            var formatter = new
            {
                TeamProjectCollection = transform(TeamProjectCollection),
                ProjectName = transform(ProjectName),
                ReleaseDefinition = transform(ReleaseDefinition),
                ReleaseStatus = transform(ReleaseStatus.ToString()),
                ReleaseUrl,
                ReleaseName = transform(ReleaseName),
                ReleaseReason = transform(ReleaseReason.ToString()),
                CreatedBy = transform(CreatedByUniqueName),
                CreatedByDisplayName = transform(CreatedByDisplayName),
                DisplayName = transform(CreatedByDisplayName),
                CreatedOn,
                UserName = transform(UserName),
                MappedUser = bot.GetMappedUser(CreatedByUniqueName)
            };
            return new[] { text.ReleaseCreatedFormat.FormatWith(formatter) };
        }

        public override EventRuleElement GetRuleMatch(string collection, IEnumerable<EventRuleElement> eventRules)
        {
            var rule = GetRulesMatch(collection, eventRules).FirstOrDefault(r => r.Events.HasFlag(TfsEvents.ReleaseCreated));

            return rule;
        }
    }
}
