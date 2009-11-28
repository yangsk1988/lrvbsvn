using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.MathLib;

namespace Apoc3D.Graphics.Effects
{
    public static class EffectParams
    {
        public static float TerrainHeightScale
        {
            get;
            set;
        }

        public static ShadowMap ShadowMap
        {
            get;
            set;
        }

        public static Atmosphere Atmosphere
        {
            get;
            set;
        }

        public static ICamera CurrentCamera
        {
            get;
            set;
        }
    }
}
