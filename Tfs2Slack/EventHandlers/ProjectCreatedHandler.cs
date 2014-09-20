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

using DevCore.Tfs2Slack.Configuration;
using DevCore.Tfs2Slack.Notifications;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.EventHandlers
{
    class ProjectCreatedHandler : BaseHandler
    {
        public override Type[] SubscribedTypes()
        {
            return new Type[] { typeof(ProjectCreatedEvent) };
        }

        protected override INotification CreateNotification(TeamFoundationRequestContext requestContext, object notificationEventArgs, int maxLines)
        {
            var ev = (ProjectCreatedEvent)notificationEventArgs;
            var locationService = requestContext.GetService<TeamFoundationLocationService>();

            string projectUrl = String.Format("{0}/{1}/{2}",
                locationService.GetAccessMapping(requestContext, "PublicAccessMapping").AccessPoint,
                requestContext.ServiceHost.Name,
                ev.Name);

            if (!this.ProjectsNames.ContainsKey(ev.Uri))
                this.ProjectsNames.Add(ev.Uri, ev.Name);

            return new ProjectCreatedNotification() { TeamProjectCollection = requestContext.ServiceHost.Name, ProjectUrl = projectUrl, ProjectName = ev.Name };
        }
    }
}
