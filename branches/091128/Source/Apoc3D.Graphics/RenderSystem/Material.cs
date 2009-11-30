﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Vfs;

namespace Apoc3D.Graphics
{
    [Flags()]
    public enum MaterialFlags
    {
        None = 0,
        RemapColor = 1
    }

    public abstract class MaterialBase
    {
        #region Constants

        /// <summary>
        ///  获取最大允许的纹理层数
        /// </summary>
        public const int MaxTexLayers = 16;

        static readonly string IsTransparentTag = "IsTransparent";
        static readonly string CullModeTag = "CullMode";
        #endregion

        #region Properties

        public CullMode CullMode
        {
            get;
            set;
        }

        public bool IsTransparent
        {
            get;
            set;
        }

        #endregion

        #region Methods

        protected virtual void ReadData(BinaryDataReader data)
        {
            CullMode = (CullMode)data.GetDataInt32(CullModeTag, 0);
            IsTransparent = data.GetDataBool(IsTransparentTag, false);
        }

        protected virtual void WriteData(BinaryDataWriter data)
        {
            data.AddEntry(CullModeTag, (int)CullMode);
            data.AddEntry(IsTransparentTag, IsTransparent);
        }

        #endregion
    }

    /// <summary>
    ///  定义材质的基本结构
    /// </summary>
    /// <typeparam name="TexType"></typeparam>
    public abstract class MeshMaterialBase<TexType> : MaterialBase, IDisposable
        where TexType : class
    {
        #region Constants
        static readonly string MaterialColorTag = "MaterialColor";

        static readonly string IsTexEmbededTag = "IsEmbeded";
        static readonly string MaterialFlagTag = "Flags";
        static readonly string HasTextureTag = "HasTexture";
        static readonly string TextureTag = "Texture";
        static readonly string EffectTag = "Effect";
        #endregion

        #region Field
        protected Color4F ambient;
        protected Color4F diffuse;
        protected Color4F specular;
        protected Color4F emissive;
        protected float power;

        protected string[] textureFiles = new string[MaxTexLayers];

        protected bool[] texEmbeded = new bool[MaxTexLayers];

        protected TexType[] textures = new TexType[MaxTexLayers];

        string effectName;

        bool disposed;

        #endregion

        #region 属性


        public Color4F Ambient
        {
            get { return ambient; }
            set { ambient = value; }
        }

        public Color4F Diffuse
        {
            get { return diffuse; }
            set { diffuse = value; }
        }

        public Color4F Specular
        {
            get { return specular; }
            set { specular = value; }
        }

        public Color4F Emissive
        {
            get { return emissive; }
            set { emissive = value; }
        }

        public float Power
        {
            get { return power; }
            set { power = value; }
        }

        public Effect Effect
        {
            get;
            protected set;
        }

        public int BatchIndex
        {
            get;
            protected set;
        }

        public string GetTextureFile(int idx)
        {
            return textureFiles[idx];
        }
        public void SetTextureFile(int idx, string filePath)
        {
            textureFiles[idx] = filePath;
        }

        public TexType GetTexture(int idx)
        {
            return textures[idx];
        }
        public void SetTexture(int idx, TexType tex)
        {
            textures[idx] = tex;
        }


        public void SetEffect(Effect eff)
        {
            Effect = eff;
        }

        public void SetBatchIndex(int index)
        {
            BatchIndex = index;
        }

        public MaterialFlags Flags
        {
            get;
            set;
        }
        #endregion

        #region Abstract Methods
        protected abstract TexType LoadTexture(ContentBinaryReader br, bool isEmbeded, int index);
        protected abstract void DestroyTexture(TexType tex);

        protected abstract void SaveTexture(ContentBinaryWriter bw, TexType tex, bool isEmbeded, int index);
        protected abstract Effect LoadEffect(string name);
        #endregion

        #region Methods

        protected override void ReadData(BinaryDataReader data)
        {
            base.ReadData(data);

            ContentBinaryReader br;

            Flags = (MaterialFlags)data.GetDataInt32(MaterialFlagTag);

            br = data.GetData(MaterialColorTag);
            ambient.Alpha = br.ReadSingle();
            ambient.Red = br.ReadSingle();
            ambient.Green = br.ReadSingle();
            ambient.Blue = br.ReadSingle();

            diffuse.Alpha = br.ReadSingle();
            diffuse.Red = br.ReadSingle();
            diffuse.Green = br.ReadSingle();
            diffuse.Blue = br.ReadSingle();

            specular.Alpha = br.ReadSingle();
            specular.Red = br.ReadSingle();
            specular.Green = br.ReadSingle();
            specular.Blue = br.ReadSingle();

            emissive.Alpha = br.ReadSingle();
            emissive.Red = br.ReadSingle();
            emissive.Green = br.ReadSingle();
            emissive.Blue = br.ReadSingle();

            power = br.ReadSingle();

            br.Close();


            br = data.GetData(EffectTag);
            effectName = br.ReadStringUnicode();
            if (effectName.Length == 0)
                effectName = StandardEffectFactory.Name;
            Effect = LoadEffect(effectName);

            br.Close();

            br = data.GetData(IsTexEmbededTag);
            for (int i = 0; i < MaxTexLayers; i++)
            {
                texEmbeded[i] = br.ReadBoolean();
            }
            br.Close();

            bool[] hasTexture = new bool[MaxTexLayers];
            br = data.GetData(HasTextureTag);
            for (int i = 0; i < MaxTexLayers; i++)
            {
                hasTexture[i] = br.ReadBoolean();
            }
            br.Close();


            for (int i = 0; i < MaxTexLayers; i++)
            {
                if (hasTexture[i])
                {
                    br = data.GetData(TextureTag + i.ToString());
                    textures[i] = LoadTexture(br, texEmbeded[i], i);
                    br.Close();
                }
            }
        }

        protected override void WriteData(BinaryDataWriter data)
        {
            base.WriteData(data);


            data.AddEntry(MaterialFlagTag, (int)Flags);

            ContentBinaryWriter bw;

            bw = data.AddEntry(EffectTag);
            //if (Effect == null)
            //{
            bw.WriteStringUnicode(effectName);
            //}
            //else
            //{
            //    bw.WriteStringUnicode(Effect.Name);
            //}
            bw.Close();

            bw = data.AddEntry(MaterialColorTag);
            bw.Write(ambient.Alpha);
            bw.Write(ambient.Red);
            bw.Write(ambient.Green);
            bw.Write(ambient.Blue);

            bw.Write(diffuse.Alpha);
            bw.Write(diffuse.Red);
            bw.Write(diffuse.Green);
            bw.Write(diffuse.Blue);

            bw.Write(specular.Alpha);
            bw.Write(specular.Red);
            bw.Write(specular.Green);
            bw.Write(specular.Blue);

            bw.Write(emissive.Alpha);
            bw.Write(emissive.Red);
            bw.Write(emissive.Green);
            bw.Write(emissive.Blue);

            bw.Write(power);

            bw.Close();


            bw = data.AddEntry(IsTexEmbededTag);
            for (int i = 0; i < MaxTexLayers; i++)
            {
                bw.Write(texEmbeded[i]);
            }
            bw.Close();

            bw = data.AddEntry(HasTextureTag);
            for (int i = 0; i < MaxTexLayers; i++)
            {
                bw.Write(textures[i] != null || !string.IsNullOrEmpty(textureFiles[i]));
            }
            bw.Close();

            for (int i = 0; i < MaxTexLayers; i++)
            {
                if (textures[i] != null || !string.IsNullOrEmpty(textureFiles[i]))
                {
                    bw = data.AddEntry(TextureTag + i.ToString());
                    SaveTexture(bw, textures[i], texEmbeded[i], i);
                    bw.Close();
                }
            }
        }

        #endregion

        #region IDisposable 成员

        public bool Disposed
        {
            get { return disposed; }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                if (textures != null)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (textures[i] != null)
                        {
                            DestroyTexture(textures[i]);
                            textures[i] = null;
                        }
                    }
                    textures = null;
                }
                disposed = true;
            }
            else
                throw new ObjectDisposedException(ToString());
        }

        #endregion

        ~MeshMaterialBase()
        {
            if (!disposed)
            {
                Dispose();
            }
        }

    }

    /// <summary>
    ///  材质
    /// </summary>
    public class Material : MeshMaterialBase<Texture>, IComparable<Material>
    {
        #region Constants

        public static Material DefaultMaterial
        {
            get;
            private set;
        }

        #endregion

        #region 构造函数
        static Material()
        {
            DefaultMaterial = new Material(null);

            Color4F clr;
            clr.Alpha = 1;
            clr.Blue = 1;
            clr.Green = 1;
            clr.Red = 1;

            DefaultMaterial.Ambient = clr;
            DefaultMaterial.Diffuse = clr;
            DefaultMaterial.Power = 0;
            clr.Alpha = 0;
            clr.Red = 0;
            clr.Green = 0;
            clr.Blue = 0;
            DefaultMaterial.Emissive = clr;
            DefaultMaterial.Specular = clr;

            DefaultMaterial.CullMode = CullMode.None;
        }

        public Material(RenderSystem dev)
        {
            device = dev;
        }
        #endregion

        #region 字段

        protected RenderSystem device;
        #endregion

        #region IComparable<MeshMaterial> 成员

        public int CompareTo(Material other)
        {
            return this.GetHashCode().CompareTo(other.GetHashCode());
        }

        #endregion

        #region 方法

        /// <summary>
        ///  重写以适应不同环境下的使用
        /// </summary>
        Texture LoadTexture(string fileName)
        {
            fileName = fileName.Trim();
            if (!string.IsNullOrEmpty(fileName))
            {
                FileLocation fl = FileSystem.Instance.TryLocate(fileName, FileLocateRules.Model);
                if (fl != null)
                {
                    return TextureManager.Instance.CreateInstance(device, fl);
                }
                else
                {
                    EngineConsole.Instance.Write("找不到纹理：" + fileName, ConsoleMessageType.Warning);
                    return null;
                }
            }
            return null;
        }
        protected override Texture LoadTexture(ContentBinaryReader br, bool isEmbeded, int index)
        {
            if (isEmbeded)
            {
                return new GameTexture(Texture.FromStream(device, br.BaseStream, Usage.None, Pool.Managed));
            }
            else
            {
                string fn = br.ReadStringUnicode();
                textureFiles[index] = fn;

                return LoadTexture(fn);
            }
        }

        protected override void SaveTexture(ContentBinaryWriter bw, Texture tex, bool isEmbeded, int index)
        {
            if (isEmbeded)
            {
                throw new NotSupportedException();
            }
            bw.WriteStringUnicode(textureFiles[index]);
        }

        protected override Effect LoadEffect(string name)
        {
            return EffectManager.Instance.GetModelEffect(name);
        }
        public static Material FromBinary(RenderSystem dev, BinaryDataReader data)
        {
            Material res = new Material(dev);

            res.ReadData(data);

            return res;
        }
        public static BinaryDataWriter ToBinary(Material mat)
        {
            BinaryDataWriter data = new BinaryDataWriter();
            mat.WriteData(data);

            return data;
        }

        protected override void DestroyTexture(Texture tex)
        {
            TextureManager.Instance.DestroyInstance(tex);
        }

        #endregion
    }
}