using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle;
using VirtualBicycle.Logic;
using VirtualBicycle.Scene;

namespace VirtualBicycle.ObjectMgr
{
    public class ObjectCreator
    {
        private Device device;
        private World world;
        private ObjectManager manager;

        public ObjectCreator(Device device,World world,ObjectManager manager)
        {
            this.device = device;
            this.world = world;
            this.manager = manager;
        }

        public bool CreateCar(Vector3 pos)
        {
            throw new NotImplementedException();
        }

        public Bicycle CreateBicycle(Vector3 pos)
        {
            int clusterX;
            int clusterY;
            GameScene scene = world.Scene;

            Bicycle bicycle = new Bicycle(device);
            bicycle.Position = pos;

            scene.GetClusterCoord(pos.X, pos.Y, out clusterX, out clusterY);
            ClusterDescription clusterDes = new ClusterDescription(clusterX, clusterY);
            Cluster cluster = scene.ClusterTable[clusterDes];

            //bicycle.BuildCollisionModel(world.CollisionSpace);
            cluster.SceneManager.AddObjectToScene(bicycle);

            manager.AddBicycle(bicycle);
            return bicycle;
        }

        public bool CreatePedestrian(Vector2 pos)
        {
            throw new NotImplementedException();
        }
    }
}
