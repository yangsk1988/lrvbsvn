using System;
using System.Collections.Generic;
using System.Text;
using MainLogic;
using SlimDX.Direct3D9;
using VirtualBicycle.Logic;

namespace VirtualBicycle
{
    public class LogicConponment : IDisposable
    {
        protected internal virtual void Initialize()
        {

        }
        protected internal virtual void WorldInitialize(InGameObjectManager manager)
        {

        }
        protected internal virtual void WorldLoaded(World world) 
        {

        }

        //protected internal virtual RenderOperation[] Render()
        //{
        //    return null;
        //}

        //protected internal virtual void Render(Sprite spr)
        //{

        //}

        protected internal virtual void Update(float dt)
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

        public Game Game
        {
            get { return GameMainLogic.Game; }
        }
    }
}
