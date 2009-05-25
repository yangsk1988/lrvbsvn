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
using VirtualBicycle.Sound;

namespace VirtualBicycle.UI
{
    public class StartScreen : UIComponent
    {
        static readonly string UIFontName = "微软雅黑";

        #region Fields
        private const string menuBackground = "startScreen.png";
        private MenuPic picBackground;
        private MenuPic picFontBg;
        private Device device;
        private GameUI gameUI;
        GameMainLogic logic;

        private DrawTextBox textBox;
        #endregion

        #region Constructor
        public StartScreen(Game game, GameUI gameUI, GameMainLogic logic)
            : base(game)
        {
            this.device = gameUI.Device;
            this.gameUI = gameUI;
            this.logic = logic;
        }
        #endregion

        #region Render
        protected override void render()
        {

        }

        private void DrawText(Sprite sprite)
        {
            textBox.Render(sprite,true);
        }

        protected override void render(Sprite sprite)
        {
            picBackground.Render(sprite);
            picFontBg.Render(sprite);
            DrawText(sprite);
        }
        #endregion

        #region Update
        float usedTime = 0.0f;
        const float bgAnimationTime = 1.5f;
        private void UpdateBackground(float dt)
        {

            float dPara = usedTime / bgAnimationTime;
            if (dPara > 1.0f)
            {
                dPara = 1.0f;
            }
            picBackground.SetCurrentPara(dPara);
        }

        protected override void update(float dt)
        {
            usedTime += dt;
            //当背景从暗到亮后才显示"enter ..."
            UpdateBackground(dt);
            picFontBg.SetCurrentPara(0f);
            if (usedTime > bgAnimationTime)
            {
                picFontBg.SetCurrentPara(0f);
            }
        }
        #endregion


        private void StartScreen_Enter(object sender, EventArgs e)
        {
            if (usedTime > bgAnimationTime)
            {
                picBackground.Activate();
            }
        }


        #region Load
        protected override void load()
        {
            picBackground = new MenuPic(Game, "startScreen.png", "Start Screen Background");
            MenuPicDrawPara para;
            para.Alpha = 0.5f;
            para.PosX = 1024f / 2;
            para.PosY = 768f / 2;
            para.desiredWidth = 1024f;
            para.desiredHeight = 768f;
            picBackground.firstDrawPara = para;

            para.Alpha = 1.0f;
            picBackground.nextDrawPara = para;
            picBackground.ActivedHandler += Bg_Activated;

            picFontBg = new MenuPic(Game, "startScreen_font_bg.png", "Start Screen Font BG");
            para.PosX = 1024f / 2;
            para.PosY = 600f;
            para.Alpha = 0.8f;
            para.desiredWidth = 500.0f;
            para.desiredHeight = 75.0f;
            picFontBg.firstDrawPara = para;
            picFontBg.nextDrawPara = MenuPicDrawPara.Zero;

            //初始化TextBox
            Rectangle stringRect;
            stringRect = new Rectangle((int)(para.PosX - para.desiredWidth / 2), (int)(para.PosY - para.desiredHeight / 2),
                (int)(para.desiredWidth), (int)(para.desiredHeight));
            textBox = new DrawTextBox(Game, device, StringTableManager.StringTable["MSG:SMPressEnter"], UIFontName, 22, Color.White, stringRect);

        }
        #endregion

        #region Unload
        protected override void unload()
        {

        }
        #endregion

        private void Bg_Activated(object sender, EventArgs e) 
        {
            gameUI.CurrentComponent = logic.UILogic.GetMainMenu();
            //gameUI.CurrentComponent = new ReportScreen(Game, gameUI, logic);
        }

        public override void NotifyActivated()
        {
            base.NotifyActivated();

            Themes.Instance.Play("Menu");

            InputManager.Instance.Enter += this.StartScreen_Enter;
        }

        public override void NotifyDeactivated()
        {
            base.NotifyDeactivated();

            InputManager.Instance.Enter -= this.StartScreen_Enter;
        }

    }
}
