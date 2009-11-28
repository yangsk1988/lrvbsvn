using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Config;
using Apoc3D.Vfs;

namespace Apoc3D.Scene
{
    /// <summary>
    ///  管理游戏中所有物体类型，可从二进制数据反序列化物体
    /// </summary>
    public class ObjectTypeManager : IDisposable
    {
        #region 字段

        Dictionary<string, ObjectFactory> factories;

        #endregion


        public void RegisterObjectType(ObjectFactory fac)
        {
            factories.Add(fac.TypeTag, fac);
            EngineConsole.Instance.Write("找到物体类型 " + fac.TypeTag, ConsoleMessageType.Information);
        }

        public void UnregisterObjectType(ObjectFactory fac)
        {
            factories.Remove(fac.TypeTag);
        }

        public SceneObject DeserializeObject(string typeTag, BinaryDataReader data, SceneDataBase sceneData)
        {
            ObjectFactory fac;
            if (factories.TryGetValue(typeTag, out fac))
            {
                return fac.Deserialize(data, sceneData);
            }

            string msg = "无法加载物体：{0}。不支持这种类型。";
            EngineConsole.Instance.Write(string.Format(msg, typeTag), ConsoleMessageType.Error);
            throw new NotSupportedException(typeTag);
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
