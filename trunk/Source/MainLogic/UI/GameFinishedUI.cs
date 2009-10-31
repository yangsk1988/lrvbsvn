using System;
using System.Collections.Generic;
using System.Text;
using MainLogic;
using SlimDX;
using SlimDX.Direct3D9;
using System.Drawing;

namespace VirtualBicycle.UI
{
    public class GameFinishedUI : UIComponent
    {
        static readonly string UIFontName = "微软雅黑";

        Device device;
        GameUI gameUI;
        GameMainLogic logic;
        World world;
        IngameUI ingameUI;

        DrawTextBox message;
        MenuPic backgroundImage;

        float remainingTime;

        public bool Finished
        {
            get;
            private set;
        }

        public GameFinishedUI(Game game, World world, GameMainLogic logic, GameUI gameUI, IngameUI ingameUI)
            : base(game)
        {
            this.device = game.Device;
            this.world = world;
            this.gameUI = gameUI;
            this.logic = logic;


            int fontWidth = 450;
            int fontHeight = 50;
            int posX = 1024 / 2 - fontWidth / 2;
            int posY = 768 / 2 - fontHeight / 2;
            Rectangle timeRect = new Rectangle(posX, posY, fontWidth, fontHeight);

            this.message = new DrawTextBox(game, device, "", UIFontName, 32, System.Drawing.Color.White, timeRect, DrawTextFormat.Center | DrawTextFormat.VerticalCenter);

            backgroundImage = new MenuPic(Game, "finishedMsgBg.png", "Select Map");

            MenuPicDrawPara para;
            para.Alpha = 0.5f;
            para.desiredWidth = 500;
            para.desiredHeight = 100;
            para.PosX = 1024f / 2;
            para.PosY = 768f / 2;

            backgroundImage.curDrawPara = para;
            backgroundImage.nextDrawPara = MenuPicDrawPara.Zero;
            backgroundImage.ModColor = Color.White;

            this.ingameUI = ingameUI;
            this.remainingTime = 4;
        }

        protected override void update(float dt)
        {
            remainingTime -= dt;
            if (remainingTime < 0)
            {
                Finished = true;
                remainingTime = 0;
            }
        }

        protected override void render(Sprite sprite)
        {
            backgroundImage.Render(sprite);
            if (logic.CurrentCompetition.CannotWin)
            {
                this.message.Render(sprite, "游戏结束。电脑赢了", true);
            }
            else
            {
                this.message.Render(sprite, "恭喜，你赢了！", true);
            }

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
}
