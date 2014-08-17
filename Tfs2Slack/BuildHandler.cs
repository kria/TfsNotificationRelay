using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack
{
    class BuildHandler
    {
        private static Properties.Settings settings = Properties.Settings.Default;
        private static Properties.Text text = Properties.Text.Default;

        public static List<string> CreateMessage(TeamFoundationRequestContext requestContext, BuildCompletionNotificationEvent buildNotification)
        {
            var lines = new List<string>();

            var build = buildNotification.Build;

            if (settings.NotifyOnBuildSucceeded && build.Status.HasFlag(BuildStatus.Succeeded) ||
                (settings.NotifyOnBuildFailed && build.Status.HasFlag(BuildStatus.Failed)))
            {
                var locationService = requestContext.GetService<TeamFoundationLocationService>();

                string buildUrl = String.Format("{0}/{1}/{2}/_build#buildUri={3}&_a=summary",
                    locationService.GetAccessMapping(requestContext, "PublicAccessMapping").AccessPoint,
                    requestContext.ServiceHost.Name,
                    build.TeamProject,
                    build.Uri);
                
                lines.Add(text.FormatBuildText(buildUrl, build.TeamProject, build.BuildNumber, build.Status.ToString()));
            }

            return lines;
        }

    }
}
