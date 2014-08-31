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
    public class ConfigurationHelper
    {
        public static T GetConfigurationSection<T>(Assembly assembly, string sectionName) where T : ConfigurationSection
        {
            ResolveEventHandler resolver = (s, a) => { return assembly; };
            AppDomain.CurrentDomain.AssemblyResolve += resolver;

            string configPath = new Uri(assembly.CodeBase).LocalPath + ".config";
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(
                    new ExeConfigurationFileMap() { ExeConfigFilename = configPath },
                    ConfigurationUserLevel.None);
            var section = configuration.GetSection(sectionName) as T;

            AppDomain.CurrentDomain.AssemblyResolve -= resolver;

            return section;
        }
    }
}
