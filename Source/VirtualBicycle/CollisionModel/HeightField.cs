using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SlimDX.Direct3D9;
using VirtualBicycle.Graphics;
using VirtualBicycle.Scene;

namespace VirtualBicycle.CollisionModel
{
    public unsafe class HeightField : Resource
    {
        HeightField refHF;
        GameTexture dispTexture;

        float[] heightData;
        public float[] HeightData
        {
            get
            {
                if (IsResourceEntity)
                {
                    Use();
                    return heightData;
                }
                return refHF.HeightData;
            }
        }

        public static string GetHashString(GameTexture dispTexture)
        {
            return "[HeighField Shape]" + dispTexture.HashString;
        }

        public HeightField(HeightField heightfield)
            : base(HeightFieldManager.Instance, heightfield)
        {
            this.refHF = heightfield;

            this.Width = heightfield.Width;
            this.Height = heightfield.Height;
        }

        public HeightField(GameTexture dispTexture)
            : base(HeightFieldManager.Instance, GetHashString(dispTexture))
        {
            this.dispTexture = dispTexture;

            Texture tex = dispTexture.GetTexture;

            SurfaceDescription desc = tex.GetLevelDescription(0);

            this.Width = desc.Width;
            this.Height = desc.Height;
        }

        public override void ReadCacheData(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);

            heightData = new float[Width * Height];

            for (int i = 0; i < heightData.Length; i++)
            {
                heightData[i] = br.ReadSingle();
            }

            br.Close();
        }

        public override void WriteCacheData(Stream stream)
        {
            Use();
            BinaryWriter bw = new BinaryWriter(stream);

            for (int i = 0; i < heightData.Length; i++)
            {
                bw.Write(heightData[i]);
            }

            bw.Close();
        }

        public override int GetSize()
        {
            return Width * Height * sizeof(float);
        }

        public int Width
        {
            get;
            private set;
        }
        public int Height
        {
            get;
            private set;
        }

        protected override void load()
        {
            heightData = new float[Width * Height];

            Texture tex = dispTexture.GetTexture;
            float* src = (float*)tex.LockRectangle(0, LockFlags.ReadOnly).Data.DataPointer.ToPointer();
            fixed (float* dataPtr = &heightData[0])
            {
                Memory.Copy(src, dataPtr, GetSize());
            }
            tex.UnlockRectangle(0);

        }

        protected override void unload()
        {
            heightData = null;
        }
    }
}
