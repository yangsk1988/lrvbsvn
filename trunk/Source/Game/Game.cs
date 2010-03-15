using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Config;
using VirtualBicycle.ConfigModel;
using VirtualBicycle.Graphics.Effects;
using VirtualBicycle.Input;
using VirtualBicycle.IO;
using VirtualBicycle.Logic.Mod;
using VirtualBicycle.Sound;
using VirtualBicycle.UI;

namespace VirtualBicycle
{
    public class Game : GameBase
    {
        public struct CmdArg
        {
            public string name;
            public string param;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder(8);
                sb.Append("{ Name=");
                sb.Append(name);
                sb.Append(", Param=");
                sb.Append(param);
                sb.Append('}');

                return sb.ToString();
            }

            public static List<CmdArg> GetCmdArguments()
            {
                List<CmdArg> cmdArgs = new List<CmdArg>();

                string[] args = Environment.GetCommandLineArgs();
                cmdArgs.Capacity = args.Length;

                for (int i = 1; i < args.Length; i++)
                {
                    args[i] = args[i].Trim();

                    CmdArg ca;
                    int pos = args[i].IndexOf(" ");

                    if (pos == -1)
                    {
                        ca.param = null;
                        ca.name = args[i];
                    }
                    else
                    {
                        ca.param = args[i].Substring(pos).Trim();
                        ca.name = args[i].Substring(0, pos);
                    }
                    cmdArgs.Add(ca);
                }

                return cmdArgs;
            }
        }

        GameUI gameUI;
        FMOD.System sndSystem;
        bool initialized;

        public Device Device
        {
            get { return GraphicsDeviceManager.Direct3D9.Device; }
        }
        public GameUI GameUI
        {
            get { return gameUI; }
            set { gameUI = value; }
        }
        public World CurrentWorld
        {
            get;
            set;
        }

        bool DeviceCreated
        {
            get;
            set;
        }

        bool windowed;
        public bool Windowed
        {
            get { return windowed; }
            private set
            {
                if (DeviceCreated)
                {
                    if (value != GraphicsDeviceManager.IsWindowed) 
                    {
                        GraphicsDeviceManager.ToggleFullScreen();
                    }
                }
                windowed = value;
            }
        }


        void CreateD3D()
        {
            Rectangle rect = Screen.PrimaryScreen.Bounds;

            VideoConfigs vidCon = (VideoConfigs)BasicConfigs.Instance[VideoConfigFactory.VideoConfigName];
            int width = vidCon.ScreenWidth;
            int height = vidCon.ScreenHeight;

            Window.ClientSize = new Size(width, height);
            Window.Location = new Point((rect.Width - Window.Width) / 2, (rect.Height - Window.Height) / 2);

            DeviceSettings sets = new DeviceSettings();

            sets.AdapterOrdinal = 0;
            sets.BackBufferCount = 1;
            sets.BackBufferFormat = Format.X8B8G8R8;
            sets.BackBufferHeight = height;
            sets.BackBufferWidth = width;
            sets.DepthStencilFormat = Format.D24S8;
            sets.DeviceType = DeviceType.Software;
            sets.DeviceVersion = DeviceVersion.Direct3D9;
            sets.EnableVSync = false;
            sets.MultisampleType = MultisampleType.None;
            sets.Windowed = Windowed;
            sets.Multithreaded = true;

            GraphicsDeviceManager.ChangeDevice(sets);

            DeviceCreated = true;
        }

        public Game()
        {
            List<CmdArg> args = CmdArg.GetCmdArguments();

            for (int i = 0; i < args.Count; i++)
            {
                if (CaseInsensitiveStringComparer.Compare(args[i].name, "-resdir"))
                {
                    FileSystem.Instance.AddWorkingDir(args[i].param);
                }
                else if (CaseInsensitiveStringComparer.Compare(args[i].name, "-win"))
                {
                    Windowed = true;
                }
            }
            FileSystem.Instance.AddWorkingDir(Application.StartupPath);
            ConfigurationManager.Initialize();
            ConfigurationManager.Instance.Register(new ConfigurationIniFormat());

            BasicConfigs.Initialize();
            BasicConfigs.Instance.RegisterConfigType(AudioConfigFacotry.AudioConfigName, new AudioConfigFacotry());
            BasicConfigs.Instance.RegisterConfigType(VideoConfigFactory.VideoConfigName, new VideoConfigFactory());
            BasicConfigs.Instance.Load();

            CreateD3D();
            
        }

        protected internal override void Initialize()
        {
            if (!initialized)
            {
                Device device = Device;

                Engine.Initialize(device);
                if (!GraphicsDeviceManager.IsWindowed)
                {
                    Engine.ShowConsole = false;
                }

                EffectManager.Instance.LoadEffects();

                FileLocation fl = FileSystem.Instance.Locate("virtualbicycle.csf", FileLocateRules.Default);
                StringTableManager.Instance.LoadStringTable(fl);


                FMOD.Factory.System_Create(ref sndSystem);
                sndSystem.init(8, FMOD.INITFLAG.NORMAL, IntPtr.Zero);

                SerialPortInputProcessor seip = new SerialPortInputProcessor(InputManager.Instance);
                if (seip.IsValid)
                {
                    InputManager.Instance.Processor = seip;
                    EngineConsole.Instance.Write("使用串口输入", ConsoleMessageType.Information);
                    //InputManager.Instance.Processor_bak = new KeyboardInputProcessor(InputManager.Instance);
                }
                else
                {
                    InputManager.Instance.Processor = new KeyboardInputProcessor(InputManager.Instance);
                }

                Themes.Initialize(sndSystem, (AudioConfigs)BasicConfigs.Instance[AudioConfigFacotry.AudioConfigName]);


                LogicMod.Game = this;
                gameUI = new GameUI(device, this);

                GameLogicManager.Initialize();

                Window.Text = StringTableManager.StringTable["GUI:VirtualBicycle"];

                initialized = true;
            }
        }
        
        protected override void Draw(GameTime gameTime)
        {
            if (!IsExiting && initialized)
            {
                Device device = Device;

                device.SetTransform(TransformState.Projection, Matrix.Identity);
                device.SetTransform(TransformState.View, Matrix.Identity);
                device.SetTransform(TransformState.World, Matrix.Identity);

                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black.ToArgb(), 1, 0);

                device.BeginScene();

                if (CurrentWorld != null)
                {
                    CurrentWorld.Render();
                }
                gameUI.Render();

                device.EndScene();
            }

        }

        protected override void Update(GameTime gameTime)
        {
            if ((!IsExiting) && initialized)
            {
                float dt = gameTime.ElapsedGameTime;
                if (CurrentWorld != null)
                {
                    CurrentWorld.Update(dt);
                }
                gameUI.Update(dt);

                InputManager.Instance.Update(dt);

                if (Themes.Instance != null)
                {
                    Themes.Instance.Update(dt);
                }
            }
        }

        public override void Exit()
        {
            base.Exit();

            BasicConfigs.Instance.Save();

            GameLogicManager.Instance.GameFinalize();

            EffectManager.Instance.UnloadEffects();
            EffectManager.Instance.Dispose();
            Themes.Instance.Dispose();

            if (CurrentWorld != null)
            {
                CurrentWorld.Dispose();
            }

            GameUI.Dispose();

            Engine.Release();

            sndSystem.close();
            sndSystem.release();
            sndSystem = null;
        }

        public override void LoadUnmanagedResources()
        {
            UnmanagedResource.LoadAll();
        }
        public override void UnloadUnmanagedResources()
        {
            UnmanagedResource.UnloadAll();
        }
    }
}
