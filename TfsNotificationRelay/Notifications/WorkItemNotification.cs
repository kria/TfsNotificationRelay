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

namespace DevCore.TfsNotificationRelay.Notifications
{
    public abstract class WorkItemNotification : BaseNotification
    {
        protected static Configuration.SettingsElement settings = Configuration.TfsNotificationRelaySection.Instance.Settings;

        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string WiUrl { get; set; }
        public string WiType { get; set; }
        public int WiId { get; set; }
        public string WiTitle { get; set; }
        public string ProjectName { get; set; }
        public string AreaPath { get; set; }

        public string UserName
        {
            get { return settings.StripUserDomain ? TextHelper.StripDomain(UniqueName) : UniqueName; }
        }

    }
}
