using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Apoc3D.Graphics
{
    /// <summary>
    /// Describes the location for the include file.
    /// </summary>
    public enum IncludeType
    {
        /// <summary>
        /// Look in the local project for the include file.
        /// </summary>
        Local = 0,
        /// <summary>
        /// Look in the system path for the include file.
        /// </summary>
        System = 1
    }

    /// <summary>
    ///  表示Shader 代码中 Include 文件的文件操作处理器
    /// </summary>
    public interface Include
    {
        void Close(Stream stream);
        void Open(IncludeType includeType, string fileName, out Stream stream);
    }

    /// <summary>
    ///  表示 Shader 代码中的 Marco
    /// </summary>
    public struct Macro
    {
        string name;
        string defination;

        public string Name
        {
            get { return name; }
        }
        public string Defination
        {
            get { return defination; }
        }
        public Macro(string name, string def)
        {
            this.name = name;
            this.defination = def;
        }
    }

    /// <summary>
    ///  提供Shader的抽象接口
    /// </summary>
    public abstract class Shader : IDisposable
    {
        protected Shader(RenderSystem rs)
        {
            RenderSystem = rs;
        }

        public RenderSystem RenderSystem
        {
            get;
            private set;
        }

        public abstract void SetValue<T>(string paramName, T value) where T : struct; 

        #region IDisposable 成员

        public bool Disposed
        {
            get;
            private set;
        }

        public virtual void Dispose(bool disposing)
        {
            RenderSystem = null;
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                Dispose(true);
                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        ~Shader()
        {
            if (!Disposed)
            {
                Dispose(false);
                Disposed = true;
            }
        }
        #endregion
    }

    /// <summary>
    ///  表示顶点Shader
    /// </summary>
    public abstract class VertexShader : Shader
    {
        protected VertexShader(RenderSystem rs)
            : base(rs)
        {
        }
    }

    /// <summary>
    ///  表示像素Shader
    /// </summary>
    public abstract class PixelShader : Shader
    {
        protected PixelShader(RenderSystem rs)
            : base(rs)
        {
        }
    }
}
