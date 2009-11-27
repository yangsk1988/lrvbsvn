using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MainLogic;
using SlimDX.Direct3D9;
using VirtualBicycle.Logic;

namespace VirtualBicycle.UI
{
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
            while (qVelocity.Count > 10)
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

}
