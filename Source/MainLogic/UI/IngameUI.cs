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
        GameFinishedUI finishedUI;

        DrawTextBox heartRateText;
        MenuPic heartPic;

        bool isMenuShown;
        bool isFinishedUIShown;

        float heartInterval;
        float lastHeartInterval;

        float heartTime;
        float heartRate;

        public bool IsMenuShown
        {
            get { return isMenuShown; }
            set
            {
                if (IsFinishedUIShown)
                    return;
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

        public bool IsFinishedUIShown 
        {
            get { return isFinishedUIShown; }
            set 
            {
                if (isFinishedUIShown != value) 
                {
                    if (value)
                    {
                        finishedUI.NotifyActivated();
                    }
                    else 
                    {
                        finishedUI.NotifyDeactivated();
                    }

                    isFinishedUIShown = value;
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
            this.finishedUI = new GameFinishedUI(game, world, logic, gameUI, this);

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

                    heartPic.Render(sprite);


                    if (heartRate > 90 && heartRate < 120)
                    {
                        float lerp = (120 - heartRate) / 30;

                        Color a = Color.White;
                        Color b = Color.Yellow;

                        int dr = (int)(a.R * lerp + b.R * (1 - lerp));
                        int dg = (int)(a.G * lerp + b.G * (1 - lerp));
                        int db = (int)(a.B * lerp + b.B * (1 - lerp));

                        heartPic.ModColor = Color.FromArgb(dr, dg, db);
                    }
                    else if (heartRate > 120 && heartRate < 180)
                    {
                        float lerp = (180 - heartRate) / 60;

                        Color a = Color.Yellow;
                        Color b = Color.Red;

                        int dr = (int)(a.R * lerp + b.R * (1 - lerp));
                        int dg = (int)(a.G * lerp + b.G * (1 - lerp));
                        int db = (int)(a.B * lerp + b.B * (1 - lerp));

                        heartPic.ModColor = Color.FromArgb(dr, dg, db);
                    }
                    else if (heartRate > 180)
                    {
                        heartPic.ModColor = Color.Red;
                    }
                    else
                    {
                        heartPic.ModColor = Color.White;
                    }


                    if (heartRate < 30)
                    {
                        heartRateText.Render(sprite, "--", true);
                    }
                    else
                    {
                        heartRateText.Render(sprite, heartRate.ToString("F00"), true);
                    }
                }

            }
            if (IsFinishedUIShown) 
            {
                finishedUI.Render(sprite);
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

                heartInterval += dt;
                heartTime += dt;

                if (lastHeartInterval > float.Epsilon)                
                    this.heartPic.SetCurrentPara(Math.Min(heartInterval / lastHeartInterval, 1));           
                else
                    this.heartPic.SetCurrentPara(0);                

                if (heartTime > 2.5f)
                {
                    heartRate = 0;
                }
            }
            if (IsFinishedUIShown)
            {
                finishedUI.Update(dt);
                if (finishedUI.Finished)
                {
                    Game.GameUI.Pop();
                    Game.CurrentWorld = null;
                    world.Paused = true;
                    world.Dispose();

                    Game.GameUI.CurrentComponent = logic.UILogic.GetReportScreen();
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

        private void IngameUI_HeartPulse(object sender, EventArgs e)
        {
            heartRate = 60.0f / heartInterval;
            lastHeartInterval = heartInterval;
            heartInterval = 0;
            heartTime = 0;
            this.heartPic.SetCurrentPara(0);
        }

        protected override void load()
        {
            this.heartRateText = new DrawTextBox(this.Game, gameUI.Device, string.Empty, Control.DefaultFont, 30, Color.White,
                new Rectangle(70, 100, 250, 70), DrawTextFormat.Top | DrawTextFormat.Left | DrawTextFormat.SingleLine);
            this.heartPic = new MenuPic(this.Game, "heart.png", string.Empty);

            MenuPicDrawPara para;
            para.Alpha = 1f;
            para.desiredWidth = 50;
            para.desiredHeight = 50;
            para.PosX = 10 + 25;
            para.PosY = 100 + 25;

            this.heartPic.ModColor = Color.White;
            this.heartPic.firstDrawPara = para;

            para.Alpha = 1f;
            para.desiredWidth = 30;
            para.desiredHeight = 30;
            para.PosX = 10 + 25;
            para.PosY = 100 + 25;

            this.heartPic.nextDrawPara = para;
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
            InputManager.Instance.HeartPulse += IngameUI_HeartPulse;
        }


        public override void NotifyDeactivated()
        {
            base.NotifyDeactivated();

            InputManager.Instance.Escape -= IngameUI_Escape;
            InputManager.Instance.HeartPulse -= IngameUI_HeartPulse;
        }

    }
}
