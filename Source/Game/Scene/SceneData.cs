using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;
using VirtualBicycle.Logic;
using VirtualBicycle.Logic.Traffic;
using VirtualBicycle.MathLib;

namespace VirtualBicycle.Scene
{
    /// <summary>
    ///  记录场景地形（全局）的参数
    /// </summary>
    public class TerrainSettings
    {
        #region 常量

        protected static readonly string HeightScaleTag = "HeightScale";
        protected static readonly string MaterialTag = "Material";

        #endregion

        #region 属性

        /// <summary>
        ///  获取地形的高度比例。实际高度为高度图中的像素乘以高度比例
        /// </summary>
        public float HeightScale
        {
            get;
            protected set;
        }

        /// <summary>
        ///  获取地形材质的环境光成分
        /// </summary>
        public Color4 MaterialAmbient
        {
            get;
            protected set;
        }

        /// <summary>
        ///  获取地形材质的漫射光成分
        /// </summary>
        public Color4 MaterialDiffuse
        {
            get;
            protected set;
        }

        /// <summary>
        ///  获取地形材质的镜面光成分
        /// </summary>
        public Color4 MaterialSpecular
        {
            get;
            protected set;
        }

        /// <summary>
        ///  获取地形材质的发射光成分
        /// </summary>
        public Color4 MaterialEmissive
        {
            get;
            protected set;
        }

        /// <summary>
        ///  
        /// </summary>
        public float MaterialPower
        {
            get;
            protected set;
        }
        #endregion

        #region 方法

        public void SetHeightScale(float value)
        {
            HeightScale = value;
        }

        public void SetMaterialAmbient(Color4 color)
        {
            MaterialAmbient = color;
        }

        public void SetMaterialDiffuse(Color4 color)
        {
            MaterialDiffuse = color;
        }

        public void SetMaterialSpecular(Color4 color)
        {
            MaterialSpecular = color;
        }

        public void SetMaterialEmissive(Color4 color)
        {
            MaterialEmissive = color;
        }

        public void SetMaterialPower(float value)
        {
            MaterialPower = value;
        }

        /// <summary>
        ///  从一个<see cref="BinaryDataReader"/>中读取数据
        /// </summary>
        /// <param name="data">数据</param>
        public virtual void ReadData(BinaryDataReader data)
        {
            HeightScale = data.GetDataSingle("HeightScale");

            ContentBinaryReader br = data.GetData("Material");

            Material mat;
            br.ReadMaterial(out mat);

            br.Close();

            this.MaterialAmbient = mat.Ambient;
            this.MaterialDiffuse = mat.Diffuse;
            this.MaterialEmissive = mat.Emissive;
            this.MaterialSpecular = mat.Specular;
            this.MaterialPower = mat.Power;
        }

        /// <summary>
        ///  把数据存入<see cref="BinaryDataReader"/>中
        /// </summary>
        /// <returns>返回保存数据的<see cref="BinaryDataReader"/></returns>
        public virtual BinaryDataWriter WriteData()
        {
            BinaryDataWriter data = new BinaryDataWriter();

            data.AddEntry(HeightScaleTag, HeightScale);

            ContentBinaryWriter bw = data.AddEntry(MaterialTag);

            Material mat = new Material();
            mat.Ambient = MaterialAmbient;
            mat.Diffuse = MaterialDiffuse;
            mat.Emissive = MaterialEmissive;
            mat.Specular = MaterialSpecular;
            mat.Power = MaterialPower;

            bw.Write(ref mat);

            bw.Close();

            return data;
        }

        #endregion

    }

    /// <summary>
    ///  场景数据（地图数据）
    /// </summary>
    public class SceneData : SceneDataBase
    {
        #region 构造函数

        public SceneData(Device device, InGameObjectManager mgr)
            : base(device, mgr)
        {

        }

        #endregion

        #region 静态方法

        /// <summary>
        ///  从场景文件中读取场景数据
        /// </summary>
        /// <param name="device"></param>
        /// <param name="mgr"></param>
        /// <param name="file">场景文件的路径</param>
        /// <param name="cbk">读取场景进度回调。当进度变化时会被调用。</param>
        /// <returns></returns>
        public static SceneData FromFile(Device device, InGameObjectManager mgr, string file, ProgressCallBack cbk)
        {
            return FromFile(device, mgr, new FileLocation(file), cbk);
        }

        /// <summary>
        ///  从场景文件中读取场景数据
        /// </summary>
        /// <param name="device"></param>
        /// <param name="mgr"></param>
        /// <param name="file">场景文件的路径</param>
        /// <param name="cbk">读取场景进度回调。当进度变化时会被调用。</param>
        /// <returns></returns>
        public static SceneData FromFile(Device device, InGameObjectManager mgr, FileLocation fl, ProgressCallBack cbk)
        {
            ContentBinaryReader br = new ContentBinaryReader(fl);

            if (br.ReadInt32() == FileId)
            {
                SceneData result = new SceneData(device, mgr);

                BinaryDataReader data = br.ReadBinaryData();

                result.ReadData(data, cbk);

                data.Close();

                return result;
            }

            br.Close();
            throw new DataFormatException();
        }

        #endregion
    }
}
