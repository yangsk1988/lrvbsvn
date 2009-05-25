using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SlimDX.Direct3D9;
using VirtualBicycle.CollisionModel;
using VirtualBicycle.Config;
using VirtualBicycle.Graphics;
using VirtualBicycle.Graphics.Effects;
using VirtualBicycle.IO;
using VirtualBicycle.Media;
using VirtualBicycle.Scene;

namespace VirtualBicycle
{
    public static class Engine
    {
        static bool showConsole;

        public static bool ShowConsole
        {
            get { return showConsole; }
            set
            {
                showConsole = value;

                if (value)
                {
                    EngineConsole.Instance.Show();
                }
                else
                {
                    EngineConsole.Instance.Hide();
                }
            }
        }

        public static void Release() 
        {
            EngineConsole.Instance.Close();

            TextureLibrary.Instance.Dispose();

        }

        public static void Initialize(Device device)
        {
            ShowConsole = true;
                   
            Cache.Initialize();

            StringTableManager.Initialize();
            StringTableManager.Instance.Register(new StringTableCsfFormat());

            ConfigurationManager.Initialize();
            ConfigurationManager.Instance.Register(new ConfigurationIniFormat());


            ImageManager.Initialize();
            ImageManager.Instance.RegisterImageFormat(new PngImageFormat());
            ImageManager.Instance.RegisterImageFormat(new TgaImageFormat());
            ImageManager.Instance.RegisterImageFormat(new DdsImageFormat());
            ImageManager.Instance.RegisterImageFormat(new BmpImageFormat());
            ImageManager.Instance.RegisterImageFormat(new JpegImageFormat());
            ImageManager.Instance.RegisterImageFormat(new PcxImageFormat());


            TextureManager.Initialize(128 * 1048576);
            TerrainTextureManager.Initialize(128 * 1048576);

            string filePath = Path.Combine(Paths.Configs, "textureLib.ini");
            FileLocation fl = FileSystem.Instance.Locate(filePath, FileLocateRules.Default);

            TextureLibrary.Initialize(device);
            TextureLibrary.Instance.LoadTextureSet(fl);

            ModelManager.Initialize();

            CollisionMeshManager.Initialize();
            HeightFieldManager.Initialize();

            EffectManager.Initialize(device);

        }

    }
}
