using System;
using System.Collections.Generic;
using System.Text;

namespace Apoc3D.Graphics
{
    public abstract class UnmanagedResource : IUnmanagedResource, IDisposable
    {
        static List<UnmanagedResource> resources = new List<UnmanagedResource>();

        public static void LoadAll()
        {
            for (int i = 0; i < resources.Count; i++)
            {
                resources[i].LoadUnmanagedResources();
            }
        }
        public static void UnloadAll()
        {
            for (int i = 0; i < resources.Count; i++)
            {
                resources[i].UnloadUnmanagedResources();
            }
        }

        protected UnmanagedResource()
        {
            resources.Add(this);
        }

        protected abstract void loadUnmanagedResources();
        protected abstract void unloadUnmanagedResources();

        #region IDisposable 成员

        public bool Disposed
        {
            get;
            private set;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) 
            {
                UnloadUnmanagedResources();
            }
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                resources.Remove(this);

                Dispose(true);
                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion

        public bool IsLoaded
        {
            get;
            private set;
        }

        #region IUnmanagedResource 成员

        public void LoadUnmanagedResources()
        {
            if (!IsLoaded)
            {
                loadUnmanagedResources();
                IsLoaded = true;
            }
        }

        public void UnloadUnmanagedResources()
        {
            if (IsLoaded)
            {
                unloadUnmanagedResources();
                IsLoaded = false;
            }
        }

        #endregion
    }
    public interface IUnmanagedResource
    {
        void LoadUnmanagedResources();
        void UnloadUnmanagedResources();
    }
}
