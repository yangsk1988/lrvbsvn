using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SlimDX.Direct3D9;
using SlimDX.DirectInput;
using VirtualBicycle.Config;

namespace VirtualBicycle
{

    public delegate void LoadHandler(object sender);

    public delegate void CloseHandler(object sender);
    public delegate void UpdateHandler(object sender, float dt);
    //public delegate void PaintHandler();
    public delegate void PaintHandler2D(Sprite spr);

    public delegate void MouseHandler(object sender, MouseEventArgs e);

    //public delegate void KeyHandler(object sender, KeyCollection pressed);

    public delegate void ParseHander(object sender, ConfigurationSection sect);
    
}
