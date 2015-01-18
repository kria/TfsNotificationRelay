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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay
{
    public class StringTransformFormatProvider : IFormatProvider, ICustomFormatter
    {
        protected Func<string, string> transform;
        public StringTransformFormatProvider(Func<string, string> transform)
        {
            this.transform = transform;
        }
        public static StringTransformFormatProvider From(Func<string, string> transform)
        {
            return transform != null ? new StringTransformFormatProvider(transform) : null;
        }
        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg is string)
            {
                return transform(arg as string);
            }
            else
            {
                try
                {
                    return HandleOtherFormats(format, arg);
                }
                catch (FormatException e)
                {
                    throw new FormatException(String.Format("The format of '{0}' is invalid.", format), e);
                }
            }
        }

        private string HandleOtherFormats(string format, object arg)
        {
            var formattable = arg as IFormattable;
            if (formattable != null)
                return formattable.ToString(format, CultureInfo.CurrentCulture);
            return arg != null ? arg.ToString() : String.Empty;
        }
    }
}
