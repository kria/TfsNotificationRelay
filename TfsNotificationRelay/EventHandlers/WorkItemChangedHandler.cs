/*
 * TfsNotificationRelay - http://github.com/kria/TfsNotificationRelay
 * 
 * Copyright (C) 2014 Kristian Adrup
 * 
 * This file is part of TfsNotificationRelay.
 * 
 * TfsNotificationRelay is free software: you can redistribute it and/or 
 * modify it under the terms of the GNU General Public License as published 
 * by the Free Software Foundation, either version 3 of the License, or 
 * (at your option) any later version. See included file COPYING for details.
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
using DevCore.TfsNotificationRelay.Notifications;

namespace DevCore.TfsNotificationRelay.EventHandlers
{
    class WorkItemChangedHandler : BaseHandler<WorkItemChangedEvent>
    {
        protected override IEnumerable<INotification> CreateNotifications(TeamFoundationRequestContext requestContext, WorkItemChangedEvent ev, int maxLines)
        {
            var notifications = new List<INotification>();

            var identityService = requestContext.GetService<TeamFoundationIdentityService>();
            var identity = identityService.ReadIdentity(requestContext, IdentitySearchFactor.Identifier, ev.ChangerSid);

            if (ev.CoreFields == null) throw new TfsNotificationRelayException("ev.CoreFields is null");
            if (ev.CoreFields.StringFields == null) throw new TfsNotificationRelayException("ev.CoreFields.StringFields is null");
            if (ev.CoreFields.IntegerFields == null) throw new TfsNotificationRelayException("ev.CoreFields.IntegerFields is null");

            var typeField = ev.CoreFields.StringFields.SingleOrDefault(f => f.ReferenceName == "System.WorkItemType");
            if (typeField == null) throw new TfsNotificationRelayException("missing System.WorkItemType");
            string type = typeField.NewValue;

            var idField = ev.CoreFields.IntegerFields.Single(f => f.ReferenceName == "System.Id");
            if (idField == null) throw new TfsNotificationRelayException("missing System.Id");
            int id = idField.NewValue;

            var teamNames = GetUserTeamsByProjectUri(requestContext, ev.ProjectNodeId, identity.Descriptor);

            if (ev.TextFields != null)
            {
                var comment = ev.TextFields.FirstOrDefault(f => f.ReferenceName == "System.History" && !String.IsNullOrEmpty(f.Value));
                if (comment != null)
                {
                    var commentNotification = new WorkItemCommentNotification()
                    {
                        TeamProjectCollection = requestContext.ServiceHost.Name,
                        UniqueName = identity.UniqueName,
                        DisplayName = identity.DisplayName,
                        WiUrl = ev.DisplayUrl,
                        WiType = type,
                        WiId = id,
                        WiTitle = ev.WorkItemTitle,
                        ProjectName = ev.PortfolioProject,
                        AreaPath = ev.AreaPath,
                        CommentHtml = comment.Value,
                        Comment = TextHelper.HtmlToText(comment.Value),
                        TeamNames = teamNames
                    };

                    notifications.Add(commentNotification);
                }
            }

            var changeNotification = new WorkItemChangedNotification()
            {
                TeamProjectCollection = requestContext.ServiceHost.Name,
                IsNew = ev.ChangeType == ChangeTypes.New,
                UniqueName = identity.UniqueName,
                DisplayName = identity.DisplayName,
                WiUrl = ev.DisplayUrl,
                WiType = type,
                WiId = id,
                WiTitle = ev.WorkItemTitle,
                ProjectName = ev.PortfolioProject,
                AreaPath = ev.AreaPath,
                IsStateChanged = ev.ChangedFields != null && ev.ChangedFields.StringFields != null && ev.ChangedFields.StringFields.Any(f => f.ReferenceName == "System.State"),
                IsAssignmentChanged = ev.ChangedFields != null && ev.ChangedFields.StringFields != null && ev.ChangedFields.StringFields.Any(f => f.ReferenceName == "System.AssignedTo"),
                State = ev.CoreFields.StringFields.GetFieldValue("System.State", f => f.NewValue),
                AssignedTo = ev.CoreFields.StringFields.GetFieldValue("System.AssignedTo", f => f.NewValue),
                CoreFields = ev.CoreFields,
                ChangedFields = ev.ChangedFields,
                TeamNames = teamNames
            };
            notifications.Add(changeNotification);

            return notifications;
        }

    }

    static class WorkItemExtensions
    {
        public static string GetFieldValue(this StringField[] fields, string key, Func<StringField, string> selector)
        {
            var field = fields.SingleOrDefault(f => f.ReferenceName == key);
            return field != null ? selector(field) : null;
        }
    }

    
}
