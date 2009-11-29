using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Scene;
using Apoc3D.Vfs;

namespace Apoc3D.Graphics.Effects
{
    public abstract class EffectFactory
    {
        public abstract Effect CreateInstance();

        public abstract void DestroyInstance(Effect fx);
    }

    public abstract class Effect
    {
        bool begun;

        public Effect(bool supportsInstancing, string name)
        {
            Name = name;
            SupportsInstancing = supportsInstancing;
        }
       
        protected VertexShader LoadVertexShader(RenderSystem rs, ResourceLocation vs, Macro[] macros, string funcName)
        {
            ObjectFactory fac = rs.ObjectFactory;
            return VertexShader.FromResource(fac, vs, macros, funcName);
        }
        protected PixelShader LoadPixelShader(RenderSystem rs, ResourceLocation ps, Macro[] macros, string funcName)
        {
            ObjectFactory fac = rs.ObjectFactory;
            return PixelShader.FromResource(fac, ps, macros, funcName);
        }

        #region 属性
        public bool SupportsInstancing
        {
            get;
            protected set;
        }

        public string Name
        {
            get;
            private set;
        }
        protected bool EnableAutoParameter
        {
            get;
            set;
        }
        #endregion

        protected void SetAutoParameter(PixelShader pixShader, VertexShader vtxShader)
        {
            if (pixShader != null)
            {
                pixShader.AutoSetParameters();
            }
            if (vtxShader != null)
            {
                pixShader.AutoSetParameters();
            }
        }

        protected abstract int begin();
        protected abstract void end();
        public abstract void BeginPass(int passId);
        public abstract void EndPass();

        public abstract void BeginShadowPass();
        public abstract void EndShadowPass();

        public int Begin()
        {
            if (!begun)
            {
                begun = true;

                return begin();
            }
            return -1;
        }

        public void End()
        {
            if (begun)
            {
                end();
                begun = false;
            }
        }

        protected virtual int beginInst()
        {
            throw new NotSupportedException();
        }
        protected virtual void endInst()
        {
            throw new NotSupportedException();
        }
        public virtual void BeginPassInst(int passId)
        {
            throw new NotSupportedException();
        }
        public virtual void EndPassInst()
        {
            throw new NotSupportedException();
        }

        public int BeginInst()
        {
            if (!begun)
            {
                begun = true;
                return beginInst();
            }
            return -1;
        }
        public void EndInst()
        {
            if (begun)
            {
                endInst();
                begun = false;
            }
        }


        public virtual void SetupInstancing(Material mat)
        {
            throw new NotSupportedException();
        }

        public abstract void Setup(Material mat, ref RenderOperation op);
        public abstract void SetupShadowPass(Material mat, ref RenderOperation op);

 
        #region IDisposable 成员
        public bool Disposed
        {
            get;
            private set;
        }
        protected abstract void Dispose(bool disposing);


        public void Dispose()
        {
            if (!Disposed)
            {
                Dispose(true);
                Disposed = true;
            }
            else
                throw new ObjectDisposedException(ToString());
        }

        #endregion

        ~Effect()
        {
            if (!Disposed)
                Dispose();
        }
    }
}
