﻿using Sphynx.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sphynx.Core
{
    public static class Application
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern string GetAppName();

        public static readonly string ApplicationName = GetAppName(); 
    }
}
