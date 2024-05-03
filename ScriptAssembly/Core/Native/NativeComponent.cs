﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sphynx.Core.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeComponent : IDisposable
    {
        internal HandleRef NativePtr;

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool NativeFinalize(HandleRef NativePointer);

        [AllowReversePInvokeCalls]
        public void Dispose()
        {
            NativeFinalize(NativePtr);
        }
    }
}
