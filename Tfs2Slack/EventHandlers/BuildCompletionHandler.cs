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
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DevCore.Tfs2Slack.Notifications;
using Newtonsoft.Json.Linq;

namespace DevCore.Tfs2Slack.EventHandlers
{
    class BuildCompletionHandler : BaseHandler
    {
        public override Type[] SubscribedTypes()
        {
            return new Type[] { typeof(BuildCompletionNotificationEvent) };
        }

        protected override INotification CreateNotification(TeamFoundationRequestContext requestContext, object notificationEventArgs, int maxLines)
        {
            var buildNotification = (BuildCompletionNotificationEvent)notificationEventArgs;
            BuildDetail build = buildNotification.Build;
            var locationService = requestContext.GetService<TeamFoundationLocationService>();
            var buildService = requestContext.GetService<TeamFoundationBuildService>();

            using (var buildReader = buildService.QueryQueuedBuildsById(requestContext, build.QueueIds, new[] { "*"}, QueryOptions.None))
            {
                var result = buildReader.Current<BuildQueueQueryResult>();
                QueuedBuild qb = result.QueuedBuilds.FirstOrDefault();

                string buildUrl = String.Format("{0}/{1}/{2}/_build#buildUri={3}&_a=summary",
                locationService.GetAccessMapping(requestContext, "PublicAccessMapping").AccessPoint,
                requestContext.ServiceHost.Name,
                build.TeamProject,
                build.Uri);
                var notification = new BuildCompletionNotification()
                {
                    BuildUrl = buildUrl,
                    ProjectName = build.TeamProject,
                    BuildNumber = build.BuildNumber,
                    BuildStatus = build.Status,
                    BuildReason = build.Reason,
                    StartTime =  build.StartTime,
                    FinishTime = build.FinishTime,
                    RequestedFor = qb.RequestedFor,
                    RequestedForDisplayName = qb.RequestedForDisplayName
                };

                return notification;
            }

            
        }
    }
}
