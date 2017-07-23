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

using Microsoft.TeamFoundation.Build.WebApi;

namespace DevCore.TfsNotificationRelay
{
    class Build2Converter
    {
        public Microsoft.TeamFoundation.Build.Server.BuildStatus ConvertBuildStatus(BuildStatus? status, BuildResult? result)
        {
            if (!status.HasValue)
            {
                return Microsoft.TeamFoundation.Build.Server.BuildStatus.NotStarted;
            }
            BuildStatus value = status.Value;
            switch (value)
            {
                case BuildStatus.None:
                    return Microsoft.TeamFoundation.Build.Server.BuildStatus.None;
                case BuildStatus.InProgress:
                    return Microsoft.TeamFoundation.Build.Server.BuildStatus.InProgress;
                case BuildStatus.Completed:
                    return this.ConvertBuildResult(result);
                case (BuildStatus)3:
                case (BuildStatus)5:
                case (BuildStatus)6:
                case (BuildStatus)7:
                    break;
                case BuildStatus.Cancelling:
                    return Microsoft.TeamFoundation.Build.Server.BuildStatus.Stopped;
                case BuildStatus.Postponed:
                    return Microsoft.TeamFoundation.Build.Server.BuildStatus.NotStarted;
                default:
                    if (value == BuildStatus.NotStarted)
                    {
                        return Microsoft.TeamFoundation.Build.Server.BuildStatus.NotStarted;
                    }
                    break;
            }
            return Microsoft.TeamFoundation.Build.Server.BuildStatus.NotStarted;
        }

        public Microsoft.TeamFoundation.Build.Server.BuildStatus ConvertBuildResult(BuildResult? result)
        {
            if (!result.HasValue)
            {
                return Microsoft.TeamFoundation.Build.Server.BuildStatus.None;
            }
            BuildResult value = result.Value;
            switch (value)
            {
                case BuildResult.None:
                    return Microsoft.TeamFoundation.Build.Server.BuildStatus.None;
                case (BuildResult)1:
                case (BuildResult)3:
                    break;
                case BuildResult.Succeeded:
                    return Microsoft.TeamFoundation.Build.Server.BuildStatus.Succeeded;
                case BuildResult.PartiallySucceeded:
                    return Microsoft.TeamFoundation.Build.Server.BuildStatus.PartiallySucceeded;
                default:
                    if (value == BuildResult.Failed)
                    {
                        return Microsoft.TeamFoundation.Build.Server.BuildStatus.Failed;
                    }
                    if (value == BuildResult.Canceled)
                    {
                        return Microsoft.TeamFoundation.Build.Server.BuildStatus.Stopped;
                    }
                    break;
            }
            return Microsoft.TeamFoundation.Build.Server.BuildStatus.None;
        }

        public Microsoft.TeamFoundation.Build.Server.BuildReason ConvertReason(BuildReason reason)
        {
            if (reason <= BuildReason.UserCreated)
            {
                switch (reason)
                {
                    case BuildReason.None:
                        return Microsoft.TeamFoundation.Build.Server.BuildReason.None;
                    case BuildReason.Manual:
                        return Microsoft.TeamFoundation.Build.Server.BuildReason.Manual;
                    case BuildReason.IndividualCI:
                        return Microsoft.TeamFoundation.Build.Server.BuildReason.IndividualCI;
                    case (BuildReason)3:
                    case (BuildReason)5:
                    case (BuildReason)6:
                    case (BuildReason)7:
                        break;
                    case BuildReason.BatchedCI:
                        return Microsoft.TeamFoundation.Build.Server.BuildReason.BatchedCI;
                    case BuildReason.Schedule:
                        return Microsoft.TeamFoundation.Build.Server.BuildReason.Schedule;
                    default:
                        if (reason == BuildReason.UserCreated)
                        {
                            return Microsoft.TeamFoundation.Build.Server.BuildReason.UserCreated;
                        }
                        break;
                }
            }
            else
            {
                if (reason == BuildReason.ValidateShelveset)
                {
                    return Microsoft.TeamFoundation.Build.Server.BuildReason.ValidateShelveset;
                }
                if (reason == BuildReason.CheckInShelveset)
                {
                    return Microsoft.TeamFoundation.Build.Server.BuildReason.CheckInShelveset;
                }
            }
            return Microsoft.TeamFoundation.Build.Server.BuildReason.Manual;
        }

    }
}
