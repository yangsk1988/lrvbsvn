using System;
using System.Collections.Generic;
using System.Text;
using MainLogic;
using SlimDX.Direct3D9;
using VirtualBicycle.Logic.Mod;
using VirtualBicycle.UI;

namespace VirtualBicycle
{
    public class UILogic : LogicConponment
    {
        StartScreen startScreen;
        MainMenu mainMenu;
        OptionScreen optScreen;
        MapSelectScreen mapSelectMenu;
        ReportScreen reportScreen;

        GameMainLogic logic;

        public ReportScreen GetReportScreen() 
        {
            if (reportScreen == null) 
            {
                reportScreen = new ReportScreen(Game, Game.GameUI, logic);
            }
            return reportScreen;
        }
        public MapSelectScreen GetMapSelectScreen()
        {
            if (mapSelectMenu == null) 
            {
                mapSelectMenu = new MapSelectScreen(Game, Game.GameUI, logic);
            }
            return mapSelectMenu;
        }
        public StartScreen GetStartScreen()
        {
            if (startScreen == null) 
            {
                startScreen = new StartScreen(Game, Game.GameUI, logic);
            }
            return startScreen;
        }
        public MainMenu GetMainMenu()
        {
            if (mainMenu == null)
            {
                mainMenu = new MainMenu(Game, Game.GameUI, logic);
            }
            return mainMenu;
        }
        public OptionScreen GetOptionScreen()
        {
            if (optScreen == null) 
            {
                optScreen = new OptionScreen(Game, Game.GameUI, logic);
            }
            return optScreen;
        }

        public UILogic(GameMainLogic logic)
        {
            this.logic = logic;
        }

        protected internal override void Initialize()
        {
            base.Initialize();

            startScreen = GetStartScreen();
            Game.GameUI.CurrentComponent = startScreen;
        }

        protected internal override void Update(float dt)
        {
            base.Update(dt);
        }
    }
}
