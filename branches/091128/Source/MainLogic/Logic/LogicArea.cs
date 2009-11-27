using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using VirtualBicycle.CollisionModel;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic
{
    public class LogicalArea : SceneObject
    {
        #region Fields
        private string typeName;
        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }

        private float radius;
        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        #endregion

        #region Constructor
        public LogicalArea(float radius, Vector3 pos,string name)
            : base(false)
        {
            this.radius = radius;
            this.position = pos;
            this.typeName = name;
        }
        #endregion

        #region SceneObject 序列化

        public override void Serialize(BinaryDataWriter data)
        {
            LogicalAreaFactory.Serialize(this, data);
        }

        public override bool IsSerializable
        {
            get { return true; }
        }

        public override string TypeTag
        {
            get
            {
                return LogicalAreaFactory.TypeId;
            }
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