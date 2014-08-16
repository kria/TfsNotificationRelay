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
using System.Linq;

namespace DevCore.Tfs2Slack
{
    public class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] a, byte[] b)
        {
            if (a == null || b == null)
                return a == b;
            return a.SequenceEqual(b);
        }
        public int GetHashCode(byte[] x)
        {
            if (x == null)
                throw new ArgumentNullException();
            int iHash = 0;
            for (int i = 0; i < x.Length; ++i)
                iHash ^= (x[i] << ((0x03 & i) << 3));
            return iHash;
        }
    }
}
