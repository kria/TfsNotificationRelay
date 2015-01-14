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

using DevCore.TfsNotificationRelay.Notifications;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace DevCore.TfsNotificationRelay.EventHandlers
{
    public abstract class BaseHandler<T> : BaseHandler
    {
        public override Type[] SubscribedTypes()
        {
            return new Type[] { typeof(T) };
        }

        protected override INotification CreateNotification(TeamFoundationRequestContext requestContext, object notificationEventArgs, int maxLines)
        {
            return CreateNotification(requestContext, (T)notificationEventArgs, maxLines);
        }

        protected abstract INotification CreateNotification(TeamFoundationRequestContext requestContext, T notificationEventArgs, int maxLines);
    }

    public abstract class BaseHandler : ISubscriber
    {
        protected static Configuration.SettingsElement settings;
        private static IDictionary<string, string> projectsNames;

        public string Name
        {
            get { return "TfsNotificationRelay handler"; }
        }

        public SubscriberPriority Priority
        {
            get { return SubscriberPriority.Normal; }
        }

        public IDictionary<string, string> ProjectsNames
        {
            get { return projectsNames; }
        }

        public abstract Type[] SubscribedTypes();

        protected abstract INotification CreateNotification(TeamFoundationRequestContext requestContext, object notificationEventArgs, int maxLines);

        public virtual EventNotificationStatus ProcessEvent(TeamFoundationRequestContext requestContext, NotificationType notificationType,
            object notificationEventArgs, out int statusCode, out string statusMessage, out Microsoft.TeamFoundation.Common.ExceptionPropertyCollection properties)
        {
            settings = Configuration.TfsNotificationRelaySection.Instance.Settings;
            statusCode = 0;
            statusMessage = string.Empty;
            properties = null;

            if (projectsNames == null)
            {
                var commonService = requestContext.GetService<CommonStructureService>();
                projectsNames = commonService.GetProjects(requestContext).ToDictionary(p => p.Uri, p => p.Name);
            }

            var config = Configuration.TfsNotificationRelaySection.Instance;

            if (notificationType == NotificationType.Notification)
            {
                var notification = CreateNotification(requestContext, notificationEventArgs, settings.MaxLines);

                foreach (var bot in config.Bots)
                {
                    if (!notification.IsMatch(requestContext.ServiceHost.Name, bot.EventRules)) continue;

                    var botType = Type.GetType(bot.Type);
                    if (botType != null)
                    {
                        try
                        {
                            var notifier = (INotifier)Activator.CreateInstance(botType);
                            notifier.Notify(notification, bot);
                        }
                        catch (Exception ex)
                        {
                            TeamFoundationApplicationCore.LogException(requestContext, "TfsNotificationRelay: Problem notifying bot: " + bot.Id, ex);
                        }
                    }
                    else
                    {
                        string errmsg = String.Format("TfsNotificationRelay: Unknown bot type: {0}, Skipping bot: {1}", bot.Type, bot.Id);
                        TeamFoundationApplicationCore.Log(requestContext, errmsg, 0, EventLogEntryType.Error);
                    }
                }
            }

            return EventNotificationStatus.ActionPermitted;
        }


    }

}
