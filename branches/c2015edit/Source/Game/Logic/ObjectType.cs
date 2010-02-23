using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Config;

namespace VirtualBicycle.Logic
{
    public abstract class ObjectType : IConfigurable, IDisposable
    {
        #region 属性

        public string TypeName
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        #endregion

        #region 方法

        #region IConfigurable 成员

        public virtual void Parse(ConfigurationSection sect)
        {
            TypeName = sect.Name;

            Name = sect.GetString("Name", string.Empty);
        }

        #endregion

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

        #endregion
    }
}
