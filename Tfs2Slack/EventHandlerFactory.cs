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

using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Integration.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevCore.Tfs2Slack.EventHandlers;
using DevCore.Tfs2Slack.Configuration;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;

namespace DevCore.Tfs2Slack
{
    class EventHandlerFactory
    {
        public static IEventHandler GetHandler(object notificationEventArgs)
        {
            if (notificationEventArgs is PushNotification)
                return new GitPushHandler();
            else if (notificationEventArgs is BuildCompletionNotificationEvent)
                return new BuildCompletionHandler();
            else if (notificationEventArgs is ProjectCreatedEvent)
                return new ProjectCreatedHandler();
            else if (notificationEventArgs is ProjectDeletedEvent)
                return new ProjectDeletedHandler();
            else if (notificationEventArgs is CheckinNotification)
                return new CheckinHandler();
            else if (notificationEventArgs is WorkItemChangedEvent)
                return new WorkItemChangedHandler();
            else
                throw new NotImplementedException();
                

        }
    }
}
