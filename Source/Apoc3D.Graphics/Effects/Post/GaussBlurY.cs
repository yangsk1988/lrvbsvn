﻿using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Vfs;

namespace Apoc3D.Graphics.Effects.Post
{
    class GaussBlurY : PostEffect
    {
        public GaussBlurY(RenderSystem rs)
            : base(rs)
        {
            string filePath = FileSystem.CombinePath(Paths.Effects, "gaussBlurY.ps");
            FileLocation fl = FileSystem.Instance.Locate(filePath, FileLocateRules.Default);

            LoadPixelShader(rs, fl, null, "main");
        }
    }
}
