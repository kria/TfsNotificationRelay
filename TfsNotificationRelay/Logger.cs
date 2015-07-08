/*
 * TfsNotificationRelay - http://github.com/kria/TfsNotificationRelay
 * 
 * Copyright (C) 2014 Kristian Adrup
 * 
 * This file is part of TfsNotificationRelay.
 * 
 * TfsNotificationRelay is free software: you can redistribute it and/or 
 * modify it under the terms of the GNU General Public License as published 
 * by the Free Software Foundation, either version 3 of the License, or 
 * (at your option) any later version. See included file COPYING for details.
 */

using DevCore.TfsNotificationRelay.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay
{
    [Obsolete]
    public class Logger
    {
        public static void Log(IEnumerable<string> lines)
        {
            if (String.IsNullOrEmpty(TfsNotificationRelaySection.Instance.Settings.Logfile)) return;

            using (StreamWriter sw = File.AppendText(TfsNotificationRelaySection.Instance.Settings.Logfile))
            {
                foreach (string line in lines)
                {
                    sw.WriteLine("[{0}] {1}", DateTime.Now, line);
                }
            }
        }

        public static void Log(string line)
        {
            Log(new[] { line });
        }

        public static void Log(string format, params Object[] args)
        {
            Log(String.Format(format, args));
        }

        public static void Log(Exception ex)
        {
            Log(ex.ToString());
        }

        public static void Log(string arg, object obj)
        {
            Log(arg + ":" + Utils.Dump(obj));
        }
    }
}
