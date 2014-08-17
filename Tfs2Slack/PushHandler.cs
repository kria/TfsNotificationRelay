using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Integration.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack
{
    class PushHandler
    {
        private static Properties.Settings settings = Properties.Settings.Default;
        private static Properties.Text text = Properties.Text.Default;

        public static List<string> CreateMessage(TeamFoundationRequestContext requestContext, PushNotification pushNotification)
        {
            var repositoryService = requestContext.GetService<TeamFoundationGitRepositoryService>();
            var commonService = requestContext.GetService<CommonStructureService>();

            using (TfsGitRepository repository = repositoryService.FindRepositoryById(requestContext, pushNotification.RepositoryId))
            {
                string repoName = pushNotification.RepositoryName;
                string repoUri = repository.GetRepositoryUri(requestContext);
                string projectName = commonService.GetProject(requestContext, pushNotification.TeamProjectUri).Name;
                string userName = pushNotification.AuthenticatedUserName;
                if (settings.StripUserDomain) userName = Utils.StripDomain(userName);

                var lines = new List<string>();

                lines.Add(text.FormatPushText(userName, repoUri, projectName, repoName, pushNotification.IsForceRequired(requestContext, repository)));

                var refNames = new Dictionary<byte[], List<string>>(new ByteArrayComparer());
                var oldCommits = new HashSet<byte[]>(new ByteArrayComparer());
                var unknowns = new List<RefUpdateResultGroup>();

                // Associate refs (branch, lightweight and annotated tag) with corresponding commit
                var refUpdateResultGroups = pushNotification.RefUpdateResults
                    .Where(r => r.Succeeded)
                    .GroupBy(r => r.NewObjectId, (key, items) => new RefUpdateResultGroup(key, items), new ByteArrayComparer());

                foreach (var refUpdateResultGroup in refUpdateResultGroups)
                {
                    byte[] newObjectId = refUpdateResultGroup.NewObjectId;
                    byte[] commitId = null;

                    if (newObjectId.IsZero())
                    {
                        commitId = newObjectId;
                    }
                    else
                    {
                        TfsGitObject gitObject = repository.LookupObject(requestContext, newObjectId);

                        if (gitObject.ObjectType == TfsGitObjectType.Commit)
                        {
                            commitId = newObjectId;
                        }
                        else if (gitObject.ObjectType == TfsGitObjectType.Tag)
                        {
                            var tag = (TfsGitTag)gitObject;
                            var commit = tag.TryResolveToCommit(requestContext);
                            if (commit != null)
                            {
                                commitId = commit.ObjectId;
                            }
                        }
                    }

                    if (commitId != null)
                    {
                        List<string> names;
                        if (!refNames.TryGetValue(commitId, out names))
                        {
                            names = new List<string>();
                            refNames.Add(commitId, names);
                        }
                        names.AddRange(RefsToStrings(refUpdateResultGroup.RefUpdateResults));

                        if (commitId.IsZero() || !pushNotification.IncludedCommits.Any(r => r.SequenceEqual(commitId)))
                        {
                            oldCommits.Add(commitId);
                        }
                    }
                    else
                    {
                        unknowns.Add(refUpdateResultGroup);
                    }

                }

                // Display new commits with refs
                foreach (byte[] commitId in pushNotification.IncludedCommits)
                {
                    TfsGitCommit gitCommit = (TfsGitCommit)repository.LookupObject(requestContext, commitId);
                    string line = CommitToString(requestContext, gitCommit, text.Commit, pushNotification, refNames);
                    lines.Add(line);
                }

                // Display updated refs to old commits
                foreach (byte[] commitId in oldCommits)
                {
                    string line = null;

                    if (commitId.IsZero())
                    {
                        line = String.Format("{0} {1}", String.Concat(refNames[commitId]), text.Deleted);
                    }
                    else
                    {
                        TfsGitCommit gitCommit = (TfsGitCommit)repository.LookupObject(requestContext, commitId);
                        line = CommitToString(requestContext, gitCommit, text.RefPointer, pushNotification, refNames);
                    }
                    lines.Add(line);
                }

                // Display "unknown" refs
                foreach (var refUpdateResultGroup in unknowns)
                {
                    byte[] newObjectId = refUpdateResultGroup.NewObjectId;
                    TfsGitObject gitObject = repository.LookupObject(requestContext, newObjectId);
                    string line = String.Format("{0} {1} {2} {3}",
                        RefsToString(refUpdateResultGroup.RefUpdateResults), text.RefPointer, gitObject.ObjectType, newObjectId.ToHexString());

                    lines.Add(line);
                }

                return lines;
            }
        }

        private static string RefsToString(IEnumerable<TfsGitRefUpdateResult> refUpdateResults)
        {
            return String.Concat(RefsToStrings(refUpdateResults));
        }

        private static string[] RefsToStrings(IEnumerable<TfsGitRefUpdateResult> refUpdateResults)
        {
            if (refUpdateResults.Count() == 0) return null;
            var refStrings = new List<string>();
            foreach (var gitRef in refUpdateResults)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append('[');
                if (gitRef.Name.StartsWith("refs/heads/") && gitRef.OldObjectId.IsZero())
                    sb.Append('+');
                sb.AppendFormat("{0}]", gitRef.Name.Replace("refs/heads/", "").Replace("refs/tags/", ""));
                refStrings.Add(sb.ToString());
            }
            return refStrings.ToArray();
        }

        private static string CommitToString(TeamFoundationRequestContext requestContext, TfsGitCommit gitCommit, string action, PushNotification pushNotification,
            Dictionary<byte[], List<string>> refNames)
        {
            string repoUri = gitCommit.Repository.GetRepositoryUri(requestContext);
            string commitUri = repoUri + "/commit/" + gitCommit.ObjectId.ToHexString();
            DateTime authorTime = gitCommit.GetLocalAuthorTime(requestContext);
            string authorName = gitCommit.GetAuthor(requestContext);
            string comment = gitCommit.GetComment(requestContext);

            StringBuilder sb = new StringBuilder();
            List<string> names = null;
            if (refNames.TryGetValue(gitCommit.ObjectId, out names)) sb.AppendFormat("{0} ", String.Concat(names));
            sb.Append(text.FormatCommitText(action, commitUri, gitCommit.ObjectId.ToShortHexString(), authorTime, authorName, comment.Truncate(settings.CommentMaxLength)));

            return sb.ToString();
        }
    }

    class RefUpdateResultGroup
    {
        public RefUpdateResultGroup(byte[] newObjectId, IEnumerable<TfsGitRefUpdateResult> refUpdateResults)
        {
            this.NewObjectId = newObjectId;
            this.RefUpdateResults = refUpdateResults;
        }
        public byte[] NewObjectId { get; set; }
        public IEnumerable<TfsGitRefUpdateResult> RefUpdateResults { get; set; }

    }

}
