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
using Microsoft.TeamFoundation.Git.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevCore.TfsNotificationRelay.Notifications.GitPush
{
    public class GitRef
    {
        public string Name { get; }
        public string FullName { get; }
        public byte[] CommitId { get; }
        public GitRefType Type { get; }
        public bool IsNew { get; }

        public GitRef(TfsGitRefUpdateResult updateResult)
        {
            CommitId = updateResult.NewObjectId;
            FullName = updateResult.Name;
            IsNew = updateResult.OldObjectId.IsZero();

            if (updateResult.Name.StartsWith("refs/heads/"))
            {
                Type = GitRefType.Branch;
                Name = updateResult.Name.Replace("refs/heads/", "");
            }
            else if (updateResult.Name.StartsWith("refs/tags/"))
            {
                Type = GitRefType.Tag;
                Name = updateResult.Name.Replace("refs/tags/", "");
            }
            else
            {
                Type = GitRefType.Other;
                Name = updateResult.Name;
            }
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
