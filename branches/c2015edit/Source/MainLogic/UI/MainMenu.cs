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
    public class MainMenu : UIComponent
    {
        static readonly string UIFontName = "微软雅黑";

        #region Fields
        private Device device;

        private GameUI gameUI;
        private GameMainLogic logic;

        MenuPic picBackground;
        List<MenuPic> picSelectIcons;
        List<MenuPicDrawPara> picInitPara;
        MenuPic iconBg;
        DrawTextBox textBox;
        int curIndex = 0;

        #endregion

        #region Constructor
        public MainMenu(Game game, GameUI gameUI, GameMainLogic logic)
            :base(game)
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
            textBox.Render(sprite, picSelectIcons[curIndex].Detail, true);
        }

        protected override void render(Sprite sprite)
        {
            picBackground.Render(sprite);

            for (int i = 0; i < picSelectIcons.Count; i++)
            {
                picSelectIcons[i].Render(sprite);
            }

            iconBg.Render(sprite);
            DrawText(sprite);
        }
        #endregion

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

        }
        #endregion


        private void MainMenu_ItemMoveLeft(object sender, EventArgs e)
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
        private void MainMenu_ItemMoveRight(object sender, EventArgs e)
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
        
        private void MainMenu_Enter(object sender, EventArgs e)
        {
            if (!isAnimation)
            {
                picSelectIcons[curIndex].Activate();
            }
        }
        private void MainMenu_Escape(object sender, EventArgs e)
        {
            Game.Exit();
        }

        #region Load
        private void LoadBackground()
        {
            picBackground = new MenuPic(Game, "mainMenu.png", "MainMenu");
            MenuPicDrawPara para;
            para.Alpha = 1;
            para.desiredWidth = 1024f;
            para.desiredHeight = 768f;
            para.PosX = 1024f / 2;
            para.PosY = 768f / 2;

            picBackground.curDrawPara = para;
            picBackground.ModColor = Color.FromArgb(140, 140, 140);
        }

        private void LoadIconBackground()
        {
            iconBg = new MenuPic(Game, "Menu_Background.png", "Icon Background");
            MenuPicDrawPara para;
            para.Alpha = 1f;
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
            picSelectIcons.Add(new MenuPic(Game, "challenge.png", StringTableManager.StringTable["GUI:MMChallenge"]));
            picSelectIcons[picSelectIcons.Count - 1].ActivedHandler += ChallengeActivated;

            picSelectIcons.Add(new MenuPic(Game, "Option.png", StringTableManager.StringTable["GUI:MMOption"]));//"选项"
            picSelectIcons[picSelectIcons.Count - 1].ActivedHandler += OptionActived;

            picSelectIcons.Add(new MenuPic(Game, "Help.png", StringTableManager.StringTable["GUI:MMHelp"]));//"帮助"
            picSelectIcons[picSelectIcons.Count - 1].ActivedHandler += HelpActivated;

            picSelectIcons.Add(new MenuPic(Game, "Coach.png", StringTableManager.StringTable["GUI:MMCoach"]));//"教学模式"
            picSelectIcons[picSelectIcons.Count - 1].ActivedHandler += TutorialActivated;

            picSelectIcons.Add(new MenuPic(Game, "Heros.png", StringTableManager.StringTable["GUI:MMHighscores"]));//"高分榜"
            picSelectIcons[picSelectIcons.Count - 1].ActivedHandler += HighScoresActivated;
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
            Vector2 unSelectSize =  new Vector2(128f, 128f);
            Vector2 twoPicNearDis = new Vector2(200f,0);
            
            float selctAlpha = 1f;
            float unSelectAlpha = 0.5f;
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
            picInitPara.Add(new MenuPicDrawPara(midLocation - 2 * twoPicNearDis,0f,unSelectSize));
            picInitPara.Add(new MenuPicDrawPara(midLocation - twoPicNearDis, unSelectAlpha, unSelectSize));
            picInitPara.Add(new MenuPicDrawPara(midLocation,1.0f,selectSize));
            picInitPara.Add(new MenuPicDrawPara(midLocation + twoPicNearDis, unSelectAlpha, unSelectSize));
            picInitPara.Add(new MenuPicDrawPara(midLocation + 2 * twoPicNearDis, 0f, unSelectSize));
        }
        #endregion

        #region Unload
        protected override void unload()
        {

        }
        #endregion

        #region Events
        private void ChallengeActivated(object sender, EventArgs e)
        {
            gameUI.CurrentComponent = logic.UILogic.GetMapSelectScreen();
        }

        private void OptionActived(object sender, EventArgs e)
        {
            gameUI.CurrentComponent = logic.UILogic.GetOptionScreen();
        }
        void HelpActivated(object sender, EventArgs e) 
        {

        }
        void TutorialActivated(object sender, EventArgs e) 
        {

        
        }

        void HighScoresActivated(object sender, EventArgs e) 
        {

        }

        #endregion

        public override void NotifyActivated()
        {
            base.NotifyActivated();

            InputManager.Instance.Enter += this.MainMenu_Enter;
            InputManager.Instance.ItemMoveLeft += this.MainMenu_ItemMoveLeft;
            InputManager.Instance.ItemMoveRight += this.MainMenu_ItemMoveRight;
            InputManager.Instance.Escape += this.MainMenu_Escape;
        }
        public override void NotifyDeactivated()
        {
            base.NotifyDeactivated();

            InputManager.Instance.Enter -= this.MainMenu_Enter;
            InputManager.Instance.ItemMoveLeft -= this.MainMenu_ItemMoveLeft;
            InputManager.Instance.ItemMoveRight -= this.MainMenu_ItemMoveRight;
            InputManager.Instance.Escape -= this.MainMenu_Escape;
        }
    }
}
