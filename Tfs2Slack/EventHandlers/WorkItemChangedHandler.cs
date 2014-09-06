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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.TeamFoundation.Framework.Common;

namespace DevCore.Tfs2Slack.EventHandlers
{
    class WorkItemChangedHandler :  IEventHandler
    {
        private static Configuration.SettingsElement settings = Configuration.Tfs2SlackSection.Instance.Settings;
        private static Configuration.TextElement text = Configuration.Tfs2SlackSection.Instance.Text;

        public IList<string> ProcessEvent(TeamFoundationRequestContext requestContext, object notificationEventArgs, Configuration.BotElement bot)
        {
            var ev = (WorkItemChangedEvent)notificationEventArgs;
            if (bot.NotifyOn.HasFlag(TfsEvents.WorkItemStateChange) && ev.ChangedFields.StringFields.Any(f => f.ReferenceName == "System.State") ||
                bot.NotifyOn.HasFlag(TfsEvents.WorkItemAssignmentChange) && ev.ChangedFields.StringFields.Any(f => f.ReferenceName == "System.AssignedTo"))
            {
                var identityService = requestContext.GetService<TeamFoundationIdentityService>();
                var identity = identityService.ReadIdentity(requestContext, IdentitySearchFactor.Identifier, ev.ChangerSid);
                string userName = identity.UniqueName;
                if (settings.StripUserDomain) userName = Utils.StripDomain(userName);

                string msg = text.WorkItemchangedFormat.FormatWith(new
                {
                    UserName = userName,
                    WiUrl = ev.DisplayUrl,
                    WiType = ev.CoreFields.StringFields.Single(f => f.ReferenceName == "System.WorkItemType").NewValue,
                    WiId = ev.CoreFields.IntegerFields.Single(f => f.ReferenceName == "System.Id").NewValue,
                    WiTitle = ev.WorkItemTitle,
                    ProjectName = ev.PortfolioProject
                });

                return new[] { msg };
            }
            else return null;
        }
    }
}
