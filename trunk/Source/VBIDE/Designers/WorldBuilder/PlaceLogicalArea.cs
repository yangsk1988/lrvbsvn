using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using VBIDE.Editors.EditableObjects;
using VBIDE.Properties;
using VBIDE.Tools;
using VirtualBicycle.MathLib;
using VirtualBicycle.Scene;

namespace VBIDE.Designers.WorldBuilder
{
    class PlaceLogicalArea : WBToolBoxItem
    {
        public WorldDesigner WorldBuilder
        {
            get;
            private set;
        }

        public PlaceLogicalArea(WorldDesigner wb)
        {
            WorldBuilder = wb;
        }

        public override void Render()
        {

        }

        public override Image Icon
        {
            get { return Resources.New; }
        }

        public override string Name
        {
            get { return "逻辑区域"; }
        }

        public override ToolBoxCategory Category
        {
            get { return ToolCategories.Instance.Logic; }
        }

        public override void NotifyMouseDown(MouseEventArgs e)
        {

        }

        public override void NotifyMouseUp(MouseEventArgs e)
        {

        }

        public override void NotifyMouseMove(MouseEventArgs e)
        {

        }

        public override void NotifyMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                LineSegment ray = WorldBuilder.GetPickRay(e.X, e.Y);

                if (WorldBuilder.CurrentTerrain != null)
                {
                    Vector3 pos;
                    if (WorldBuilder.CurrentTerrain.Intersects(ray, out pos))
                    {
                        EditableCluster cluster = WorldBuilder.CurrentCluster;
                        SceneManagerBase sceMgr = cluster.SceneManager;

                        FakeLogicalArea loga = new FakeLogicalArea(10, pos, "NoName");

                        sceMgr.AddObjectToScene(loga);
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                WorldBuilder.CurrentToolBoxItem = null;
            }
        }

        public override void NotifyMouseDoubleClick(MouseEventArgs e)
        {

        }

        public override void NotifyMouseWheel(MouseEventArgs e)
        {

        }

        public override void NotifyActivated()
        {
            base.NotifyActivated();

            WorldBuilder.CurrentToolBoxItem = this;
        }
    }
}
