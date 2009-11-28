using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Vfs;

namespace Apoc3D.Graphics.Effects
{
    class Composite : PostEffect
    {
        public Composite(RenderSystem rs)
            : base(rs)
        {
            string filePath = FileSystem.CombinePath(Paths.Effects, "composite.ps");
            FileLocation fl = FileSystem.Instance.Locate(filePath, FileLocateRules.Default);

            LoadPixelShader(rs, fl, null, "main");
        }
    }
}
