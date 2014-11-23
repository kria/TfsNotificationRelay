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
using Microsoft.TeamFoundation.Framework.Common;
using System.Text.RegularExpressions;
using DevCore.Tfs2Slack.Notifications;

namespace DevCore.Tfs2Slack.EventHandlers
{
    class WorkItemChangedHandler : BaseHandler<WorkItemChangedEvent>
    {
        protected override INotification CreateNotification(TeamFoundationRequestContext requestContext, WorkItemChangedEvent ev, int maxLines)
        {
            var identityService = requestContext.GetService<TeamFoundationIdentityService>();
            var identity = identityService.ReadIdentity(requestContext, IdentitySearchFactor.Identifier, ev.ChangerSid);
            var notification = new WorkItemChangedNotification()
            {
                TeamProjectCollection = requestContext.ServiceHost.Name,
                IsNew = ev.ChangeType == ChangeTypes.New,
                UniqueName = identity.UniqueName,
                DisplayName = identity.DisplayName,
                WiUrl = ev.DisplayUrl,
                WiType = ev.CoreFields.StringFields.Single(f => f.ReferenceName == "System.WorkItemType").NewValue,
                WiId = ev.CoreFields.IntegerFields.Single(f => f.ReferenceName == "System.Id").NewValue,
                WiTitle = ev.WorkItemTitle,
                ProjectName = ev.PortfolioProject,
                IsStateChanged = ev.ChangedFields.StringFields.Any(f => f.ReferenceName == "System.State"),
                IsAssignmentChanged = ev.ChangedFields.StringFields.Any(f => f.ReferenceName == "System.AssignedTo"),
                State = ev.CoreFields.StringFields.Single(f => f.ReferenceName == "System.State").NewValue,
                AssignedTo = ev.CoreFields.StringFields.Single(f => f.ReferenceName == "System.AssignedTo").NewValue
            };

            return notification;
        }
    }
}
