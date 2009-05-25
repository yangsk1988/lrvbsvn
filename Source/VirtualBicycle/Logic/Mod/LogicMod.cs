using System;
using System.Collections.Generic;
using System.Text;
using SlimDX.Direct3D9;
using VirtualBicycle.Graphics;
using VirtualBicycle.Scene;
using VirtualBicycle.UI;

namespace VirtualBicycle.Logic.Mod
{
    public abstract class LogicMod : IDisposable
    {

        protected internal virtual void GameInitialize()
        {

        }

        protected internal virtual void WorldInitialize(InGameObjectManager manager)
        {

        }
        protected internal virtual void WorldLoaded(World world) 
        {

        }
        protected internal virtual void WorldFinalize()
        {

        }
        protected internal virtual void GameFinalize()
        {

        }

        protected internal virtual void Update(float dt)
        {

        }

        public static Game Game
        {
            get;
            internal set;
        }

        #region IDisposable 成员

        public bool Disposed
        {
            get;
            private set;
        }

        protected virtual void Dispose(bool disposing) { }

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

        ~LogicMod()
        {
            if (!Disposed)
            {
                Dispose(false);
                Disposed = true;
            }
        }

        #endregion
    }

    public abstract class LogicModFactroy 
    {
        public abstract LogicMod CreateInstance();
    }
}
