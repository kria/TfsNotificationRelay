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
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.VersionControl.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.EventHandlers
{
    class DebugHandler : BaseHandler
    {
        public override Type[] SubscribedTypes()
        {
            return new Type[] { 
                typeof(TitleDescriptionUpdatedNotification),
                typeof(MergeCompletedNotification),
                typeof(ReviewersUpdateNotification),
                typeof(PreProjectDeletionNotification),
                typeof(LabelNotification),
                typeof(RepositoryCreatedNotification)
            };
        }

        protected override Notifications.INotification CreateNotification(TeamFoundationRequestContext requestContext, object eventargs, int maxLines)
        {
            Logger.Log("eventargs: " + JObject.FromObject(eventargs).ToString());

            throw new Tfs2SlackException("DebugHandler");
        }
    }
}
