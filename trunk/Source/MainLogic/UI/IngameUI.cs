using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using MainLogic;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Graphics;
using VirtualBicycle.Input;
using VirtualBicycle.IO;
using VirtualBicycle.Logic;
using VirtualBicycle.Logic.Mod;
using VirtualBicycle.MathLib;
using VirtualBicycle.Physics.Dynamics;
using VirtualBicycle.Sound;

namespace VirtualBicycle.UI
{
    public unsafe class PowerGraph : UIComponent, IDisposable, IUnmanagedResource
    {

        struct InstData
        {
            public float Distance;
            public float HeightRatio;

            public static int Size 
            {
                get { return sizeof(float) * 2; }
            }
        }
        struct PBVertex
        {
            public Vector3 pos;
            public Vector2 tex1;

            public static VertexFormat Format
            {
                get { return VertexFormat.Position | VertexFormat.Texture1; }
            }

            public static int Size
            {
                get { return Vector3.SizeInBytes + Vector2.SizeInBytes; }
            }
        }

        public const int BarWidth = 10;
        public const int BarHeight = 125;
        public const int BarCount = 50;

        #region 字段
        GameMainLogic logic;
        IngameUI ingameUI;

        Device device;
        VertexBuffer powerBar;
        VertexBuffer instanceData;
        IndexBuffer idxBuf;

        VertexElement[] elements;
        VertexDeclaration vtxDecl;

        Effect effect;
        //EffectHandle ehBarHeights;
        EffectHandle ehBarPosition;
        EffectHandle ehBarTex;
        EffectHandle ehProj;

        Texture barTex;

        float[] powers = new float[BarCount];

        int barUpdateFrame;

        #endregion

        public Bicycle CurrectBicycle
        {
            get;
            set;
        }
        public PowerGraph(GameMainLogic logic, IngameUI ingameUI, Device device, Game game)
            : base(game)
        {
            this.logic = logic;
            this.ingameUI = ingameUI;

            this.device = device;

            FileLocation fl = FileSystem.Instance.Locate(FileSystem.CombinePath(Paths.Effects, "powerBar.fx"), FileLocateRules.Default);
            ContentStreamReader sr = new ContentStreamReader(fl);

            string err;
            string code = sr.ReadToEnd();
            effect = Effect.FromString(device, code, null, null, null, ShaderFlags.OptimizationLevel3, null, out err);
            sr.Close();

            effect.Technique = new EffectHandle("PowerBar");

            //ehBarHeights = new EffectHandle("barHeights");
            ehBarPosition = new EffectHandle("barPosition");
            ehBarTex = new EffectHandle("barTex");
            ehProj = new EffectHandle("proj");
        }

        protected override void render()
        {
         
            Size s = Game.Window.ClientSize;
            effect.SetValue<Matrix>(ehProj, Matrix.OrthoOffCenterRH(0, s.Width, s.Height, 0, 0, 10));
            effect.SetValue<Vector2>(ehBarPosition, 
                new Vector2(
                    (s.Width - (BarWidth + 1) * BarCount) / 2,
                    s.Height - BarHeight - 30));

            effect.SetTexture(ehBarTex, barTex);
            effect.CommitChanges();

            effect.Begin(FX.DoNotSaveState | FX.DoNotSaveSamplerState | FX.DoNotSaveShaderState);

            effect.BeginPass(0);

            device.SetRenderState<Cull>(RenderState.CullMode, Cull.None);
            device.VertexDeclaration = vtxDecl;
            device.VertexFormat = VertexFormat.None;
            device.Indices = idxBuf;

            device.SetStreamSource(0, powerBar, 0, PBVertex.Size);
            device.SetStreamSourceFrequency(0, BarCount, StreamSource.IndexedData);

            device.SetStreamSource(1, instanceData, 0, InstData.Size);
            device.SetStreamSourceFrequency(1, 1, StreamSource.InstanceData);

            device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, 4, 0, 2);

            effect.EndPass();
            effect.End();
        }

        protected override void render(Sprite sprite)
        {

        }

        protected override void update(float dt)
        {
            barUpdateFrame++;

            if (barUpdateFrame == 1)
            {
                for (int i = 1; i < BarCount; i++)
                {
                    powers[i - 1] = powers[i];
                }

                powers[BarCount - 1] += CurrectBicycle.Power;
                powers[BarCount - 1] *= 0.5f;

                barUpdateFrame = 0;
            }
            else 
            {
                powers[BarCount - 1] = CurrectBicycle.Power;

            }

            InstData* dst = (InstData*)instanceData.Lock(0, 0, LockFlags.None).DataPointer;

            for (int i = 0; i < BarCount; i++) 
            {
                dst->HeightRatio = Math.Min(powers[i] / 600f, 1);
                dst++;
            }

            instanceData.Unlock();
        }


        #region IDisposable 成员

        public bool Disposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                effect.Dispose();

                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion

        protected override void load()
        {
            elements = new VertexElement[5];
            elements[0] = new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0);
            elements[1] = new VertexElement(0, sizeof(float) * 3, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0);
            elements[2] = new VertexElement(1, 0, DeclarationType.Float1, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 1);
            elements[3] = new VertexElement(1, sizeof(float), DeclarationType.Float1, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 2);

            elements[4] = VertexElement.VertexDeclarationEnd;

            vtxDecl = new VertexDeclaration(device, elements);


            powerBar = new VertexBuffer(device, PBVertex.Size * 4, Usage.None, PBVertex.Format, Pool.Managed);

            PBVertex* ptr = (PBVertex*)powerBar.Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

            //Rectangle progArea = new Rectangle(0, 0, 512, 100);
            //ptr[0].dummy = 1.0f;
            ptr[0].pos = new Vector3(0, 0, 0);
            ptr[0].tex1 = new Vector2(0, 0);

            //ptr[1].dummy = 1.0f;
            ptr[1].pos = new Vector3(0, BarHeight, 0);
            ptr[1].tex1 = new Vector2(0, 1);

            //ptr[2].dummy = 1.0f;
            ptr[2].pos = new Vector3(BarWidth, 0, 0);
            ptr[2].tex1 = new Vector2(1, 0);

            //ptr[3].dummy = 1.0f;
            ptr[3].pos = new Vector3(BarWidth, BarHeight, 0);
            ptr[3].tex1 = new Vector2(1, 1);


            powerBar.Unlock();

            idxBuf = new IndexBuffer(device, sizeof(int) * 4, Usage.None, Pool.Managed, false);
            int* idst = (int*)idxBuf.Lock(0, 0, LockFlags.None).DataPointer;
            idst[0] = 0;
            idst[1] = 1;
            idst[2] = 2;
            idst[3] = 3;
            idxBuf.Unlock();

            FileLocation fl = FileSystem.Instance.Locate(Path.Combine(Paths.DataUI, "PowerBar.png"), FileLocateRules.Default);
            barTex = TextureLoader.LoadUITexture(device, fl);

            LoadUnmanagedResources();
        }

        protected override void unload()
        {

        }

        #region IUnmanagedResource 成员

        public void LoadUnmanagedResources()
        {
            instanceData = new VertexBuffer(device, InstData.Size * BarCount, Usage.Dynamic, VertexFormat.None, Pool.Default);
            InstData* dst = (InstData*)instanceData.Lock(0, 0, LockFlags.None).DataPointer;

            for (int i = 0; i < BarCount; i++)
            {
                dst->Distance = (BarWidth + 1) * i;
                dst++;
            }

            instanceData.Unlock();
        }

        public void UnloadUnmanagedResources()
        {
            instanceData.Dispose();
        }

        #endregion
    }

    public class StateIndicator : UIComponent
    {
        static readonly string UIFontName = "微软雅黑";

        MenuPic rightTop;

        DrawTextBox timeTextBox;
        DrawTextBox SpeedTextBox;
        DrawTextBox upRankTextBox;
        DrawTextBox downRankTextBox;

        GameMainLogic logic;
        IngameUI ingameUI;

        float timeElasped;

        float velTimeElasped;
        float velocity;
        Queue<float> qVelocity = new Queue<float>();

        public StateIndicator(GameMainLogic logic, IngameUI ingameUI, Device device, Game game)
            : base(game)
        {
            this.logic = logic;
            this.ingameUI = ingameUI;

            this.rightTop = new MenuPic(game, "InGameRightTop.png", "In Game Right Top");
            MenuPicDrawPara rightTopPara;

            rightTopPara.PosX = 1024f - rightTop.Width / 2 - 20f;
            rightTopPara.PosY = 0f + rightTop.Height / 2 + 20f;
            rightTopPara.Alpha = 0.5f;
            rightTopPara.desiredWidth = rightTop.Width;
            rightTopPara.desiredHeight = rightTop.Height;

            this.rightTop.firstDrawPara = rightTopPara;
            this.rightTop.nextDrawPara = MenuPicDrawPara.Zero;
            this.rightTop.SetCurrentPara(0f);

            int posX;
            int posY;
            int fontWidth;
            int fontHeight;

            System.Drawing.Rectangle timeRect;
            System.Drawing.Rectangle speedRect;
            System.Drawing.Rectangle upRankRect;
            System.Drawing.Rectangle downRankRect;

            posX = (int)(rightTopPara.PosX - rightTopPara.desiredWidth / 2 + 178f);
            posY = (int)(rightTopPara.PosY - rightTopPara.desiredHeight / 2 + 13f);
            fontWidth = 330 - 178;
            fontHeight = 46 - 13;
            timeRect = new Rectangle(posX, posY, fontWidth, fontHeight);

            posX = (int)(rightTopPara.PosX - rightTopPara.desiredWidth / 2 + 182f);
            posY = (int)(rightTopPara.PosY - rightTopPara.desiredHeight / 2 + 74f);
            fontWidth = 271 - 182;
            fontHeight = 111 - 74;
            speedRect = new Rectangle(posX, posY, fontWidth, fontHeight);

            posX = (int)(rightTopPara.PosX - rightTopPara.desiredWidth / 2 + 35f);
            posY = (int)(rightTopPara.PosY - rightTopPara.desiredHeight / 2 + 10f);
            fontWidth = 88 - 36;
            fontHeight = 54 - 10;
            upRankRect = new Rectangle(posX, posY, fontWidth, fontHeight);

            posX = (int)(rightTopPara.PosX - rightTopPara.desiredWidth / 2 + 35f);
            posY = (int)(rightTopPara.PosY - rightTopPara.desiredHeight / 2 + 72f);
            fontWidth = 84 - 35;
            fontHeight = 119 - 72;
            downRankRect = new Rectangle(posX, posY, fontWidth, fontHeight);

            timeTextBox = new DrawTextBox(game, device, "", UIFontName, 24, System.Drawing.Color.White, timeRect);
            upRankTextBox = new DrawTextBox(game, device, "", UIFontName, 30, System.Drawing.Color.White, upRankRect);
            downRankTextBox = new DrawTextBox(game, device, "", UIFontName, 30, System.Drawing.Color.White, downRankRect);
            SpeedTextBox = new DrawTextBox(game, device, "", UIFontName, 24, System.Drawing.Color.White, speedRect);
        }

        public bool TimerStarted
        {
            get;
            set;
        }

        public Bicycle CurrectBicycle
        {
            get;
            set;
        }

        protected override void render(Sprite sprite)
        {
            rightTop.Render(sprite);
            DrawText(sprite);
        }

        private void DrawText(Sprite sprite)
        {
            TimeSpan time = TimeSpan.FromMilliseconds(timeElasped * 1000);
            string timeText = time.Hours.ToString("D2") + ":" +
                time.Minutes.ToString("D2") + ":" +
                time.Seconds.ToString("D2");

            float v = CurrectBicycle.RigidBody.LinearVelocity.Length();
            qVelocity.Enqueue(v);


            float v2;
            if (qVelocity.Count > 30)
            {
                qVelocity.Dequeue();
            }
            if (qVelocity.Count > 0)
            {
                v2 = 0;
                foreach (float lean in qVelocity)
                {
                    v2 += lean;
                }
                v2 /= qVelocity.Count;
            }
            else
            {
                v2 = v;
            }

            if (velTimeElasped > 0.5f)
            {
                velocity = v2;

                velTimeElasped = 0;
            }

            string speedText = (velocity * 3600 / 1000).ToString("F01");
            string upRankText = CurrectBicycle.Rank.ToString();
            string downRankText = BicycleManager.BicycleCount.ToString();
            timeTextBox.Render(sprite, timeText, true);
            SpeedTextBox.Render(sprite, speedText, true);
            upRankTextBox.Render(sprite, upRankText, true);
            downRankTextBox.Render(sprite, downRankText, true);
        }

        protected override void update(float dt)
        {
            if (TimerStarted)
            {
                timeElasped += dt;
            }
            velTimeElasped += dt;
        }

        protected override void render()
        {

        }

        protected override void load()
        {

        }

        protected override void unload()
        {

        }
    }

    public class IngameMenu : UIComponent
    {
        static readonly string UIFontName = "微软雅黑";

        Device device;
        GameUI gameUI;
        GameMainLogic logic;
        World world;
        IngameUI ingameUI;

        MenuPic picBackground;
        List<MenuPic> picSelectIcons;
        List<MenuPicDrawPara> picInitPara;
        MenuPic iconBg;
        DrawTextBox textBox;
        int curIndex = 0;

        public IngameMenu(Game game, World world, GameMainLogic logic, GameUI gameUI, IngameUI ingameUI)
            : base(game)
        {
            this.device = game.Device;
            this.world = world;
            this.gameUI = gameUI;
            this.logic = logic;

            this.ingameUI = ingameUI;
        }

        #region Update
        bool isAnimation = false;
        bool isMoveLeft = true;
        float usedAnimationTime = 0f;
        float curTime = 0;

        private void updateIconBackground()
        {
            const float iconAnimationTime = 0.5f;
            float time = curTime - (int)(curTime / iconAnimationTime) * iconAnimationTime;
            float dPara = time / iconAnimationTime - 0.5f;
            iconBg.SetCurrentPara(dPara);
        }

        protected override void update(float dt)
        {
            curTime += dt;
            updateIconBackground();
            const float animationTime = 0.2f;
            if (isAnimation)
            {
                int dIndex = curIndex - 2;
                usedAnimationTime += dt;
                if (usedAnimationTime > animationTime)
                {
                    usedAnimationTime = animationTime + 1e-7f;
                }

                for (int i = curIndex - 2; i <= curIndex + 2; i++)
                {
                    if ((i >= 0) && (i < picSelectIcons.Count))
                    {
                        int next;
                        //考虑往左移的情况
                        if (isMoveLeft)
                        {
                            //求得目标的参数
                            next = i - dIndex - 1;
                        }
                        else
                        {
                            next = i - dIndex + 1;
                        }

                        if ((next >= 0) && (next <= 4))
                        {
                            float dPara = usedAnimationTime / animationTime;
                            picSelectIcons[i].nextDrawPara = picInitPara[next];
                            picSelectIcons[i].SetCurrentPara(dPara);
                        }
                    }
                }

                //如果超过了动画时间,则动画播放结束
                if (usedAnimationTime > animationTime)
                {
                    isAnimation = false;
                    if (isMoveLeft)
                    {
                        curIndex++;
                    }
                    else
                    {
                        curIndex--;
                    }
                }
            }

            if (world.Disposed) 
            {
                gameUI.Pop();
                Game.CurrentWorld = null;
            }
        }
        #endregion

        private void DrawText(Sprite sprite)
        {
            textBox.Render(sprite, picSelectIcons[curIndex].Detail, true);
        }

        protected override void render()
        {
            if (world.IsValid)
            {
 
            }
        }
        protected override void render(Sprite sprite)
        {
            if (world.IsValid)
            {
                picBackground.Render(sprite);

                for (int i = 0; i < picSelectIcons.Count; i++)
                {
                    picSelectIcons[i].Render(sprite);
                }

                iconBg.Render(sprite);
                DrawText(sprite);
            }
        }

        #region Load
        private void LoadBackground()
        {
            picBackground = new MenuPic(Game, "ingameMenu.png", "MainMenu");
            MenuPicDrawPara para;
            para.Alpha = 0.3f;
            para.desiredWidth = 1024f;
            para.desiredHeight = 768f;
            para.PosX = 1024f / 2;
            para.PosY = 768f / 2;

            picBackground.curDrawPara = para;
            picBackground.ModColor = new Color3(0.55f, 0.55f, 0.55f);
        }

        private void LoadIconBackground()
        {
            iconBg = new MenuPic(Game, "Menu_Background.png", "Icon Background");
            MenuPicDrawPara para;
            para.Alpha = 0.8f;
            para.desiredWidth = 256f;
            para.desiredHeight = 256f;
            para.PosX = 512f;
            para.PosY = 300f;

            //set firstPara
            iconBg.firstDrawPara = para;

            //set lastPara
            para.desiredWidth = 226f;
            para.desiredHeight = 226f;

            iconBg.nextDrawPara = para;
        }

        private void LoadIcons()
        {
            picSelectIcons = new List<MenuPic>();
            //设置图标列表
            picSelectIcons.Add(new MenuPic(Game, "challenge.png", StringTableManager.StringTable["GUI:IGMResumeGame"]));
            picSelectIcons[picSelectIcons.Count - 1].ActivedHandler += ResumeGame_Activated;

            picSelectIcons.Add(new MenuPic(Game, "Option.png", StringTableManager.StringTable["GUI:IGMOptions"]));
            picSelectIcons[picSelectIcons.Count - 1].ActivedHandler += Options_Activated;

            picSelectIcons.Add(new MenuPic(Game, "Help.png", StringTableManager.StringTable["GUI:IGMExitMainMenu"]));
            picSelectIcons[picSelectIcons.Count - 1].ActivedHandler += ExitMainMenu_Activated;

        }

        private void LoadFont()
        {
            int width = 500;
            int height = 100;
            int x = 512 - width / 2;
            int y = 550 - height / 2;

            System.Drawing.Rectangle stringRect = new System.Drawing.Rectangle(x, y, width, height);
            textBox = new DrawTextBox(Game, device, "", UIFontName, 30, System.Drawing.Color.White, stringRect);
        }

        protected override void load()
        {
            LoadBackground();
            LoadIcons();
            LoadIconBackground();
            LoadFont();

            //下面设置图片的初始位置
            Vector2 midLocation = new Vector2(512f, 300f);
            Vector2 currentLocation = midLocation;
            Vector2 selectSize = new Vector2(256f, 256f);
            Vector2 unSelectSize = new Vector2(128f, 128f);
            Vector2 twoPicNearDis = new Vector2(200f, 0);

            const float selctAlpha = 0.8f;
            const float unSelectAlpha = 0.4f;
            int placeTime = 0;
            for (int i = 0; i < picSelectIcons.Count; i++)
            {
                if (i == 0)
                {
                    picSelectIcons[i].curDrawPara = new MenuPicDrawPara(midLocation.X, midLocation.Y,
                                                                        selctAlpha,
                                                                        selectSize.X, selectSize.Y);
                }
                else
                {
                    placeTime++;
                    if (placeTime <= 2)
                    {
                        currentLocation = currentLocation + twoPicNearDis;
                    }
                    if (i == 1)
                    {
                        picSelectIcons[i].curDrawPara = new MenuPicDrawPara(currentLocation.X, currentLocation.Y,
                                                                            unSelectAlpha,
                                                                            unSelectSize.X, unSelectSize.Y);
                    }
                    else
                    {
                        picSelectIcons[i].curDrawPara = new MenuPicDrawPara(currentLocation.X, currentLocation.Y,
                                                                            0f,
                                                                            unSelectSize.X, unSelectSize.Y);
                    }
                }
            }

            //下面设置5个渲染位置的初始参数
            picInitPara = new List<MenuPicDrawPara>();
            picInitPara.Add(new MenuPicDrawPara(midLocation - 2 * twoPicNearDis, 0f, unSelectSize));
            picInitPara.Add(new MenuPicDrawPara(midLocation - twoPicNearDis, unSelectAlpha, unSelectSize));
            picInitPara.Add(new MenuPicDrawPara(midLocation, 1.0f, selectSize));
            picInitPara.Add(new MenuPicDrawPara(midLocation + twoPicNearDis, unSelectAlpha, unSelectSize));
            picInitPara.Add(new MenuPicDrawPara(midLocation + 2 * twoPicNearDis, 0f, unSelectSize));
        }
        #endregion

        protected override void unload()
        {

        }

        #region 事件处理

        void ResumeGame_Activated(object sender, EventArgs e)
        {
            if (world.IsValid)
            {
                ingameUI.IsMenuShown = false;
            }
        }
        void Options_Activated(object sender, EventArgs e)
        {

        }
        void ExitMainMenu_Activated(object sender, EventArgs e)
        {
            world.Unload();
        }


        #endregion

        private void IngameMenu_ItemMoveLeft(object sender, EventArgs e)
        {
            if (world.IsValid)
            {
                if (!isAnimation)
                {
                    if (curIndex > 0)
                    {
                        isAnimation = true;
                        isMoveLeft = false;
                        usedAnimationTime = 0f;
                        for (int i = curIndex - 2; i <= curIndex + 2; i++)
                        {
                            if ((i >= 0) && (i < picSelectIcons.Count))
                            {
                                //如果是需要移动的内容,则记录下上一次的渲染参数
                                picSelectIcons[i].firstDrawPara = picSelectIcons[i].curDrawPara;
                            }
                        }
                    }
                }
            }
        }
        private void IngameMenu_ItemMoveRight(object sender, EventArgs e)
        {
            if (world.IsValid)
            {
                if (!isAnimation)
                {
                    if (curIndex < picSelectIcons.Count - 1)
                    {
                        isAnimation = true;
                        isMoveLeft = true;
                        usedAnimationTime = 0f;
                        for (int i = curIndex - 2; i <= curIndex + 2; i++)
                        {
                            if ((i >= 0) && (i < picSelectIcons.Count))
                            {
                                //如果是需要移动的内容,则记录下上一次的渲染参数
                                picSelectIcons[i].firstDrawPara = picSelectIcons[i].curDrawPara;
                            }
                        }
                    }
                }
            }
        }

        private void IngameMenu_Enter(object sender, EventArgs e)
        {
            if (world.IsValid)
            {
                if (!isAnimation)
                {
                    picSelectIcons[curIndex].Activate();
                }
            }
        }

        public override void NotifyActivated()
        {
            base.NotifyActivated();

            InputManager.Instance.Enter += this.IngameMenu_Enter;
            InputManager.Instance.ItemMoveLeft += this.IngameMenu_ItemMoveLeft;
            InputManager.Instance.ItemMoveRight += this.IngameMenu_ItemMoveRight;
        }
        public override void NotifyDeactivated()
        {
            base.NotifyDeactivated();

            InputManager.Instance.Enter -= this.IngameMenu_Enter;
            InputManager.Instance.ItemMoveLeft -= this.IngameMenu_ItemMoveLeft;
            InputManager.Instance.ItemMoveRight -= this.IngameMenu_ItemMoveRight;
        }
    }

    /// <summary>
    /// 游戏进行中的UI
    /// 可以用于显示加载进度条,速度,功率等等
    /// </summary>
    public class IngameUI : UIComponent
    {
        #region Fields
        LoadingScreen loadScreen;
        World world;

        GameUI gameUI;

        SlimDX.Direct3D9.Font debugFont;

        FpsCounter fpsc;

        GameMainLogic logic;

        StateIndicator stateIdi;
        PowerGraph powerGraph;
        IngameMenu ingameMenu;

        bool isMenuShown;

        public bool IsMenuShown
        {
            get { return isMenuShown; }
            set
            {
                if (isMenuShown != value)
                {
                    if (value)
                    {
                        ingameMenu.NotifyActivated();
                    }
                    else
                    {
                        ingameMenu.NotifyDeactivated();
                    }

                    isMenuShown = value;
                }
                if (world != null)
                {
                    world.Paused = value;
                }
            }
        }

        #endregion

        public IngameUI(GameUI gameUI, Game game, World world, GameMainLogic logic)
            : base(game)
        {
            this.gameUI = gameUI;
            this.logic = logic;
            this.world = world;

            this.debugFont = new SlimDX.Direct3D9.Font(gameUI.Device, System.Windows.Forms.Control.DefaultFont);

            this.fpsc = new FpsCounter();
            this.stateIdi = new StateIndicator(logic, this, gameUI.Device, Game);
            this.powerGraph = new PowerGraph(logic, this, gameUI.Device, Game);
            this.ingameMenu = new IngameMenu(game, world, logic, gameUI, this);

            this.loadScreen = new LoadingScreen(Game, gameUI, world.CreationParameters.LoadScreenConfig);
            this.world.CreationParameters.ProgressCallBack += this.loadScreen.SetProgressValue;
        }


        protected override void render()
        {
            if (loadScreen != null)
            {
                loadScreen.Render();
            }

            if (world.IsValid)
            {
                powerGraph.Render();
            }

            if (IsMenuShown)
            {
                ingameMenu.Render();
            }
        }

        protected override void render(Sprite sprite)
        {
            fpsc.Update();
            if (loadScreen != null)
            {
                loadScreen.Render(sprite);
            }
            else
            {
                if (world.IsValid)
                {
                    debugFont.DrawString(sprite, fpsc.ToString(), 5, 5, -1);
                    debugFont.DrawString(sprite, "BatchCount: " + world.Scene.BatchCount, 5, 15, -1);
                    debugFont.DrawString(sprite, "PrimCount: " + world.Scene.PrimitiveCount, 5, 25, -1);

                    stateIdi.Render(sprite);
                }

            }
            if (IsMenuShown)
            {
                ingameMenu.Render(sprite);
            }
        }

        protected override void update(float dt)
        {
            if (!world.IsLoaded)
            {
                loadScreen.Update(dt);
            }
            else
            {
                if (loadScreen != null)
                {
                    loadScreen.Unload();
                    loadScreen = null;

                    OnLoaded();
                }

                if (world.IsValid)
                {
                    if (!world.Paused)
                    {
                        stateIdi.Update(dt);
                        powerGraph.Update(dt);
                    }
                }

            }
            if (IsMenuShown)
            {
                ingameMenu.Update(dt);
            }
        }

        void OnLoaded()
        {
            stateIdi.CurrectBicycle = logic.CurrentCompetition.CurrentBicycle;
            stateIdi.TimerStarted = true;
            powerGraph.CurrectBicycle = stateIdi.CurrectBicycle;

            Themes.Instance.Play();
        }

        private void IngameUI_Escape(object sender, EventArgs e)
        {
            if (!IsMenuShown && world.IsValid)
            {
                IsMenuShown = true;
            }
        }


        protected override void load()
        {

        }

        protected override void unload()
        {
            //powerGraph.Dispose();
        }

        public override void NotifyActivated()
        {
            base.NotifyActivated();

            Themes.Instance.Play("Loading");

            InputManager.Instance.Escape += IngameUI_Escape;
        }

        public override void NotifyDeactivated()
        {
            base.NotifyDeactivated();

            InputManager.Instance.Escape -= IngameUI_Escape;
        }

    }
}
