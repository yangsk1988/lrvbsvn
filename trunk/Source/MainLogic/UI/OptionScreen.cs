using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using MainLogic;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.ConfigModel;
using VirtualBicycle.Graphics;
using VirtualBicycle.Input;
using VirtualBicycle.IO;
using VirtualBicycle.Sound;

namespace VirtualBicycle.UI
{
    public class OptionScreen : UIComponent
    {
        static readonly string UIFontName = "微软雅黑";

        #region Fields
        const int maxVolume = 9;

        private Device device;
        private GameUI gameUI;
        private GameMainLogic logic;

        MenuPic picBackground;
        List<MenuPic> picSelectIcons;
        List<MenuPicDrawPara> picInitPara;
       
        MenuPic picGrayBoard;
        MenuPic iconBg;
        DrawTextBox nameTextBox;
        int curIndex = 0;

        System.Drawing.Rectangle inputDevStringRect;
        System.Drawing.Rectangle drawInputDevStringRect;
        DrawTextBox inputDevieTextBox;

        /// <summary>
        /// 是否选择了图标
        /// </summary>
        bool isSelectedIcon;

        public int SoundVolume
        {
            get
            {
                AudioConfigs audCon = (AudioConfigs)BasicConfigs.Instance[AudioConfigFacotry.AudioConfigName];

                return (int)(audCon.SoundVolume * 9);
            }
            set
            {
                AudioConfigs audCon = (AudioConfigs)BasicConfigs.Instance[AudioConfigFacotry.AudioConfigName];

                audCon.SoundVolume = value / 9.0f;
            }
        }

        public int MusicVolume
        {
            get
            {
                AudioConfigs audCon = (AudioConfigs)BasicConfigs.Instance[AudioConfigFacotry.AudioConfigName];

                return (int)(audCon.ScoreVolume * 9);
            }
            set
            {
                AudioConfigs audCon = (AudioConfigs)BasicConfigs.Instance[AudioConfigFacotry.AudioConfigName];
                audCon.ScoreVolume = value / 9.0f;
                Themes.Instance.SetVolume(audCon.ScoreVolume);
            }
        }


        enum EnumInputType
        {
            Keyboard,
            Bicycle
        }
        EnumInputType inputType;
        #endregion

        #region Constructor
        public OptionScreen(Game game, GameUI gameUI, GameMainLogic logic)
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
            System.Drawing.Color color;
            if (isSelectedIcon)
            {
                color = System.Drawing.Color.SkyBlue;
            }
            else
            {
                color = System.Drawing.Color.White;
            }
            nameTextBox.Render(sprite, picSelectIcons[curIndex].Detail, color,true);
        }

        protected override void render(Sprite sprite)
        {
            picBackground.Render(sprite);

            picGrayBoard.Render(sprite);

            for (int i = 0; i < picSelectIcons.Count; i++)
            {
                picSelectIcons[i].Render(sprite);
            }

            iconBg.Render(sprite);
            DrawText(sprite);
            RenderOptions(sprite);
        }

        List<MenuPic> VolumeBar = null;

        private void RenderVolume(Sprite sprite, int n)
        {
            Vector2 midPlace = new Vector2(512f, 620f);
            Vector2 size = new Vector2(20f, 50f);
            const float nearDis = 50f;
            const string fileName = "Volume_Block.png";
            Color colorOn = Color.FromArgb(173, 255, 216);
            Color colorOff = Color.FromArgb(87, 87, 87);

            
            if (VolumeBar == null)
            {
                VolumeBar = new List<MenuPic>();
                for (int i = 1; i <= maxVolume; i++)
                {

                    VolumeBar.Add(new MenuPic(Game, fileName, "Volume_Block"));
                    MenuPicDrawPara para = new MenuPicDrawPara(midPlace.X - (maxVolume / 2 - i + 1) * nearDis, midPlace.Y, 1f, 20f, 50f);
                    VolumeBar[VolumeBar.Count - 1].firstDrawPara = para;
                    VolumeBar[VolumeBar.Count - 1].nextDrawPara = MenuPicDrawPara.Zero;
                    VolumeBar[VolumeBar.Count - 1].SetCurrentPara(0f);
                }
            }

            for (int i = 0; i < VolumeBar.Count; i++)
            {
                if (i < n)
                {
                    VolumeBar[i].ModColor = colorOn;
                }
                else
                {
                    VolumeBar[i].ModColor = colorOff;
                }
                VolumeBar[i].Render(sprite);
            }
        }

        private void RenderInputDevice(Sprite sprite)
        {
            string text;
            if (inputType == EnumInputType.Bicycle)
            {
                text = "使用自行车输入";
            }
            else 
            {
                text = "使用键盘输入";
            }
            inputDevieTextBox.Render(sprite, text,true);
        }

        private void RenderOptions(Sprite sprite)
        {
            if (picSelectIcons[curIndex].Detail == "音效")
            {
                RenderVolume(sprite, SoundVolume);
            }
            else if (picSelectIcons[curIndex].Detail == "音乐")
            {
                RenderVolume(sprite, MusicVolume);
            }
            else if (picSelectIcons[curIndex].Detail == "输入设备")
            {
                RenderInputDevice(sprite);
            }
        }
        #endregion

        #region Update
        bool isAnimation;
        bool isMoveLeft = true;
        float usedAnimationTime = 0f;
        float curTime = 0;

        private void ChangeInputDevice()
        {
            if (inputType == EnumInputType.Bicycle)
            {
                inputType = EnumInputType.Keyboard;
                InputManager.Instance.Processor = new KeyboardInputProcessor(InputManager.Instance);
            }
            else
            {
                inputType = EnumInputType.Bicycle;
                InputManager.Instance.Processor = new SerialPortInputProcessor(InputManager.Instance);
            }
        }

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

        private void OptionScreen_ItemMoveLeft(object sender, EventArgs e)
        {
            if (!isAnimation)
            {
                if ((curIndex > 0) && (!isSelectedIcon))
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
                else if (isSelectedIcon)
                {
                    if (picSelectIcons[curIndex].Detail == "音效")
                    {
                        if (SoundVolume > 0)
                        {
                            SoundVolume--;
                        }
                    }
                    else if (picSelectIcons[curIndex].Detail == "音乐")
                    {
                        if (MusicVolume > 0)
                        {
                            MusicVolume--;
                        }
                    }
                    else if (picSelectIcons[curIndex].Detail == "输入设备")
                    {
                        ChangeInputDevice();
                    }
                }
            }
        }
        private void OptionScreen_ItemMoveRight(object sender, EventArgs e)
        {
            if (!isAnimation)
            {
                if ((curIndex < picSelectIcons.Count - 1) && (!isSelectedIcon))
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
                else if (isSelectedIcon)
                {
                    if (picSelectIcons[curIndex].Detail == "音效")
                    {
                        if (SoundVolume < maxVolume)
                        {
                            SoundVolume++;
                        }
                    }
                    else if (picSelectIcons[curIndex].Detail == "音乐")
                    {
                        if (MusicVolume < maxVolume)
                        {
                            MusicVolume++;
                        }
                    }
                    else if (picSelectIcons[curIndex].Detail == "输入设备")
                    {
                        ChangeInputDevice();
                    }
                }
            }
        }

        private void OptionScreen_Enter(object sender, EventArgs e)
        {
            if (!isAnimation)
            {
                isSelectedIcon = !isSelectedIcon;
            }
        }

        private void OptionScreen_Escape(object sender, EventArgs e)
        {
            if (!isAnimation)
            {
                gameUI.Pop();
                //gameUI.CurrentComponent = logic.UILogic.GetMainMenu();
            }
        }

        #region Load
        private void LoadBackground()
        {
            picBackground = new MenuPic(Game, "Option_Background.png", "Option");
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
            picSelectIcons.Add(new MenuPic(Game, "input.png", "输入设备"));
            picSelectIcons.Add(new MenuPic(Game, "sound.png", "音效"));
            picSelectIcons.Add(new MenuPic(Game, "Music.png", "音乐"));
            picSelectIcons.Add(new MenuPic(Game, "View.png", "显示"));
        }

        private void LoadFont()
        {
            System.Drawing.Rectangle stringRect;

            int width = 500;
            int height = 100;
            int x = 512 - width / 2;
            int y = 500 - height / 2;

            stringRect = new System.Drawing.Rectangle(x, y, width, height);
            nameTextBox = new DrawTextBox(Game, device, "", UIFontName, 30, System.Drawing.Color.White, stringRect);

            width = 500;
            height = 100;
            x = 512 - width / 2;
            y = 620 - height / 2;

            stringRect = new System.Drawing.Rectangle(x, y, width, height);
            inputDevieTextBox = new DrawTextBox(Game, device, "", UIFontName, 25, System.Drawing.Color.White, stringRect);
        }

        private void LoadGrayBoard()
        {
            picGrayBoard = new MenuPic(Game, "startScreen_font_bg.png", "Gray Board");
            MenuPicDrawPara para;
            para.Alpha = 0.5f;
            para.PosX = 512f;
            para.PosY = 378f;
            para.desiredWidth = 1024f;
            para.desiredHeight = 700f;
            picGrayBoard.firstDrawPara = para;
            picGrayBoard.nextDrawPara = MenuPicDrawPara.Zero;
            picGrayBoard.SetCurrentPara(0f);
        }

        protected override void load()
        {
            LoadBackground();
            LoadIcons();
            LoadIconBackground();
            LoadFont();
            LoadGrayBoard();

            //下面设置图片的初始位置
            Vector2 midLocation = new Vector2(512f, 300f);
            Vector2 currentLocation = midLocation;
            Vector2 selectSize = new Vector2(256f, 256f);
            Vector2 unSelectSize = new Vector2(128f, 128f);
            Vector2 twoPicNearDis = new Vector2(250f, 0);

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
            picInitPara.Add(new MenuPicDrawPara(midLocation - 2 * twoPicNearDis, 0f, unSelectSize));
            picInitPara.Add(new MenuPicDrawPara(midLocation - twoPicNearDis, unSelectAlpha, unSelectSize));
            picInitPara.Add(new MenuPicDrawPara(midLocation, 1.0f, selectSize));
            picInitPara.Add(new MenuPicDrawPara(midLocation + twoPicNearDis, unSelectAlpha, unSelectSize));
            picInitPara.Add(new MenuPicDrawPara(midLocation + 2 * twoPicNearDis, 0f, unSelectSize));

            isSelectedIcon = false;
        }
        #endregion

        #region Unload
        protected override void unload()
        {

        }
        #endregion

        #region Events
        
        #endregion

        public override void NotifyActivated()
        {
            base.NotifyActivated();

            InputManager.Instance.ItemMoveLeft += this.OptionScreen_ItemMoveLeft;
            InputManager.Instance.ItemMoveRight += this.OptionScreen_ItemMoveRight;
            InputManager.Instance.Enter += OptionScreen_Enter;
            InputManager.Instance.Escape += OptionScreen_Escape;
        }

        public override void NotifyDeactivated()
        {
            base.NotifyDeactivated();

            InputManager.Instance.ItemMoveLeft -= this.OptionScreen_ItemMoveLeft;
            InputManager.Instance.ItemMoveRight -= this.OptionScreen_ItemMoveRight;
            InputManager.Instance.Enter -= OptionScreen_Enter;
            InputManager.Instance.Escape -= OptionScreen_Escape;
        }

    }
}
