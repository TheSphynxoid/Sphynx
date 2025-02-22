﻿using Sphynx.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Sphynx.Graphics
{
    //1:1 From Mesh.h
    /// <summary>
    /// Specifies the use of the mesh.
    /// </summary>
    public enum MeshType : byte
    {
        /// <summary>
        /// Changing Buffer.
        /// </summary>
        Dynamic,
        /// <summary>
        /// Constant Buffer.
        /// </summary>
        Static,
        /// <summary>
        /// GL Specific (Meaning : contents will be modified once and used at most a few times.)
        /// </summary>
        Stream
    };

    /// <summary>
    /// Indicates how the renderer will draw primitives for a mesh.
    /// </summary>
    public enum RenderMode : byte
    {
        Points, Lines, LineLoop, LineStrip, Trig, TrigStrip, TrigFan, 
        LinesAdj = 0xA, LineStripAdj, TrigAdj, TrigStripAdj, Patches /* 1:1 From opengl */
    };

    /// <summary>
    /// Represents the element of data in a <see cref="VertexBuffer"/>.
    /// </summary>
    [NativeCppClass]
    public struct BufferElement
    {
        ulong size;
        [Pure]
        public ulong Size { get => size; }

        internal ulong offset;
        [Pure]
        public ulong Offset { get => offset; }

        readonly byte shaderDataType;
        [Pure]
        public ShaderDataType DataType { get => (ShaderDataType)shaderDataType; }

        public bool Normalized { [Pure] readonly get; init; }

        public BufferElement(ShaderDataType dataType, bool norm)
        {
            Normalized = norm;
            shaderDataType = (byte)dataType;
            size = dataType.GetSize();
        }

        //Copied 1:1 from "Mesh.h".
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte GetComponentCount()
        {
            switch ((ShaderDataType)shaderDataType)
            {
                case ShaderDataType.None:       return 0;
                case ShaderDataType.Float:      return 1;
                case ShaderDataType.Float2:     return 2;
                case ShaderDataType.Float3:     return 3;
                case ShaderDataType.Float4:     return 4;
                case ShaderDataType.Double:     return 1;
                case ShaderDataType.Double2:    return 2;
                case ShaderDataType.Double3:    return 3;
                case ShaderDataType.Double4:    return 4;
                case ShaderDataType.Int:        return 1;
                case ShaderDataType.Int2:       return 2;
                case ShaderDataType.Int3:       return 3;
                case ShaderDataType.Int4:       return 4;
                case ShaderDataType.UInt:       return 1;
                case ShaderDataType.UInt2:      return 2;
                case ShaderDataType.UInt3:      return 3;
                case ShaderDataType.UInt4:      return 4;
                case ShaderDataType.Bool:       return 1;
                case ShaderDataType.Mat2x2:     return 2;
                case ShaderDataType.Mat2x3:     return 3;
                case ShaderDataType.Mat2x4:     return 4;
                case ShaderDataType.Mat3x2:     return 2;
                case ShaderDataType.Mat3x3:     return 3;
                case ShaderDataType.Mat3x4:     return 4;
                case ShaderDataType.Mat4x2:     return 2;
                case ShaderDataType.Mat4x3:     return 3;
                case ShaderDataType.Mat4x4:     return 4;
                default:                        return 0;
            }
        }
    }

    /*Because of arrays this type is not a native cpp type.*/
    /// <summary>
    /// Data layout of the Vertex Buffer, a collection of <see cref="BufferElement"/>.
    /// </summary>
    [Header("Mesh.h")]
    public struct BufferLayout
    {
        public BufferElement[] BufferElements;
        public uint Stride = 0;

        public BufferLayout() { }

        public BufferLayout(BufferElement[] elems)
        {
            ulong offset = 0;
            BufferElements = new BufferElement[elems.Length];
            for (var i = 0; i < elems.Length; i++)
            {
                elems[i].offset = offset;
                BufferElements[i] = elems[i];
                offset += elems[i].Size;
                Stride += (uint)elems[i].Size;
            }
        }

    }

    /// <summary>
    /// A Vertex Buffer Object (VBO) is a buffer in GPU memory (See also: <see cref="GPUBuffer"/>)
    /// that contains information about vertexes to be sent to the Vertex Shader.
    /// This Buffer is necessary for rendering, if not provided <see cref="Mesh"/> will create one with the data provided.
    /// </summary>
    public class VertexBuffer
    {
        HandleRef native;
        GPUBuffer underlying;

        [NativeCppClass]
        struct NativeBufferLayout
        {
            public IntPtr bufferElements;
            public int count;
            public uint stride;

            public NativeBufferLayout(BufferLayout layout)
            {
                bufferElements = Engine.GetArrayPointer(layout.BufferElements);
                count = layout.BufferElements.Length;
                stride = layout.Stride;
            }
        }

        BufferLayout layout;
        public BufferLayout Layout { get => layout; set { layout = value; SetLayout(native.Handle, new NativeBufferLayout(value)); } }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [SuppressUnmanagedCodeSecurity]
        static extern IntPtr CreateVB(IntPtr floatBuffer, nuint count);
        [MethodImpl(MethodImplOptions.InternalCall)]
        [SuppressUnmanagedCodeSecurity]
        static extern IntPtr GetUnderlyingBuffer(IntPtr NativeVB);
        [MethodImpl(MethodImplOptions.InternalCall)]
        [SuppressUnmanagedCodeSecurity]
        static extern void SetLayout(IntPtr NativeVB, NativeBufferLayout l);
        [MethodImpl(MethodImplOptions.InternalCall)]
        [SuppressUnmanagedCodeSecurity]
        static extern void SetData(IntPtr native, IntPtr data, ulong size, ulong offset);
        [MethodImpl(MethodImplOptions.InternalCall)]
        [SuppressUnmanagedCodeSecurity]
        static extern nuint GetVBSize(IntPtr NativeVB);

        public VertexBuffer(float[] verts, int vertCount, BufferLayout? bufferLayout = null)
        {
            native = new(this, CreateVB(Engine.GetArrayPointer(verts), (nuint)vertCount));
            underlying = new(GetUnderlyingBuffer(native.Handle), (nuint)(sizeof(float) * vertCount));
            if(bufferLayout is BufferLayout b)Layout = b;
        }

        public void SetData(float[] data, nuint size) 
        {
            SetData(native.Handle, Engine.GetArrayPointer(data), size, 0);
        }

        internal IntPtr GetNative()
        {
            return native.Handle;
        }

        public GPUBuffer GetBuffer()
        {
            return underlying;
        }

        public nuint Size { get => GetVBSize(native.Handle); }
    }

    /// <summary>
    /// An Index Buffer Object contains the order of the vertexes in <see cref="VertexBuffer"/> to be drawn.
    /// This Buffer is optional for rendering.
    /// </summary>
    public class IndexBuffer
    {
        HandleRef native;
        GPUBuffer underlying;

        uint count;
        public uint Count { get => count; }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [SuppressUnmanagedCodeSecurity]
        static extern IntPtr CreateIB(IntPtr data,  nuint size);
        [MethodImpl(MethodImplOptions.InternalCall)]
        [SuppressUnmanagedCodeSecurity]
        static extern IntPtr GetUnderlyingBuffer(IntPtr native);
        [MethodImpl(MethodImplOptions.InternalCall)]
        [SuppressUnmanagedCodeSecurity]
        static extern void SetData_int(IntPtr native, IntPtr buf, nuint size);

        public IndexBuffer(uint[] indexes)
        {
            native = new HandleRef(this, CreateIB(indexes.ToPointer(), (nuint)indexes.LongLength));
            underlying = new GPUBuffer(GetUnderlyingBuffer(native.Handle), Count * sizeof(uint));
            count = (uint)indexes.Length;
        }

        public void SetData(uint[] indexes)
        {
            count = (uint)indexes.Length;
            SetData_int(native.Handle, indexes.ToPointer(), count * sizeof(uint));
        }

        internal IntPtr GetNative()
        {
            return native.Handle;
        }

        public GPUBuffer GetBuffer()
        {
            return underlying;
        }
    }

    [NativeWrapper("Mesh", true)]
    [Header("Mesh.h")]
    public class Mesh
    {
        internal HandleRef NativePtr;
        internal List<VertexBuffer> VBOs;
        internal IndexBuffer IBO;
        internal RenderMode Mode = RenderMode.Trig;
        public RenderMode RenderMode { get => Mode; set { Mode = value;SetRenderMode(NativePtr.Handle, (byte)value); } }
        
        [MethodImpl(MethodImplOptions.InternalCall)]
        [SuppressUnmanagedCodeSecurity]
        static extern IntPtr CreateMeshEmpty();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [SuppressUnmanagedCodeSecurity]
        static extern IntPtr Create(IntPtr VBuffer, IntPtr IBuffer);
        [MethodImpl(MethodImplOptions.InternalCall)]
        [SuppressUnmanagedCodeSecurity]
        static extern IntPtr CreateList(IntPtr VBList, int Count, IntPtr IBuffer);
        [MethodImpl(MethodImplOptions.InternalCall)]
        [SuppressUnmanagedCodeSecurity]
        static extern void AddVB(IntPtr native, IntPtr VB);
        [MethodImpl(MethodImplOptions.InternalCall)]
        [SuppressUnmanagedCodeSecurity]
        static extern void SetVBs(IntPtr native, int count, IntPtr VBs);
        [MethodImpl(MethodImplOptions.InternalCall)]
        [SuppressUnmanagedCodeSecurity]
        static extern void SetIB(IntPtr native, IntPtr ib);
        [MethodImpl(MethodImplOptions.InternalCall)]
        [SuppressUnmanagedCodeSecurity]
        static extern void Bind(IntPtr native);
        [MethodImpl(MethodImplOptions.InternalCall)]
        [SuppressUnmanagedCodeSecurity]
        static extern void Unbind(IntPtr native);
        [MethodImpl(MethodImplOptions.InternalCall)]
        [SuppressUnmanagedCodeSecurity]
        static extern void SetRenderMode(IntPtr native, byte mode);

        public Mesh()
        {
            NativePtr = new HandleRef(this, CreateMeshEmpty());
        }

        public Mesh(VertexBuffer vbo, IndexBuffer ibo)
        {
            VBOs = new List<VertexBuffer>
            {
                vbo
            };
            IBO = ibo;
            NativePtr = new HandleRef(this, Create(vbo.GetNative(), ibo.GetNative()));
        }

        public Mesh(List<VertexBuffer> vbs, IndexBuffer ib)
        {
            VBOs = vbs;
            IBO = ib;
            var count = vbs.Count;
            var texPtr = new IntPtr[count];
            for (var i = 0; i < count; i++)
            {
                texPtr[i] = vbs[i].GetNative();
            }
            NativePtr = new HandleRef(this, CreateList(texPtr.ToPointer(), count, ib.GetNative()));
        }

        public void AddVertexBuffer(VertexBuffer vbo)
        {
            VBOs.Add(vbo);
            AddVB(NativePtr.Handle, vbo.GetNative());
        }

        public void SetVertexBuffers(List<VertexBuffer> vbs)
        {
            VBOs = vbs;
            IntPtr[] ptrs = new IntPtr[vbs.Count];
            for (int i = 0; i < vbs.Count; i++)
            {
                ptrs[i] = vbs[i].GetNative();
            }
            SetVBs(NativePtr.Handle, vbs.Count, ptrs.ToPointer());
        }

        public void SetIndexBuffer(IndexBuffer ibo)
        {
            SetIB(NativePtr.Handle, ibo.GetNative());
        }

        public void Bind()
        {
            Bind(NativePtr.Handle);
        }

        public void Unbind()
        {
            Unbind(NativePtr.Handle);
        }

        public HandleRef GetNative()
        {
            return NativePtr;
        }
    }
}
