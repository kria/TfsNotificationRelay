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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.Configuration
{
    public class Tfs2SlackSection : ConfigurationSection
    {
        private static Tfs2SlackSection instance = ConfigurationHelper.GetConfigurationSection<Tfs2SlackSection>(Assembly.GetExecutingAssembly(), "applicationSettings/tfs2Slack");

        public static Tfs2SlackSection Instance
        {
            get { return instance; }
        }
        [ConfigurationProperty("settings")]
        public SettingsElement Settings
        {
            get { return (SettingsElement)this["settings"]; }
        }

        [ConfigurationProperty("bots", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(BotElementCollection),
            AddItemName = "bot")]
        public BotElementCollection Bots
        {
            get { return (BotElementCollection)base["bots"]; }
        }

        [ConfigurationProperty("text")]
        public TextElement Text
        {
            get { return (TextElement)this["text"]; }
        }

        [ConfigurationProperty("xmlns")]
        public string Xmlns
        {
            get { return (string)this["xmlns"]; }
        }

        [ConfigurationProperty("xmlns:xsi")]
        public string XmlnsXsi
        {
            get { return (string)this["xmlns:xsi"]; }
        }

        [ConfigurationProperty("xsi:noNamespaceSchemaLocation")]
        public string XsiNoNamespaceSchemaLocation
        {
            get { return (string)this["xsi:noNamespaceSchemaLocation"]; }
        }

    }
}
