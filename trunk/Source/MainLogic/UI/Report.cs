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
    public class ReportScreen : UIComponent
    {
        #region Fields

        static readonly string UIFontName = "微软雅黑";

        const string bgImage = "report_bg.png";

        MenuPic picBackground;
        MenuPic iconBg;
        MenuPic heroGridPic;
        MenuPic rankGridPic;
        MenuPic curveGridPic;
        MenuPic curveGridContent;
        List<MenuPicDrawPara> picInitPara;
        List<MenuPic> pics;

        DrawGrid heroGrid;
        DrawGrid rankGrid;

        Device device;
        GameMainLogic logic;
        GameUI gameUI;

        int curIndex;
        DrawTextBox selectItemTB;
        #endregion

        #region Constructor
        public ReportScreen(Game game, GameUI gameUI, GameMainLogic logic)
            : base(game)
        {
            this.gameUI = gameUI;

            this.device = gameUI.Device;
            this.logic = logic;
        }

        #endregion

        #region Methods
        protected override void render()
        {
        }

        private void RenderGrid(Sprite sprite)
        {
            if (pics[curIndex].Detail == StringTableManager.StringTable["GUI:RMHighscores"])
            {
                selectItemTB.Render(sprite, StringTableManager.StringTable["GUI:RMHighscores"], true);
                heroGridPic.Render(sprite);
                heroGrid.Render(sprite);
            }
            else if (pics[curIndex].Detail == StringTableManager.StringTable["GUI:RMRank"])
            {
                selectItemTB.Render(sprite, StringTableManager.StringTable["GUI:RMRank"], true);
                rankGridPic.Render(sprite);
                rankGrid.Render(sprite);
            }
            else if (pics[curIndex].Detail == StringTableManager.StringTable["GUI:RMGraph"])
            {
                selectItemTB.Render(sprite, StringTableManager.StringTable["GUI:RMGraph"], true);
                curveGridPic.Render(sprite);
                curveGridContent.Render(sprite);
            }
        }

        protected override void render(Sprite sprite)
        {
            picBackground.Render(sprite);

            iconBg.Render(sprite);

            for (int i = 0; i < pics.Count; i++)
            {
                pics[i].Render(sprite);
            }

            RenderGrid(sprite);
        }

        bool isAnimation = false;
        bool isMoveLeft = true;
        float usedAnimationTime = 0f;
        float curTime = 0;

        protected override void update(float dt)
        {
            curTime += dt;
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
                    if ((i >= 0) && (i < pics.Count))
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
                            pics[i].nextDrawPara = picInitPara[next];
                            pics[i].SetCurrentPara(dPara);
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

        private void Report_ItemMoveLeft(object sender, EventArgs e)
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
                        if ((i >= 0) && (i < pics.Count))
                        {
                            //如果是需要移动的内容,则记录下上一次的渲染参数
                            pics[i].firstDrawPara = pics[i].curDrawPara;
                        }
                    }
                }
            }
        }
        private void Report_ItemMoveRight(object sender, EventArgs e)
        {
            if (!isAnimation)
            {
                if (curIndex < pics.Count - 1)
                {
                    isAnimation = true;
                    isMoveLeft = true;
                    usedAnimationTime = 0f;
                    for (int i = curIndex - 2; i <= curIndex + 2; i++)
                    {
                        if ((i >= 0) && (i < pics.Count))
                        {
                            //如果是需要移动的内容,则记录下上一次的渲染参数
                            pics[i].firstDrawPara = pics[i].curDrawPara;
                        }
                    }
                }
            }
        }
        private void Report_Enter(object sender, EventArgs e)
        {
            if (!isAnimation)
            {
                gameUI.Pop();
            }
        }

        private void LoadIconBg()
        {
            iconBg = new MenuPic(Game, "startScreen_font_bg.png", "Icon BG");
            MenuPicDrawPara para;
            para.Alpha = 0.5f;
            para.desiredWidth = 1024f;
            para.desiredHeight = 150f;
            para.PosX = 512f;
            para.PosY = 568f;
            iconBg.curDrawPara = para;
            iconBg.ModColor = new Color3(0.55f, 0.55f, 0.55f);
        }

        private void LoadBackground()
        {
            picBackground = new MenuPic(Game, "report_bg.png", "MainMenu");
            MenuPicDrawPara para;
            para.Alpha = 1;

            para.desiredWidth = 1024f;
            para.desiredHeight = 768f;
            para.PosX = 1024f / 2;
            para.PosY = 768f / 2;

            picBackground.curDrawPara = para;
            picBackground.ModColor = new Color3(0.55f, 0.55f, 0.55f);
        }


        private void LoadIcons()
        {
            pics = new List<MenuPic>();
            //设置图标列表
            pics.Add(new MenuPic(Game, "report_curve.png", StringTableManager.StringTable["GUI:RMGraph"]));

            pics.Add(new MenuPic(Game, "report_heroes.png", StringTableManager.StringTable["GUI:RMHighscores"]));

            pics.Add(new MenuPic(Game, "report_rank.png", StringTableManager.StringTable["GUI:RMRank"]));
        }

        private void LoadGridPic()
        {
            heroGridPic = new MenuPic(Game, "report_grid_hero.png", "Hero Grid");
            rankGridPic = new MenuPic(Game, "report_grid_rank.png", "Rank Grid");
            curveGridPic = new MenuPic(Game, "report_grid_curve.png", "Curve Grid");
            MenuPicDrawPara para;
            para.Alpha = 0.6f;

            para.desiredWidth = 600f;
            para.desiredHeight = 400f;
            para.PosX = 512f;
            para.PosY = 230f;

            heroGridPic.curDrawPara = para;
            heroGridPic.ModColor = new Color3(1f, 1f, 1f);

            rankGridPic.curDrawPara = para;
            rankGridPic.ModColor = new Color3(1f, 1f, 1f);

            curveGridPic.curDrawPara = para;
            curveGridPic.ModColor = new Color3(1f, 1f, 1f);

            curveGridContent = new MenuPic(Game, "report_curve_content.png", "Curve Grid Content");
            para.desiredWidth = 600f;
            para.desiredHeight = 352f;
            para.Alpha = 1f;
            curveGridContent.curDrawPara = para;
            curveGridContent.ModColor = new Color3(1f, 1f, 1f);
        }

        private void LoadText()
        {
            System.Drawing.Rectangle selectItemRect = new System.Drawing.Rectangle(382, 655, 260, 100);
            selectItemTB = new DrawTextBox(Game, device, "", UIFontName, 22, System.Drawing.Color.White, selectItemRect);
        }

        private void LoadGrid()
        {
            //新建Hero Grid
            int[] heroRowLens = new int[10] { 40, 40, 40, 40, 40, 40, 40, 40, 40, 40 };
            int[] heroColLens = new int[4] { 74, 132, 171, 223 };
            heroGrid = new DrawGrid(10, 4, heroRowLens, heroColLens, 212, 30);

            //添加最上面的"高分榜"
            DrawTextBox heroHeroTitle = new DrawTextBox(Game, device,
                StringTableManager.StringTable["GUI:RMHighscores"], UIFontName, 22, Color.Black);
            heroGrid.AddTextBox(0, 0, 0, 3, heroHeroTitle);

            //添加最上面的"名次"
            DrawTextBox heroRankTitle = new DrawTextBox(Game, device,
                StringTableManager.StringTable["GUI:RMRank"], UIFontName, 20, Color.White);
            heroGrid.AddTextBox(1, 1, 0, 0, heroRankTitle);

            //添加最上面的"姓名"
            DrawTextBox heroNameTitle = new DrawTextBox(Game, device,
                StringTableManager.StringTable["GUI:RMName"], UIFontName, 20, Color.White);
            heroGrid.AddTextBox(1, 1, 1, 1, heroNameTitle);

            //添加最上面的"用时"
            DrawTextBox heroTimeTitle = new DrawTextBox(Game, device,
                StringTableManager.StringTable["GUI:RMTime"], UIFontName, 20, Color.White);
            heroGrid.AddTextBox(1, 1, 2, 2, heroTimeTitle);

            //添加最上面的"等级"
            DrawTextBox heroLevelTitle = new DrawTextBox(Game, device,
                StringTableManager.StringTable["GUI:RMLevel"], UIFontName, 20, Color.White);
            heroGrid.AddTextBox(1, 1, 3, 3, heroLevelTitle);


            /******************   TEST ******************/
            string[] heroRankArr = new string[] { "1", "2", "3", "4" };
            string[] heroNameArr = new string[] { " Hust", " Jason", " Micheal", " Even" };
            string[] heroTimeArr = new string[] { "04:34'27\"", "05:45'19\"", "06:12'56\"", "09:05'88\"" };
            string[] heroLevelArr = new string[] { "环法高手", "疯狂的赛车", "校园车手", "送报小童" };
            for (int i = 2; i <= 5; i++)
            {
                System.Drawing.Color color;
                if (i == 2)
                {
                    color = Color.Red;
                }
                else
                {
                    color = Color.White;
                }
                //添加表格中的"名次"
                DrawTextBox heroRankTB = new DrawTextBox(Game, device, heroRankArr[i - 2], UIFontName, 20, color);
                heroGrid.AddTextBox(i, i, 0, 0, heroRankTB);

                //添加表格中的"姓名"
                DrawTextBox heroNameTB = new DrawTextBox(Game, device, heroNameArr[i - 2], UIFontName, 20, color, Rectangle.Empty, DrawTextFormat.Left | DrawTextFormat.SingleLine | DrawTextFormat.VerticalCenter);
                heroGrid.AddTextBox(i, i, 1, 1, heroNameTB);

                //添加表格中的"用时"
                DrawTextBox heroTimeTB = new DrawTextBox(Game, device, heroTimeArr[i - 2], UIFontName, 20, color);
                heroGrid.AddTextBox(i, i, 2, 2, heroTimeTB);

                //添加表格中的"等级"
                DrawTextBox heroLevelTB = new DrawTextBox(Game, device, heroLevelArr[i - 2], UIFontName, 20, color);
                heroGrid.AddTextBox(i, i, 3, 3, heroLevelTB);
            }

            /******************   TEST ******************/

            //新建Rank Grid
            int[] rankRowLens = new int[9] { 38, 38, 38, 38, 38, 38, 38, 38, 96 };
            int[] rankColLens = new int[3] { 175, 236, 189 };
            string[] rankRankArr = new string[] { "1", "2", "3", "4" };
            string[] rankNameArr = new string[] { " Hust", " Jackson", " Linken", " Green" };
            string[] rankTimeArr = new string[] { "04:34'27\"", "08:42'32\"", "08:57'21\"", "09:45'88\"" };
            rankGrid = new DrawGrid(9, 3, rankRowLens, rankColLens, 212, 30);

            //添加最上面的"名次"
            DrawTextBox rankRankTitle = new DrawTextBox(Game, device,
                StringTableManager.StringTable["GUI:RMRank"], UIFontName, 22, Color.Black);
            rankGrid.AddTextBox(0, 0, 0, 0, rankRankTitle);

            //添加最上面的"姓名"
            DrawTextBox rankNameTitle = new DrawTextBox(Game, device,
                StringTableManager.StringTable["GUI:RMName"], UIFontName, 20, Color.Black);
            rankGrid.AddTextBox(0, 0, 1, 1, rankNameTitle);

            //添加最上面的"用时"
            DrawTextBox rankTimeTitle = new DrawTextBox(Game, device,
                StringTableManager.StringTable["GUI:RMTime"], UIFontName, 20, Color.Black);
            rankGrid.AddTextBox(0, 0, 2, 2, rankTimeTitle);

            for (int i = 1; i <= 4; i++)
            {
                System.Drawing.Color color;
                if (i == 1)
                {
                    color = System.Drawing.Color.Red;
                }
                else
                {
                    color = System.Drawing.Color.White;
                }
                //添加表格中的"名次"
                DrawTextBox rankRankTB = new DrawTextBox(Game, device, rankRankArr[i - 1], UIFontName, 20, color);
                rankGrid.AddTextBox(i, i, 0, 0, rankRankTB);

                //添加表格中的"姓名"
                DrawTextBox rankNameTB = new DrawTextBox(Game, device, rankNameArr[i - 1], UIFontName, 20, color, Rectangle.Empty, DrawTextFormat.Left | DrawTextFormat.SingleLine | DrawTextFormat.VerticalCenter);
                rankGrid.AddTextBox(i, i, 1, 1, rankNameTB);

                //添加表格中的"用时"
                DrawTextBox rankTimeTB = new DrawTextBox(Game, device, rankTimeArr[i - 1], UIFontName, 20, color);
                rankGrid.AddTextBox(i, i, 2, 2, rankTimeTB);
            }

            DrawTextBox rankAlerTitle = new DrawTextBox(Game, device, "您本次比赛的评价为: 环法高手", UIFontName, 22, Color.Black);
            rankGrid.AddTextBox(8, 8, 0, 2, rankAlerTitle);
        }

        protected override void load()
        {
            LoadBackground();
            LoadIcons();
            LoadIconBg();
            LoadGridPic();
            LoadText();
            LoadGrid();

            Vector2 midLocation = new Vector2(512f, 568f);
            Vector2 currentLocation = midLocation;
            Vector2 selectSize = new Vector2(128f, 128f);
            Vector2 unSelectSize = new Vector2(100f, 100f);
            Vector2 twoPicNearDis = new Vector2(250f, 0);
            float unSelectAlpha = 0.5f;
            float selectAlpha = 0.9f;
            int placeTime = 0;
            for (int i = 0; i < pics.Count; i++)
            {
                if (i == 0)
                {
                    pics[i].curDrawPara = new MenuPicDrawPara(midLocation.X, midLocation.Y,
                                                                selectAlpha,
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
                        pics[i].curDrawPara = new MenuPicDrawPara(currentLocation.X, currentLocation.Y,
                                                                    unSelectAlpha,
                                                                    unSelectSize.X, unSelectSize.Y);
                    }
                    else
                    {
                        pics[i].curDrawPara = new MenuPicDrawPara(currentLocation.X, currentLocation.Y,
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

        protected override void unload()
        {
            if (picBackground != null)
            {
                picBackground.Dispose();
            }

            if (iconBg != null)
            {
                iconBg.Dispose();
            }

            if (heroGridPic != null)
            {
                heroGridPic.Dispose();
            }

            if (rankGridPic != null)
            {
                rankGridPic.Dispose();
            }

            for (int i = 0; i < pics.Count; i++)
            {
                if (pics[i] != null)
                {
                    pics[i].Dispose();
                }
            }
        }
        #endregion

        public override void NotifyActivated()
        {
            base.NotifyActivated();
            InputManager.Instance.ItemMoveLeft += this.Report_ItemMoveLeft;
            InputManager.Instance.ItemMoveRight += this.Report_ItemMoveRight;
            InputManager.Instance.Enter += this.Report_Enter;
            InputManager.Instance.Escape += this.Report_Enter;
        }
        public override void NotifyDeactivated()
        {
            base.NotifyDeactivated();
            InputManager.Instance.ItemMoveLeft -= this.Report_ItemMoveLeft;
            InputManager.Instance.ItemMoveRight -= this.Report_ItemMoveRight;
            InputManager.Instance.Enter -= this.Report_Enter;
            InputManager.Instance.Escape -= this.Report_Enter;
        }
    }
}
