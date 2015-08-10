/*
 * TfsNotificationRelay - http://github.com/kria/TfsNotificationRelay
 * 
 * Copyright (C) 2015 Kristian Adrup
 * 
 * This file is part of TfsNotificationRelay.
 * 
 * TfsNotificationRelay is free software: you can redistribute it and/or 
 * modify it under the terms of the GNU General Public License as published 
 * by the Free Software Foundation, either version 3 of the License, or 
 * (at your option) any later version. See included file COPYING for details.
 */

using DevCore.TfsNotificationRelay.Configuration;
using Microsoft.TeamFoundation.Git.Common;
using Microsoft.TeamFoundation.Git.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevCore.TfsNotificationRelay.Notifications
{
    public class GitRef
    {
        public string Name { get; private set; }
        public string FullName { get; private set; }
        public Sha1Id CommitId { get; private set; }
        public GitRefType Type { get; private set; }
        public bool IsNew { get; private set; }

        public GitRef(string fullName)
        {
            FullName = fullName;

            if (fullName.StartsWith("refs/heads/"))
            {
                Type = GitRefType.Branch;
                Name = fullName.Replace("refs/heads/", "");
            }
            else if (fullName.StartsWith("refs/tags/"))
            {
                Type = GitRefType.Tag;
                Name = fullName.Replace("refs/tags/", "");
            }
            else
            {
                Type = GitRefType.Other;
                Name = fullName;
            }
        }
        public GitRef(TfsGitRefUpdateResult updateResult) : this(updateResult.Name)
        {
            CommitId = updateResult.NewObjectId;
            IsNew = updateResult.OldObjectId.IsEmpty;
        }

        public string ToString(BotElement bot, Func<string, string> transform)
        {
            string pattern = Type == GitRefType.Tag ? bot.Text.TagFormat : bot.Text.BranchFormat;
            var sb = new StringBuilder();
            if (Type == GitRefType.Branch && IsNew) sb.Append("+");
            sb.Append(Name);

            return pattern.FormatWith(new
            {
                Name = transform(sb.ToString())
            });
        }
    }

    public static class GitRefExtension
    {
        public static string ToString(this IEnumerable<GitRef> refs, BotElement bot, Func<string, string> transform)
        {
            return String.Join(bot.Text.RefSeparator, refs.Select(r => r.ToString(bot, transform)));
        }
    }

    public enum GitRefType
    {
        Branch,
        Tag,
        Other
    }
}
