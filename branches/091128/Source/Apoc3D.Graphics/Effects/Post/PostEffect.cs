using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Vfs;

namespace Apoc3D.Graphics.Effects
{
    public abstract class PostEffect
    {
        protected PixelShader pixShader;
        protected VertexShader vtxShader;

        RenderSystem renderSys;

        protected PostEffect(RenderSystem rs)
        {
            renderSys = rs;
        }

        public void SetTexture(string name, Texture tex)
        {
            pixShader.SetTexture(name, tex);
        }

        public void Begin()
        {
            renderSys.BindShader(vtxShader);
            renderSys.BindShader(pixShader);
        }

        public void End()
        {
            renderSys.BindShader((VertexShader)null);
            renderSys.BindShader((PixelShader)null);
        }

        #region Loading Shaders
        protected void LoadVertexShader(RenderSystem rs, ResourceLocation vs, Macro[] macros, string funcName)
        {
            ObjectFactory fac = rs.ObjectFactory;

            ContentStreamReader sr = new ContentStreamReader(vs);

            string code = sr.ReadToEnd();
            sr.Close();

            vtxShader = fac.CreateVertexShader(code, macros, IncludeHandler.Instance, "vs_2_0", funcName);

        }
        protected void LoadPixelShader(RenderSystem rs, ResourceLocation vs, Macro[] macros, string funcName)
        {
            ObjectFactory fac = rs.ObjectFactory;

            ContentStreamReader sr = new ContentStreamReader(vs);

            string code = sr.ReadToEnd();
            sr.Close();

            pixShader = fac.CreatePixelShader(code, macros, IncludeHandler.Instance, "ps_2_0", funcName);
        }
        #endregion
    }
}
