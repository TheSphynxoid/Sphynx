﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Sphynx.Core.Native
{
    internal static class ComponentFactory
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern NativeComponent CreateNative(ulong GameObjectID, string Name);

        internal static List<Component> comps;

        internal static void DestroyComponent(Component component)
        {
            component.OnDestroy();
        }

        public static T CreateComponent<T>(GameObject go) where T : Component, new()
        {
            var comp = new T
            {
                gameObject = go
            };

            comp.Start();

            return comp;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        static extern void CopyNativeComponent(GameObject Source, GameObject Destination, NativeComponent component);

        public static void CopyComponent<T>(GameObject Origin, GameObject Destination) where T : Component, new()
        {
            var comp = Origin.GetComponent<T>();
            if (comp != null)
            {
                //CopyNativeComponent(Origin, Destination, comp.Native);
            }
            else
            {
                //Implement Debugger
                Debugger.Break();
                throw new NullReferenceException("");
            }
        }

        //static internal NativeComponent GetNativeInstance(GameObject go,string CompName)
        //{

        //}
    }
}
