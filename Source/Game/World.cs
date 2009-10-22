using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Collections;
using VirtualBicycle.CollisionModel.Broadphase;
using VirtualBicycle.CollisionModel.Dispatch;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;
using VirtualBicycle.Logic;
using VirtualBicycle.Logic.Mod;
using VirtualBicycle.Logic.Traffic;
using VirtualBicycle.MathLib;
using VirtualBicycle.Physics;
using VirtualBicycle.Physics.Dynamics;
using VirtualBicycle.Scene;

namespace VirtualBicycle
{
    public delegate void ProgressCallBack(int current, int total);

    public class WorldCreationParameters
    {
        public string LoadScreenConfig
        {
            get;
            set;
        }

        public string SceneDataFile
        {
            get;
            set;
        }

        public ProgressCallBack ProgressCallBack
        {
            get;
            set;
        }
    }

    /// <summary>
    ///  一场游戏
    /// </summary>
    public class World : IDisposable
    {
        #region 字段

        GameScene scene;

        Thread loadThread;
        Device device;
        Game game;

        DiscreteDynamicsWorld physWorld;

        object syncHelper = new object();

        //ExistTable<Cluster> phyClusterTable;
        //ExistTable<Cluster> phyClusterTableLast;
        //RigidBodyManager bodyManager;
        //FixedJointManager jointManager;

        //public RigidBodyManager BodyManager
        //{
        //    get { return bodyManager; }
        //}

        #endregion

        #region 属性

        public InGameObjectManager ObjectTypeManager
        {
            get;
            private set;
        }

        public DynamicsWorld PhysicsWorld
        {
            get { return physWorld; }
        }

        public WorldCreationParameters CreationParameters
        {
            get;
            private set;        
        }

        public bool IsLoaded
        {
            get;
            private set;
        }
        public bool IsUnloading
        {
            get;
            private set;
        }
        public bool IsValid
        {
            get { return IsLoaded && !(IsUnloading || Disposed); }
        }

        public GameScene Scene
        {
            get { return scene; }
        }

        public TrafficNet Traffic
        {
            get;
            private set;
        }

        public bool Paused
        {
            get;
            set;
        }

        #endregion

        public World(Game game, Device device, WorldCreationParameters pm)
        {
            this.CreationParameters = pm;
            this.device = device;
            this.game = game;

            //this.phyClusterTable = new ExistTable<Cluster>();
            //this.phyClusterTableLast = new ExistTable<Cluster>();
        }

        #region 方法
        public void Load()
        {
            loadThread = new Thread(load);
            loadThread.Name = "Game Loader";
            loadThread.SetApartmentState(ApartmentState.MTA);
            loadThread.Start();
        }

        void load()
        {
            lock (syncHelper)
            {
                FileLocation fl = FileSystem.Instance.Locate(
                      Path.Combine(Paths.Configs, "objects.ini"), FileLocateRules.Default);

                ObjectTypeManager = new InGameObjectManager(fl);

                if (CreationParameters.ProgressCallBack != null)
                    CreationParameters.ProgressCallBack(5, 100);

                Traffic = new TrafficNet();

                ObjectTypeManager.RegisterObjectType(new BuildingFactory(ObjectTypeManager));
                ObjectTypeManager.RegisterObjectType(new TerrainFactory(device, ObjectTypeManager));
                ObjectTypeManager.RegisterObjectType(new TerrainObjectFactory(ObjectTypeManager));
                ObjectTypeManager.RegisterObjectType(new RoadFactory(device, ObjectTypeManager, Traffic));
                ObjectTypeManager.RegisterObjectType(new JunctionFactory(ObjectTypeManager, Traffic));

                GameLogicManager.Instance.WorldInitialize(ObjectTypeManager);

                ProgressCallBack cbk1 = delegate(int c1, int t1)
                {
                    if (CreationParameters.ProgressCallBack != null)
                        CreationParameters.ProgressCallBack((int)(5 + (c1 / (float)t1) * 10), 100);
                };
            

                ObjectTypeManager.LoadGraphics(device, cbk1);
                // progress 15

                fl = FileSystem.Instance.Locate(
                   Path.Combine(Paths.Maps, CreationParameters.SceneDataFile), FileLocateRules.Default);


                cbk1 = delegate(int c1, int t1)
                {
                    if (CreationParameters.ProgressCallBack != null)
                        CreationParameters.ProgressCallBack((int)(15 + (c1 / (float)t1) * 45), 100);
                };

                SceneData data = SceneData.FromFile(device, ObjectTypeManager, fl, cbk1);
                // progress 60

                //testCamera = new ChaseCamera((float)game.ClientSize.Width / (float)game.ClientSize.Height);
                ////testCamera = new FpsCamera((float)game.ClientSize.Width / (float)game.ClientSize.Height);
                //testCamera.FarPlane = 600f;
                //testCamera.RenderTarget = device.GetRenderTarget(0);

                //testCamera.Orientation = Quaternion.RotationAxis(new Vector3(0, 1, 0), MathEx.PIf);
                //testCamera.Position = new Vector3(0, 15, 0);

                cbk1 = delegate(int c1, int t1)
                {
                    if (CreationParameters.ProgressCallBack != null)
                        CreationParameters.ProgressCallBack((int)(60 + (c1 / (float)t1) * 35), 100);
                };

                scene = new GameScene(device, data, cbk1);
                //scene.RegisterCamera(testCamera);
                // progress 95
                if (CreationParameters.ProgressCallBack != null)
                    CreationParameters.ProgressCallBack(97, 100);


                physWorld = new DiscreteDynamicsWorld(new CollisionDispatcher(),
                    new SimpleBroadphase(), 
                    new SequentialImpulseConstraintSolver());

                physWorld.Gravity = new Vector3(0, -10, 0);

                ClusterTable clusterTable = scene.ClusterTable;

                foreach (Cluster cl in clusterTable)
                {
                    // 为所有物体加载碰撞形体
                    List<SceneObject> objects = cl.SceneManager.SceneObjects;
                    for (int i = 0; i < objects.Count; i++)
                    {
                        if (objects[i].HasPhysicsModel)
                        {
                            objects[i].BuildPhysicsModel(physWorld);
                        }
                    }
                }

                Traffic.ParseNodes();

                GameLogicManager.Instance.WorldLoaded(this);

                Thread.Sleep(2000);

                IsLoaded = true;
            }
        }

        public void Unload()
        {
            IsUnloading = true;

            loadThread = new Thread(Dispose);
            loadThread.Name = "Game Unloader";
            loadThread.SetApartmentState(ApartmentState.MTA);
            loadThread.Start();
        }

        public void Render()
        {
            if (IsValid)
            {
                scene.RenderScene();
            }
        }

        public void Update(float dt)
        {
            if (IsValid && !Paused)
            {
                GameLogicManager.Instance.Update(dt);

                if (IsValid && !Paused)
                {

                    scene.Update(dt);

                    //UpdatePhysicsClusters();

                    physWorld.StepSimulation(dt, 5, 1f / 60f);

                }
            }
        }

        //public void LoadClusterPhysics(Cluster cluster)
        //{
        //    List<SceneObject> objects = cluster.SceneManager.SceneObjects;

        //    for (int i = 0; i < objects.Count; i++)
        //    {
        //        //RigidBody body = bodyManager.CreateBody();
        //        //objects[i].SetRigidBody(body);

        //        //if (objects[i].RequiresFixedJoint)
        //        //{
        //        //    FixedJoint joint = jointManager.CreateJoint();
        //        //    objects[i].SetFixedJoint(joint);
        //        //}
        //    }
        //}
        //public void UnloadClusterPhysics(Cluster cluster)
        //{
        //    List<SceneObject> objects = cluster.SceneManager.SceneObjects;

        //    for (int i = 0; i < objects.Count; i++)
        //    {
        //        //RigidBody body = objects[i].GetRigidBody();
        //        //bodyManager.DestoryBody(body);
        //        //objects[i].SetRigidBody(null);

        //        //if (objects[i].RequiresFixedJoint)
        //        //{
        //        //    FixedJoint joint = objects[i].GetFixedJoint();
        //        //    jointManager.DestoryJoint(joint);
        //        //    objects[i].SetFixedJoint(null);
        //        //}
        //    }
        //}

        //void UpdatePhysicsClusters()
        //{
        //    FastList<Cluster> cluster = scene.VisisbleClusters;

        //    // 对于这一帧的Cluster
        //    for (int i = 0; i < cluster.Count; i++)
        //    {
        //        // 如果上一帧没有这个Cluster
        //        if (!phyClusterTableLast.Exists(cluster[i]))
        //        {
        //            // 那么这个Cluster就是新增的
        //            LoadClusterPhysics(cluster[i]);
        //        }

        //        phyClusterTable.Add(cluster[i]);
        //    }

        //    // 对于上一帧的Cluster
        //    foreach (Cluster cl in phyClusterTableLast)
        //    {
        //        // 如果这一帧没有上一帧的Cluster
        //        if (!phyClusterTable.Exists(cl))
        //        {
        //            // 那么这个Cluster就是即将去掉的
        //            UnloadClusterPhysics(cl);
        //        }
        //    }

        //    // 交换
        //    ExistTable<Cluster> tmp = phyClusterTableLast;
        //    phyClusterTableLast = phyClusterTable;
        //    phyClusterTable = tmp;

        //    phyClusterTable.Clear();
        //}
        #endregion

        #region IDisposable 成员

        public bool Disposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                IsUnloading = true;
                GameLogicManager.Instance.WorldFinalize();

                scene.Dispose();
                scene = null;

                physWorld.Dispose(true);
                physWorld = null;

                ObjectTypeManager.Dispose();
                ObjectTypeManager = null;

                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion
    }
}
