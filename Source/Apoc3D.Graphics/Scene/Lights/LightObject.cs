using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Vfs;

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

        #region SceneObject 序列化
        public override bool IsSerializable
        {
            get { return true; }
        }

        public override void Serialize(BinaryDataWriter data)
        {

        }
        public override string TypeTag
        {
            get { return base.TypeTag; }
        }
        #endregion

        public override RenderOperation[] GetRenderOperation()
        {
            return null;
        }
        public override void Update(float dt)
        {
            
        }
    }
}
