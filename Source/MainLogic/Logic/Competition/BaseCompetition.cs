using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Scene;
using SlimDX.Direct3D9;
using VirtualBicycle.Input;

namespace VirtualBicycle.Logic.Competition
{
    public class BaseCompetition : IDisposable
    {
        protected World world;

        protected Device device;

        protected Game game;

        protected ViewManager playerView;

        //protected int CompetitionID;
        public bool CannotWin
        {
            get;
            protected set;
        }

        public Bicycle CurrentBicycle
        {
            get;
            protected set;
        }

        public SceneObject[] SceneObjects
        {
            get;
            private set;
        }
        public List<LogicalArea> LogicalAreas
        {
            get;
            private set;
        }

        public BaseCompetition(Game game, World world, List<LogicalArea> logicalAreas, SceneObject[] sceneObjects)
        {
            this.SceneObjects = sceneObjects;
            this.LogicalAreas = logicalAreas;
            this.world = world;
            this.device = game.Device;
            this.game = game;

            this.LogicalAreas = new List<LogicalArea>();

            //this.CompetitionID = CID;
            Initialize();
        }

        protected virtual void InitCamera()
        {

        }

        public virtual void Initialize()
        {
            InitCamera();
        }

        public virtual void Update(float dt)
        {

        }

        public virtual void Unload()
        {

        }

        #region IDisposable 成员

        public bool Disposed
        {
            get;
            private set;
        }

        protected virtual void Dispose(bool disposing)
        {

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
