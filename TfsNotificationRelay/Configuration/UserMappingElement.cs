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

using System.Configuration;

namespace DevCore.TfsNotificationRelay.Configuration
{
    public class UserMappingElement : ConfigurationElement, IKeyedConfigurationElement
    {
        public object Key => this;

        [ConfigurationProperty("tfsUser")]
        public string TfsUser => (string)this["tfsUser"];

        [ConfigurationProperty("mappedUser")]
        public string MappedUser => (string)this["mappedUser"];
    }
}
