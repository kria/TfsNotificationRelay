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

using DevCore.TfsNotificationRelay.Configuration;
using DevCore.TfsNotificationRelay.Notifications;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.EventHandlers
{
    class ProjectDeletedHandler : BaseHandler<ProjectDeletedEvent>
    {
        protected override IEnumerable<INotification> CreateNotifications(TeamFoundationRequestContext requestContext, ProjectDeletedEvent ev, int maxLines)
        {
            string projectName;
            if (this.ProjectsNames.TryGetValue(ev.Uri, out projectName))
            {
                this.ProjectsNames.Remove(ev.Uri);
            }

            yield return new ProjectDeletedNotification() { TeamProjectCollection = requestContext.ServiceHost.Name, ProjectUri = ev.Uri, ProjectName = projectName };
        }
    }
}
