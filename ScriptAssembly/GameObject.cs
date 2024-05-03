﻿using Sphynx.Core;
using Sphynx.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Sphynx
{
    /// <summary>
    /// GameObject: Holds <see cref="Component"/>s that act as modules extending and adding functionality to the game.
    /// </summary>
    public sealed class GameObject
    {
        /// <summary>
        /// A Set of Primitive Graphical Objects.
        /// </summary>
        public enum Primitive { Sphere, Capsule, Cube, Plane, Triangle};

        public string Name { get; set; }
        //ID Field
        EntityID id;
        /// <summary>
        /// Gets the GameObject ID
        /// </summary>
        public EntityID ID { get => id; }

        internal readonly HandleRef NativePtr;
        /// <summary>
        /// The Transform Component.
        /// </summary>
        public Transform transform { get; private set; }

        /// <summary>
        /// Used by the runtime.
        /// </summary>
        internal GameObject()
        {
            transform = new Transform();
            transform.gameObject = this;
            transform.Awake();
        }

        internal Dictionary<Type,Component> components = new Dictionary<Type, Component>();
        internal void InternalAddComp(Component comp,Type type)
        {
            components.Add(type, comp);
            Logger.Info($"Native-Side Component {comp.ID} Added to 0x{id.NumericRepresentation.ToString("X")} ");
            comp.gameObject = this;
        }
        /// <summary>
        /// TODO: Not Implemented.
        /// Create a GameObject with the specified primitive (Has a MeshRenderer).
        /// </summary>
        /// <returns>A new GameObject</returns>
        public static GameObject CreatePrimitive(Primitive primitive)
        {
            return null;
        }
        /// <summary>
        /// Gets the Component.
        /// </summary>
        /// <typeparam name="T">Must Derive from <see cref="Sphynx.Component"/> </typeparam>
        public T GetComponent<T>() where T : Component
        {
            foreach(var component in components.Values)
            {
                if(component.GetType() == typeof(T))
                {
                    return (T)component;
                }
            }
            return null;
        }
        /// <summary>
        /// Adds a Component to the GameObject and returns it.
        /// </summary>
        /// <typeparam name="T">Must Derive from <see cref="Sphynx.Component"/> </typeparam>
        public T AddComponent<T>() where T : Component, new()
        {           
            var comp = ComponentFactory.CreateComponent<T>(this);
            components.Add(typeof(T),comp);
            return comp;
        }

        public void RemoveComponent<T>() where T : Component, new()
        {
            var comp = components[typeof(T)];
            if (comp.GetType() == typeof(T))
            {
                components.Remove(typeof(T));
                comp.Destroy();
            }

        }
        /// <summary>
        /// TODO: Still not implemented.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>null if not found.</returns>
        public static GameObject FindGameObjectByName(string name)
        {
            return null;
        }
        /// <summary>
        /// TODO: Still not implemented.
        /// </summary>
        /// <returns>null if not found.</returns>
        public static GameObject FindGameObjectByID(ulong ID)
        {
            return null;
        }

        /// <summary>
        /// Avoid.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public Component FindComponentByIndex(int index)
        {
            if(index >= components.Count)
            {
                throw new IndexOutOfRangeException();
            }
            return components.ElementAt(index).Value;
        }

        [SuppressUnmanagedCodeSecurity]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern IntPtr GetNativeComp(IntPtr GO, string name);

        /// <summary>
        /// <para>
        /// Native components are special components created by the core, they do not live with C# components.
        /// Native components can only be created by <c>C++</c> code.
        /// </para>
        /// <para>
        /// Although, <c>C#</c> components have an underlying native component (<see cref="NativeScript"/>) and 
        /// are the only exception of native components that can be instance in managed code.
        /// (in case of <see cref="Transform"/> the engine creates it, <see cref="Camera"/> is also a special case)
        /// </para>
        /// </summary>
        /// <param name="name">The type name of the native component.</param>
        /// <returns>A pointer to the component.</returns>
        internal IntPtr GetNativeComponent(string name)
        {
            return GetNativeComp(NativePtr.Handle, name);
        }

        internal void Awake()
        {
            transform.Awake();
            foreach (var comp in components.Values)
            {
                comp.Awake();
            }
        }
        internal void Start()
        {
            transform.Start();
            foreach (var comp in components.Values)
            {
                comp.Start();
            }
        }
        internal void Update()
        {
            foreach (var comp in components.Values)
            {
                comp.Update();
            }
            transform.Update();
        }
        internal void FixedUpdate()
        {
            transform.FixedUpdate();
            foreach (var comp in components.Values)
            {
                comp.FixedUpdate();
            }
        }
        internal void OnDestroy()
        {
            transform.OnDestroy();
            foreach (var comp in components.Values)
            {
                comp.OnDestroy();
            }
        }
    }
}
