using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SlimDX.Direct3D9;
using VirtualBicycle.Graphics;
using VirtualBicycle.Logic.PathWay;
using VirtualBicycle.Logic.Traffic;
using VirtualBicycle.Scene;
using SD = System.Drawing;

namespace VirtualBicycle.Logic
{
    public class MiniMap
    {
        #region Fields
        private GameScene gameScene;

        private Dictionary<ClusterDescription, GameTexture> miniMaps;
        public Dictionary<ClusterDescription, GameTexture> MiniMaps
        {
            get { return miniMaps; }
        }

        private AIPathManager pathManager;
        #endregion

        #region Constructor
        public MiniMap(AIPathManager pathManager)
        {
            this.pathManager = pathManager;
        }
        #endregion

        #region Methods
        private void Load()
        {
            Dictionary<ClusterDescription, Bitmap> mBitmaps;
            mBitmaps = new Dictionary<ClusterDescription, Bitmap>();
            ClusterTable table = gameScene.ClusterTable;
            for (int i = 0; i < pathManager.AITrafficComponets.Count; i++)
            {
                AITrafficComponet aiTC = pathManager.AITrafficComponets[i];
                if (aiTC is AIRoad)
                {
                    AIRoad aiRoad = (AIRoad)aiTC;
                    Road road = (Road)aiTC.SourceTC;
                    ClusterDescription description = road.ParentCluster.Description;
                    Bitmap bitmap = null;

                    if (!mBitmaps.TryGetValue(description, out bitmap))
                    {
                        bitmap = new Bitmap(Terrain.TerrainSize, Terrain.TerrainSize);
                        mBitmaps.Add(description, bitmap);
                    }

                    if (bitmap != null)
                    {
                        SD.Graphics graphic = SD.Graphics.FromImage(bitmap);
                        for (int ptI = 0; ptI < aiRoad.RoadData.RoadMiddle.Count - 1; i++)
                        {
                            RoadNode firstNode = aiRoad.RoadData.RoadMiddle[ptI];
                            RoadNode nextNode = aiRoad.RoadData.RoadMiddle[ptI + 1];

                            Point firstPoint = new Point((int)(firstNode.Position.X - road.OffsetX), (int)(firstNode.Position.Z - road.OffsetZ));
                            Point nextPoint = new Point((int)(nextNode.Position.X - road.OffsetX), (int)(nextNode.Position.Z - road.OffsetZ));
                            Pen pen = new Pen(Brushes.White, (firstNode.Width + nextNode.Width / 2));
                            graphic.DrawLine(pen, firstPoint, nextPoint);
                            pen.Dispose();
                        }

                        graphic.Dispose();
                    }
                }
            }
            Dictionary<ClusterDescription, Bitmap>.ValueCollection vals = mBitmaps.Values;
            foreach (Bitmap bitmap in vals)
            {
                //toDo
            }
        }


        private void Unload()
        {

        }
        #endregion
    }
}
