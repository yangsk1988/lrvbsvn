using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SlimDX.Direct3D9;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;
using VirtualBicycle.Scene;

namespace VirtualBicycle.UI
{
    /// <summary>
    /// 一张地图关卡的信息
    /// </summary>
    public struct LevelInfo
    {
        public string Name;
        public string Filename;

        public string Detail;
        public int Size;
        public Texture Preview;

        public void Load(Device dev, FileLocation fl)
        {
            ContentBinaryReader br = new ContentBinaryReader(fl);

            if (br.ReadInt32() == SceneDataBase.FileId)
            {
                SceneDataBase sceDataInfo = new SceneDataBase(null, null);

                BinaryDataReader data = br.ReadBinaryData();

                sceDataInfo.ReadInformation(data, null);

                data.Close();

                this.Filename = Path.GetFileName(fl.Path);

                this.Name = sceDataInfo.Name + "(" + Path.GetFileNameWithoutExtension(fl.Path) + ")";
                if (string.IsNullOrEmpty(sceDataInfo.Name))
                {
                    this.Name = StringTableManager.StringTable["GUI:MapNoName"] + this.Name;
                }

                this.Detail = sceDataInfo.Description;
                this.Size = sceDataInfo.SceneSize;


                sceDataInfo.Dispose();
            }
            br.Close();

            string path = Path.GetDirectoryName(fl.Path);
            string prevPath = Path.Combine(path, Path.GetFileNameWithoutExtension(fl.Path) + ".png");

            if (File.Exists(prevPath))
            {
                Preview = TextureLoader.LoadUITexture(dev, new FileLocation(prevPath));
            }
        }

        
    }
}
