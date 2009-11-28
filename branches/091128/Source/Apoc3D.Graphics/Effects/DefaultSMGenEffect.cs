using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Vfs;

namespace Apoc3D.Graphics.Effects
{
    public class DefaultSMGenEffect : Effect
    {
        #region Not Supported

        protected override int begin()
        {
            throw new NotSupportedException();
        }

        protected override void end()
        {
            throw new NotSupportedException();
        }

        public override void BeginPass(int passId)
        {
            throw new NotSupportedException();
        }

        public override void EndPass()
        {
            throw new NotSupportedException();
        }

        public override void Setup(Material mat, ref RenderOperation op)
        {
            throw new NotSupportedException();
        }

        #endregion

        RenderSystem renderSys;
        //Effect effect;
        PixelShader pixShader;
        VertexShader vtxShader;

        //EffectHandle ehMVP;

        public DefaultSMGenEffect(RenderSystem rs)
            : base(false, "DefaultSMGenEffect")
        {
            EnableAutoParameter = true;

            FileLocation fl = FileSystem.Instance.Locate(FileSystem.CombinePath(Paths.Effects, "HardwareShadowMap.vs"), FileLocateRules.Default);

            LoadVertexShader(rs, fl, null, "main");

            fl = FileSystem.Instance.Locate(FileSystem.CombinePath(Paths.Effects, "HardwareShadowMap.ps"), FileLocateRules.Default);

            LoadPixelShader(rs, fl, null, "main");

            renderSys = rs;
            //string err;
            //string code = sr.ReadToEnd();
            //effect = Effect.FromString(device, code, null, IncludeHandler.Instance, null, ShaderFlags.OptimizationLevel3, null, out err);
            //sr.Close();

            //effect.Technique = new EffectHandle("GenerateShadowMap");

            //ehMVP = new EffectHandle("mvp");
        }

        public override void BeginShadowPass()
        {
            renderSys.BindShader(vtxShader);
            renderSys.BindShader(pixShader);

            //effect.Begin(FX.DoNotSaveSamplerState | FX.DoNotSaveShaderState | FX.DoNotSaveState);
            //effect.BeginPass(0);
        }

        public override void EndShadowPass()
        {
            //effect.EndPass();
            //effect.End();
        }



        public override void SetupShadowPass(Material mat, ref RenderOperation op)
        {
            //effect.SetValue(ehMVP, op.Transformation * EffectParams.ShadowMap.ViewProj);
            
            //effect.CommitChanges();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) 
            {
                vtxShader.Dispose();
                pixShader.Dispose();
            }
            //if (disposing)
            //{
            //    effect.Dispose();
            //    ehMVP.Dispose();
            //}
            //ehMVP = null;
            //effect = null;
        }
    }
}
