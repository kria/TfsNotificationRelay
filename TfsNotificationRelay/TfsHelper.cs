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

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;
using System.Linq;

namespace DevCore.TfsNotificationRelay
{
    public static class TfsHelper
    {
        public static bool IsDescendantOf(this TfsGitCommit commit, TeamFoundationRequestContext requestContext, Sha1Id ancestorId)
        {
            Queue<TfsGitCommit> q = new Queue<TfsGitCommit>();
            HashSet<Sha1Id> visited = new HashSet<Sha1Id>();

            q.Enqueue(commit);

            while (q.Count > 0)
            {
                TfsGitCommit current = q.Dequeue();
                if (!visited.Add(current.ObjectId))
                    continue;

                if (current.ObjectId.Equals(ancestorId)) return true;

                foreach (var c in current.GetParents(requestContext))
                    q.Enqueue(c);

            }
            return false;
        }

        public static bool IsForceRequired(this PushNotification pushNotification, TeamFoundationRequestContext requestContext, TfsGitRepository repository)
        {
            foreach (var refUpdateResult in pushNotification.RefUpdateResults.Where(r => r.Succeeded))
            {
                // Don't bother with new or deleted refs
                if (refUpdateResult.OldObjectId.IsEmpty || refUpdateResult.NewObjectId.IsEmpty) continue;

                TfsGitObject gitObject = repository.LookupObject(requestContext, refUpdateResult.NewObjectId);
                if (gitObject.ObjectType != GitObjectType.Commit) continue;
                TfsGitCommit gitCommit = (TfsGitCommit)gitObject;

                if (!gitCommit.IsDescendantOf(requestContext, refUpdateResult.OldObjectId))
                    return true;
            }

            return false;
        }

    }
}
