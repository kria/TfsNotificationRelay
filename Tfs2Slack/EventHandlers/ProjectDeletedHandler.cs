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
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.EventHandlers
{
    class ProjectDeletedHandler : BaseHandler<ProjectDeletedEvent>
    {
        protected override INotification CreateNotification(TeamFoundationRequestContext requestContext, ProjectDeletedEvent ev, int maxLines)
        {
            string projectName;
            if (this.ProjectsNames.TryGetValue(ev.Uri, out projectName))
            {
                this.ProjectsNames.Remove(ev.Uri);
            }

            return new ProjectDeletedNotification() { TeamProjectCollection = requestContext.ServiceHost.Name, ProjectUri = ev.Uri, ProjectName = projectName };
        }
    }
}
