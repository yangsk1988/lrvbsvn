using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Graphics;
using VirtualBicycle.Ide.Designers;

namespace Plugin.ModelTools
{
    public static class PlgUtils
    {
        public static void DrawLight(Device dev, Vector3 lightPos)
        {
            Matrix lgtMat;
            Matrix.Translation(ref lightPos, out lgtMat);
            dev.SetTransform(TransformState.World, lgtMat);
            dev.SetRenderState(RenderState.Lighting, false);
            for (int i = 0; i < 4; i++)
            {
                dev.SetTexture(i, null);
            }
            GraphicsDevice.Instance.BallMesh.DrawSubset(0);
            dev.SetRenderState(RenderState.Lighting, true);

        }

        public static void SetMaterial(Device dev, EditableMeshMaterial mate)
        {
            dev.SetRenderState(RenderState.ZWriteEnable, !mate.IsTransparent);
            dev.SetRenderState<Cull>(RenderState.CullMode, mate.CullMode);

            dev.Material = mate.D3DMaterial;

            for (int j = 0; j < MaterialBase.MaxTexLayers; j++)
            {
                dev.SetTexture(j, mate.GetTexture(j));
            }
        }

        public static Bitmap Texture2Bitmap(Texture value)
        {
            throw new NotImplementedException();
        }

       
    }
}
