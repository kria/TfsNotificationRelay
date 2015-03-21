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

namespace DevCore.TfsNotificationRelay
{
    [Flags]
    public enum TfsEvents : uint
    {
        None = 0,
        GitPush = 1,
        BuildSucceeded = 2,
        BuildFailed = 4,
        ProjectCreated = 8,
        ProjectDeleted = 16,
        Checkin = 32,
        WorkItemStateChange = 64,
        WorkItemAssignmentChange = 128,
        PullRequestCreated = 256,
        PullRequestStatusUpdate = 512,
        PullRequestReviewerVote = 1024,
        BuildQualityChanged = 2048,

        All = 0xFFFFFFFF
    }
}
