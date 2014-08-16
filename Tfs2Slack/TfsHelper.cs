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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack
{
    public static class TfsHelper
    {
        public static bool IsDescendantOf(this TfsGitCommit commit, TeamFoundationRequestContext requestContext, byte[] ancestorId)
        {
            return MatchAncestor(requestContext, commit.GetParents(requestContext), ancestorId);
        }

        private static bool MatchAncestor(TeamFoundationRequestContext requestContext, IEnumerable<TfsGitCommit> commits, byte[] ancestorId)
        {
            if (commits.Count() == 0) return false;

            return commits.First().ObjectId.SequenceEqual(ancestorId) 
                || MatchAncestor(requestContext, commits.First().GetParents(requestContext), ancestorId) 
                || MatchAncestor(requestContext, commits.Skip(1), ancestorId);
        }

        public static bool IsForceRequired(this PushNotification pushNotification, TeamFoundationRequestContext requestContext, TfsGitRepository repository)
        {
            foreach (var refUpdateResult in pushNotification.RefUpdateResults.Where(r => r.Succeeded))
            {
                // Don't bother with new or deleted refs
                if (refUpdateResult.OldObjectId.IsZero() || refUpdateResult.NewObjectId.IsZero()) continue;

                TfsGitObject gitObject = repository.LookupObject(requestContext, refUpdateResult.NewObjectId);
                if (gitObject.ObjectType != TfsGitObjectType.Commit) continue;
                TfsGitCommit gitCommit = (TfsGitCommit)gitObject;

                if (!gitCommit.IsDescendantOf(requestContext, refUpdateResult.OldObjectId))
                    return true;
            }

            return false;
        }

    }
}
