/*
 * Tfs2Slack - http://github.com/kria/Tfs2Slack
 * 
 * Copyright (C) 2014 Kristian Adrup
 * 
 * This file is part of Tfs2Slack.
 * 
 * Tfs2Slack is free software: you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the
 * Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version. See included file COPYING for details.
 */

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.EventHandlers
{
    class ProjectDeletedHandler : BaseHandler
    {
        protected override IList<string> _ProcessEvent(TeamFoundationRequestContext requestContext, object notificationEventArgs, Configuration.BotElement bot)
        {
            var ev = (ProjectDeletedEvent)notificationEventArgs;
            if (!IsNotificationMatch(bot)) return null;
            return new[] { text.ProjectDeletedFormat.FormatWith(new { ProjectUri = ev.Uri }) };
        }

        public bool IsNotificationMatch(Configuration.BotElement bot)
        {
            var rule = bot.EventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.ProjectDeleted));

            if (rule != null) return rule.Notify;

            return false;
        }
    }
}
