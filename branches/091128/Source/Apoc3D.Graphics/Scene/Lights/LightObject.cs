using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;

namespace Apoc3D.Scene
{
    public abstract class LightObject : SceneObject
    {
        Light light;

        protected Light Light
        {
            get { return light; }
        }

        public LightObject() 
            : base(false)
        {
            light = new Light();
        }

        public LightObject(Light light)
            : base(false)
        {
            this.light = light;
        }
    }
}
