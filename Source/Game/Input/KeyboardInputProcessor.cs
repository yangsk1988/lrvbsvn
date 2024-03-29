﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using VirtualBicycle.MathLib;
using System.Windows.Forms;
using System.Drawing;

namespace VirtualBicycle.Input
{
    public class KeyboardInputProcessor : InputProcessor
    {
        enum VKeys : short
        {
            VK_LBUTTON = 1,
            VK_RBUTTON = 2,
            VK_CANCEL = 3,
            VK_MBUTTON = 4,  //   NOT contiguous with L & RBUTTON  
            VK_XBUTTON1 = 5,
            VK_XBUTTON2 = 6,
            VK_BACK = 8,
            VK_TAB = 9,
            VK_CLEAR = 12,
            VK_RETURN = 13,
            VK_SHIFT = 0x10,
            VK_CONTROL = 17,
            VK_MENU = 18,
            VK_PAUSE = 19,
            VK_CAPITAL = 20,
            VK_KANA = 21,
            VK_HANGUL = 21,
            VK_JUNJA = 23,
            VK_FINAL = 24,
            VK_HANJA = 25,
            VK_KANJI = 25,
            VK_CONVERT = 28,
            VK_NONCONVERT = 29,
            VK_ACCEPT = 30,
            VK_MODECHANGE = 31,
            VK_ESCAPE = 27,
            VK_SPACE = 0x20,
            VK_PRIOR = 33,
            VK_NEXT = 34,
            VK_END = 35,
            VK_HOME = 36,
            VK_LEFT = 37,
            VK_UP = 38,
            VK_RIGHT = 39,
            VK_DOWN = 40,
            VK_SELECT = 41,
            VK_PRINT = 42,
            VK_EXECUTE = 43,
            VK_SNAPSHOT = 44,
            VK_INSERT = 45,
            VK_DELETE = 46,
            VK_HELP = 47,
            VK_A = 65,
            VK_D = 68,
            VK_S = 83,
            VK_W = 87,

            //VK_0 thru VK_9 are the same as ASCII '0' thru '9' ($30 - $39)  
            //VK_A thru VK_Z are the same as ASCII 'A' thru 'Z' ($41 - $5A)  
            VK_LWIN = 91,
            VK_RWIN = 92,
            VK_APPS = 93,
            VK_SLEEP = 95,
            VK_NUMPAD0 = 96,
            VK_NUMPAD1 = 97,
            VK_NUMPAD2 = 98,
            VK_NUMPAD3 = 99,
            VK_NUMPAD4 = 100,
            VK_NUMPAD5 = 101,
            VK_NUMPAD6 = 102,
            VK_NUMPAD7 = 103,
            VK_NUMPAD8 = 104,
            VK_NUMPAD9 = 105,
            VK_MULTIPLY = 106,
            VK_ADD = 107,
            VK_SEPARATOR = 108,
            VK_SUBTRACT = 109,
            VK_DECIMAL = 110,
            VK_DIVIDE = 111,
            VK_F1 = 112,
            VK_F2 = 113,
            VK_F3 = 114,
            VK_F4 = 115,
            VK_F5 = 116,
            VK_F6 = 117,
            VK_F7 = 118,
            VK_F8 = 119,
            VK_F9 = 120,
            VK_F10 = 121,
            VK_F11 = 122,
            VK_F12 = 123,
            VK_F13 = 124,
            VK_F14 = 125,
            VK_F15 = 126,
            VK_F16 = 127,
            VK_F17 = 128,
            VK_F18 = 129,
            VK_F19 = 130,
            VK_F20 = 131,
            VK_F21 = 132,
            VK_F22 = 133,
            VK_F23 = 134,
            VK_F24 = 135,
            VK_NUMLOCK = 144,
            VK_SCROLL = 145,
            // VK_L & VK_R - left and right Alt, Ctrl and Shift virtual keys.
            // Used only as parameters to GetAsyncKeyState() and GetKeyState().
            // No other API or message will distinguish left and right keys in this way.  
            VK_LSHIFT = 160,
            VK_RSHIFT = 161,
            VK_LCONTROL = 162,
            VK_RCONTROL = 163,
            VK_LMENU = 164,
            VK_RMENU = 165,

            VK_BROWSER_BACK = 166,
            VK_BROWSER_FORWARD = 167,
            VK_BROWSER_REFRESH = 168,
            VK_BROWSER_STOP = 169,
            VK_BROWSER_SEARCH = 170,
            VK_BROWSER_FAVORITES = 171,
            VK_BROWSER_HOME = 172,
            VK_VOLUME_MUTE = 173,
            VK_VOLUME_DOWN = 174,
            VK_VOLUME_UP = 175,
            VK_MEDIA_NEXT_TRACK = 176,
            VK_MEDIA_PREV_TRACK = 177,
            VK_MEDIA_STOP = 178,
            VK_MEDIA_PLAY_PAUSE = 179,
            VK_LAUNCH_MAIL = 180,
            VK_LAUNCH_MEDIA_SELECT = 181,
            VK_LAUNCH_APP1 = 182,
            VK_LAUNCH_APP2 = 183,

            VK_OEM_1 = 186,
            VK_OEM_PLUS = 187,
            VK_OEM_COMMA = 188,
            VK_OEM_MINUS = 189,
            VK_OEM_PERIOD = 190,
            VK_OEM_2 = 191,
            VK_OEM_3 = 192,
            VK_OEM_4 = 219,
            VK_OEM_5 = 220,
            VK_OEM_6 = 221,
            VK_OEM_7 = 222,
            VK_OEM_8 = 223,
            VK_OEM_102 = 226,
            VK_PACKET = 231,
            VK_PROCESSKEY = 229,
            VK_ATTN = 246,
            VK_CRSEL = 247,
            VK_EXSEL = 248,
            VK_EREOF = 249,
            VK_PLAY = 250,
            VK_ZOOM = 251,
            VK_NONAME = 252,
            VK_PA1 = 253,
            VK_OEM_CLEAR = 254
        }

        [DllImport("user32")]
        static extern bool GetAsyncKeyState(VKeys vKey);
        [DllImport("user32")]
        static extern bool GetAsyncKeyState(short vKey);

        bool isVPressed;
        bool isEscPressed;
        bool isEnterPressed;

        bool isTPressed;

        float handlebarAngle;

        Game game;
        bool usesMouseDirection;

        public KeyboardInputProcessor(InputManager mgr, Game game, bool usesMouseDirection)
            : base(mgr)
        {
            this.game = game;
            this.usesMouseDirection = usesMouseDirection;
        }

        public override void Update(float dt)
        {
            if (GetAsyncKeyState((short)'R'))
            {
                Manager.OnReset();
            }

            if (usesMouseDirection)
            {
                int width = game.Window.Width;
                Point pos = game.Window.PointToClient(Control.MousePosition);

                float xratio = (pos.X / (float)width);
                xratio = MathEx.Clamp(0, 1, xratio); 

                int dir = xratio < 0.5f ? -1 : 1;

                xratio = xratio * 2 - 1;

                float angle = 1 - (float)Math.Sqrt(1 - xratio * xratio); //inv sphere

                angle = angle * 0.5f * dir * MathEx.PIf;

                float da = angle - handlebarAngle;

                if (da != 0)
                {
                    Manager.OnHandlebarRotated(angle, da);

                    handlebarAngle = angle;
                }

                if (GetAsyncKeyState(VKeys.VK_A) || GetAsyncKeyState(VKeys.VK_LEFT))
                {
                    Manager.OnItemMoveLeft();
                }
                if (GetAsyncKeyState(VKeys.VK_D) || GetAsyncKeyState(VKeys.VK_RIGHT))
                {
                    Manager.OnItemMoveRight();
                }
            }
            else
            {
                if (GetAsyncKeyState(VKeys.VK_A) || GetAsyncKeyState(VKeys.VK_LEFT))
                {
                    handlebarAngle -= 0.005f;

                    if (handlebarAngle < -MathEx.PiOver2)
                    {
                        handlebarAngle = -MathEx.PiOver2;
                    }

                    Manager.OnItemMoveLeft();
                    Manager.OnHandlebarRotated(handlebarAngle, -0.005f);
                }

                if (GetAsyncKeyState(VKeys.VK_D) || GetAsyncKeyState(VKeys.VK_RIGHT))
                {
                    handlebarAngle += 0.005f;
                    if (handlebarAngle > MathEx.PiOver2)
                    {
                        handlebarAngle = MathEx.PiOver2;
                    }

                    Manager.OnItemMoveRight();
                    Manager.OnHandlebarRotated(handlebarAngle, 0.005f);
                }
            }

            if (GetAsyncKeyState(VKeys.VK_W) || GetAsyncKeyState(VKeys.VK_UP))
            {
                Manager.OnWheelSpeedChanged(0, 1, true);
            }

            if (GetAsyncKeyState(VKeys.VK_S) || GetAsyncKeyState(VKeys.VK_DOWN))
            {
                Manager.OnWheelSpeedChanged(0, -1, true);
            }

            if (GetAsyncKeyState((short)'T'))
            {
                if (!isTPressed)
                {
                    Manager.OnHeartPulse();
                    isTPressed = true;
                }
            }
            else 
            {
                isTPressed = false;
            }

            if (GetAsyncKeyState(VKeys.VK_ESCAPE))
            {
                if (!isEscPressed)
                {
                    Manager.OnEscape();
                    isEscPressed = true;
                }
            }
            else
            {
                isEscPressed = false;
            }

            if (GetAsyncKeyState(VKeys.VK_RETURN))
            {
                if (!isEnterPressed)
                {
                    Manager.OnEnter();
                    isEnterPressed = true;
                }
            }
            else 
            {
                isEnterPressed = false;
            }

            if (GetAsyncKeyState((short)'V'))
            {
                if (!isVPressed)
                {
                    Manager.OnViewChanged();
                    isVPressed = true;
                }
            }
            else 
            {
                isVPressed = false;
            }
        }
    }
}
