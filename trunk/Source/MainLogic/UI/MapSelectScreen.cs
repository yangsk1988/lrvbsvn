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
using VirtualBicycle.Scene;

namespace VirtualBicycle.UI
{
    /// <summary>
    /// 用于选择一张地图进入游戏
    /// </summary>
    public class MapSelectScreen : UIComponent
    {
        #region Fields
        static readonly string UIFontName = "微软雅黑";
        const string bgImage = "SelectMapBg.png";

        GameUI gameUI;

        Device device;
        LevelInfo[] levelInfos;
        int CurrentIndex = 0;

        MenuPic backgroundImage;

        private GameMainLogic logic;

        List<MenuPicDrawPara> picInitPara;

        public struct LevelButton
        {
            public LevelInfo info;
            public MenuPic pic;
        }
        List<LevelButton> levels;

        DrawTextBox nameTextBox;
        DrawTextBox sizeTextBox;
        DrawTextBox detailTextBox;
        #endregion

        public MapSelectScreen(Game game, GameUI gameUI, GameMainLogic logic)
            : base(game)
        {
            this.gameUI = gameUI;

            this.device = gameUI.Device;
            this.logic = logic;

            backgroundImage = new MenuPic(Game, bgImage, "Select Map");

            MenuPicDrawPara para;
            para.Alpha = 1;
            para.desiredWidth = 1024f;
            para.desiredHeight = 768f;
            para.PosX = 1024f / 2;
            para.PosY = 768f / 2;

            backgroundImage.curDrawPara = para;
            backgroundImage.nextDrawPara = MenuPicDrawPara.Zero;
            backgroundImage.ModColor = Color.FromArgb(140, 140, 140);
        }

        protected override void render()
        {


        }

        protected override void render(Sprite sprite)
        {
            backgroundImage.Render(sprite);
            for (int i = 0; i < levels.Count; i++)
            {
                levels[i].pic.Render(sprite);
            }

            #region DrawString
            if ((CurrentIndex >= 0) && CurrentIndex < levels.Count)
            {
                nameTextBox.Render(sprite, string.Format(StringTableManager.StringTable["GUI:MSMMapName"], 
                    levels[CurrentIndex].info.Name), true);

                sizeTextBox.Render(sprite, string.Format(StringTableManager.StringTable["GUI:MSMSize"], 
                    levels[CurrentIndex].info.Size.ToString()), true);

                detailTextBox.Render(sprite, string.Format(StringTableManager.StringTable["GUI:MSMDescription"], 
                    levels[CurrentIndex].info.Detail), true);
            }
            #endregion
        }

        void StartScene(LevelInfo info)
        {
            WorldCreationParameters pm = new WorldCreationParameters();

            pm.LoadScreenConfig = "ls001.ini";
            pm.SceneDataFile = info.Filename;

            World world = new World(Game, device, pm);

            Game.CurrentWorld = world;
            gameUI.CurrentComponent = new IngameUI(gameUI, Game, world, logic);
            world.Load();
        }

        float usedAnimationTime = 0f;
        float curTime = 0f;
        bool isAnimation = false;
        bool isMoveLeft = true;

        protected override void update(float dt)
        {
            curTime += dt;
            const float animationTime = 0.2f;
            if (isAnimation)
            {
                int dIndex = CurrentIndex - 2;
                usedAnimationTime += dt;
                if (usedAnimationTime > animationTime)
                {
                    usedAnimationTime = animationTime + 1e-7f;
                }

                for (int i = CurrentIndex - 2; i <= CurrentIndex + 2; i++)
                {
                    if ((i >= 0) && (i < levels.Count))
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
                            levels[i].pic.nextDrawPara = picInitPara[next];
                            levels[i].pic.SetCurrentPara(dPara);
                        }
                    }
                }

                //如果超过了动画时间,则动画播放结束
                if (usedAnimationTime > animationTime)
                {
                    isAnimation = false;
                    if (isMoveLeft)
                    {
                        CurrentIndex++;
                    }
                    else
                    {
                        CurrentIndex--;
                    }
                }
            }
        }

        private void InitText()
        {
            DrawTextFormat format = DrawTextFormat.Left | DrawTextFormat.SingleLine | DrawTextFormat.Top;
            int width = 300;
            int height = 100;
            int x = 512 - width / 2;
            int y = 470 - height / 2;

            Rectangle nameRect = new Rectangle(x, y, width, height);
            nameTextBox = new DrawTextBox(Game, device, "", UIFontName, 25, Color.White, nameRect, format);

            width = 300;
            height = 100;
            x = 512 - width / 2;
            y = 550 - height / 2;
            Rectangle sizeRect = new Rectangle(x, y, width, height);
            sizeTextBox = new DrawTextBox(Game, device, "", UIFontName, 25, Color.White, sizeRect, format);

            width = 300;
            height = 200;
            x = 512 - width / 2;
            y = 680 - height / 2;
            Rectangle detailRect = new Rectangle(x, y, width, height);
            detailTextBox = new DrawTextBox(Game, device, "", UIFontName, 22, Color.White, detailRect, format);
        }
        protected override void load()
        {
            InitText();
            Vector2 midLocation = new Vector2(512f, 250f);
            Vector2 currentLocation = midLocation;
            Vector2 selectSize = new Vector2(300f, 300f);
            Vector2 unSelectSize = new Vector2(200f, 200f);
            Vector2 twoPicNearDis = new Vector2(300f, 0);
            float unSelectAlpha = 0.5f;
            float selectAlpha = 0.9f;

            //load Level Info
            string[] files = FileSystem.Instance.SearchFile(Paths.Maps + Path.DirectorySeparatorChar + "*.vmp");

            levelInfos = new LevelInfo[files.Length];
            levels = new List<LevelButton>();

            int placeTime = 0;
            //load Level Info and MenuPic
            for (int i = 0; i < files.Length; i++)
            {
                FileLocation fl = new FileLocation(files[i]);
                levelInfos[i].Load(device, fl);

                LevelButton button = new LevelButton();
                button.info = levelInfos[i];

                //button.pic = new MenuPic(levelInfos[i].Preview, levelInfos[i].Detail, device);

                button.pic = new MenuPic(Game, levelInfos[i].Preview, " ");
                if (i == 0)
                {
                    button.pic.curDrawPara = new MenuPicDrawPara(midLocation.X, midLocation.Y,
                        selectAlpha, selectSize.X, selectSize.Y);
                }
                else
                {
                    placeTime++;
                    if (placeTime <= 2)
                    {
                        currentLocation += twoPicNearDis;
                        button.pic.curDrawPara = new MenuPicDrawPara(currentLocation.X, currentLocation.Y,
                            unSelectAlpha, unSelectSize.X, unSelectSize.Y);
                    }
                    else
                    {
                        button.pic.curDrawPara = new MenuPicDrawPara(currentLocation.X, currentLocation.Y,
                            0f, unSelectSize.X, unSelectSize.Y);
                    }
                }
                levels.Add(button);
                    
            }

            picInitPara = new List<MenuPicDrawPara>();
            picInitPara.Add(new MenuPicDrawPara(midLocation - 2 * twoPicNearDis, 0, unSelectSize));
            picInitPara.Add(new MenuPicDrawPara(midLocation - twoPicNearDis,unSelectAlpha,unSelectSize));
            picInitPara.Add(new MenuPicDrawPara(midLocation,selectAlpha,selectSize));
            picInitPara.Add(new MenuPicDrawPara(midLocation + twoPicNearDis,unSelectAlpha,unSelectSize));
            picInitPara.Add(new MenuPicDrawPara(midLocation + 2 * twoPicNearDis, 0, unSelectSize));
        }

        protected override void unload()
        {
        }

        private void MapSelectScreen_ItemMoveLeft(object sender, EventArgs e)
        {
            if (!isAnimation)
            {
                if (CurrentIndex > 0)
                {
                    isAnimation = true;
                    isMoveLeft = false;
                    usedAnimationTime = 0f;
                    for (int i = CurrentIndex - 2; i <= CurrentIndex + 2; i++)
                    {
                        if ((i >= 0) && (i < levels.Count))
                        {
                            //如果是需要移动的内容,则记录下上一次的渲染参数
                            levels[i].pic.firstDrawPara = levels[i].pic.curDrawPara;
                        }
                    }
                }
            }
        }
        private void MapSelectScreen_ItemMoveRight(object sender, EventArgs e)
        {
            if (!isAnimation)
            {
                if (CurrentIndex < levels.Count - 1)
                {
                    isAnimation = true;
                    isMoveLeft = true;
                    usedAnimationTime = 0f;
                    for (int i = CurrentIndex - 2; i <= CurrentIndex + 2; i++)
                    {
                        if ((i >= 0) && (i < levels.Count))
                        {
                            //如果是需要移动的内容,则记录下上一次的渲染参数
                            levels[i].pic.firstDrawPara = levels[i].pic.curDrawPara;
                        }
                    }
                }

            }
        }

        private void MapSelectScreen_Enter(object sender, EventArgs e)
        {
            if (!isAnimation)
            {
                StartScene(levelInfos[CurrentIndex]);
            }
        }
        private void MapSelectScreen_Escape(object sender, EventArgs e)
        {
            if (!isAnimation)
            {
                gameUI.Pop();
                //gameUI.CurrentComponent = logic.UILogic.GetMainMenu();
            }
        }

        public override void NotifyActivated()
        {
            base.NotifyActivated();
            InputManager.Instance.Enter += this.MapSelectScreen_Enter;
            InputManager.Instance.Escape += this.MapSelectScreen_Escape;
            InputManager.Instance.ItemMoveLeft += this.MapSelectScreen_ItemMoveLeft;
            InputManager.Instance.ItemMoveRight += this.MapSelectScreen_ItemMoveRight;
        }
        public override void NotifyDeactivated()
        {
            base.NotifyDeactivated();
            InputManager.Instance.Enter -= this.MapSelectScreen_Enter;
            InputManager.Instance.Escape -= this.MapSelectScreen_Escape;
            InputManager.Instance.ItemMoveLeft -= this.MapSelectScreen_ItemMoveLeft;
            InputManager.Instance.ItemMoveRight -= this.MapSelectScreen_ItemMoveRight;
        }
    }
}
