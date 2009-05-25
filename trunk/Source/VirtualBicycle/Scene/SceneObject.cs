using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SlimDX;
using VirtualBicycle.Collections;
using VirtualBicycle.CollisionModel.Dispatch;
using VirtualBicycle.CollisionModel.Shapes;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;
using VirtualBicycle.MathLib;
using VirtualBicycle.Physics;
using VirtualBicycle.Physics.Dynamics;

namespace VirtualBicycle.Scene
{
    /// <summary>
    /// 场景中的对象
    /// </summary>
    public abstract class SceneObject : IRenderable, IUpdatable, IDisposable
    {
        #region 物理相关

        RigidBody rigidBody;

        [Browsable(false)]
        public RigidBody RigidBody
        {
            get { return rigidBody; }
            protected set { rigidBody = value; }
        }

        [Browsable(false)]
        public virtual bool HasPhysicsModel
        {
            get { return false; }
        }

        public virtual void BuildPhysicsModel(DynamicsWorld world)
        {
            throw new NotSupportedException();
        }

        public float AngularFactor
        {
            get
            {
                if (RigidBody == null)
                    return 0;
                return rigidBody.AngularFactor;
            }
            set { rigidBody.AngularFactor = value; }
        }

        public ContactSolverType FrictionSolverType
        {
            get
            {
                if (RigidBody == null)
                    return ContactSolverType.Default;
                return rigidBody.FrictionSolverType;
            }
            set { rigidBody.FrictionSolverType = value; }
        }

        public ContactSolverType ContactSolverType
        {
            get
            {
                if (RigidBody == null)
                    return ContactSolverType.Default;
                return RigidBody.ContactSolverType;
            }
            set { rigidBody.ContactSolverType = value; }
        }

        public float Restitution
        {
            get
            {
                if (RigidBody == null)
                    return 0;
                return RigidBody.Restitution;
            }
            set { RigidBody.Restitution = value; }
        }
        public float Friction
        {
            get
            {
                if (RigidBody == null)
                    return 0; 
                return RigidBody.Friction;
            }
            set { RigidBody.Friction = value; }
        }
        public void SetDamping(float linDamping, float angDamping)
        {
            RigidBody.SetDamping(linDamping, angDamping);
        }

        #endregion

        #region 字段

        Cluster parentCluster;

        public BoundingSphere BoundingSphere;

        public Matrix Transformation = Matrix.Identity;

        #endregion

        #region 属性

        /// <summary>
        ///  由Cluster造成的偏移
        /// </summary>
        [Browsable(false)]
        public float OffsetX
        {
            get;
            set;
        }
        [Browsable(false)]
        public float OffsetY
        {
            get;
            set;
        }
        [Browsable(false)]
        public float OffsetZ
        {
            get;
            set;
        }

        /// <summary>
        ///  获取或设置物体是否需要更新
        /// </summary>
        /// <remarks>当变换矩阵变化以后为true</remarks>
        [Browsable(false)]
        public bool RequiresUpdate
        {
            get;
            set;
        }

        [Browsable(false)]
        public GameScene GameScene
        {
            get;
            private set;
        }

        [Browsable(false)]
        public SceneNodeBase ParentSceneNode
        {
            get;
            protected internal set;
        }

        [Browsable(false)]
        public Cluster ParentCluster
        {
            get { return parentCluster; }
            set
            {
                parentCluster = value;
                if (value != null)
                {
                    GameScene = value.GameScene;
                }
            }
        }

        /// <summary>
        /// 获取该SceneObject是否包含子物体
        /// </summary>
        [Browsable(false)]
        public bool HasSubObjects
        {
            get;
            protected set;
        }

        #endregion

        #region 构造函数

        protected SceneObject(bool hasSubObject)
        {
            HasSubObjects = hasSubObject;
        }

        /// <summary>
        ///  如果该SceneObject包含子物体，准备摄像机的可见物体
        /// </summary>
        /// <param name="cam">
        ///  摄像机
        /// </param>
        public virtual void PrepareVisibleObjects(ICamera cam)
        {
            throw new NotSupportedException();
        }

        public virtual void OnAddedToScene(object sender, OctreeSceneManager sceneMgr) { }
        public virtual void OnRemovedFromScene(object sender, OctreeSceneManager sceneMgr) { }


        public virtual bool IntersectsSelectionRay(ref Ray ray)
        {
            return MathEx.BoundingSphereIntersects(ref BoundingSphere, ref ray);
        }

        #endregion

        #region IRenderable 成员

        public abstract RenderOperation[] GetRenderOperation();


        #endregion

        #region IUpdatable 成员

        public abstract void Update(float dt);

        #endregion

        #region SceneObject 序列化

        [Browsable(false)]
        public abstract bool IsSerializable
        {
            get;
        }

        public virtual void Serialize(BinaryDataWriter data)
        {
            throw new NotSupportedException();
        }

        [Browsable(false)]
        public virtual string TypeTag
        {
            get { return "Unknown"; }
        }
        #endregion

        #region IDisposable 成员

        protected virtual void Dispose(bool disposing)
        {

        }

        [Browsable(false)]
        public bool Disposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                Dispose(true);

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
