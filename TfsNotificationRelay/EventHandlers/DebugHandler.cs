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
using Microsoft.TeamFoundation.Common;

namespace DevCore.TfsNotificationRelay.EventHandlers
{
    class DebugHandler : ISubscriber
    {
        public string Name => "DebugHandler";

        public SubscriberPriority Priority => SubscriberPriority.Normal;

        public Type[] SubscribedTypes()
        {
            return new Type[] {
                typeof(RepositoryCreatedNotification),
                typeof(RepositoryDeletedNotification),
                typeof(RepositoryRenamedNotification)
            };
        }

        public EventNotificationStatus ProcessEvent(IVssRequestContext requestContext, NotificationType notificationType, object notificationEventArgs, out int statusCode, out string statusMessage, out ExceptionPropertyCollection properties)
        {
            statusCode = 0;
            statusMessage = string.Empty;
            properties = null;

            Logger.Log("notificationType: " + notificationType);
            Logger.Log("event: " + notificationEventArgs.GetType());
            Logger.Log("eventargs", notificationEventArgs);

            return EventNotificationStatus.ActionApproved;
        }
    }
}
