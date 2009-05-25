using System;
using System.Collections.Generic;
using System.Text;
using SlimDX.Direct3D9;
using VirtualBicycle.Config;
using VirtualBicycle.IO;
using VirtualBicycle.Scene;
using VirtualBicycle.Logic.Traffic;

namespace VirtualBicycle.Logic
{
    public class InGameObjectManager : IDisposable
    {
        #region 字段

        Dictionary<string, InGameObjectFactory> factories;

        #endregion

        #region 属性

        public Dictionary<string, BuildingType> BuildingTypes
        {
            get;
            set;
        }
        public Dictionary<string, TerrainObjectType> TreeTypes
        {
            get;
            set;
        }
        public Dictionary<string, TerrainObjectType> TOTypes
        {
            get;
            set;
        }
        public Dictionary<string, JunctionType> CrossingTypes
        {
            get;
            set;
        }

        #endregion

        #region 构造函数

        public InGameObjectManager(FileLocation file)
        {
            factories = new Dictionary<string, InGameObjectFactory>();

            BuildingTypes = new Dictionary<string, BuildingType>();
            TreeTypes = new Dictionary<string, TerrainObjectType>();
            TOTypes = new Dictionary<string, TerrainObjectType>();
            CrossingTypes = new Dictionary<string, JunctionType>();


            Configuration config = ConfigurationManager.Instance.CreateInstance(file);

            ConfigurationSection sect = config["BuildingTypes"];

            ConfigurationSection.ValueCollection vals = sect.Values;
            foreach (string typeName in vals)
            {
                ConfigurationSection tSect = config[typeName];

                BuildingType type = new BuildingType();

                type.Parse(tSect);

                BuildingTypes.Add(typeName, type);
            }

            sect = config["TreeTypes"];
            vals = sect.Values;

            foreach (string typeName in vals)
            {
                ConfigurationSection tSect = config[typeName];

                TerrainObjectType type = new TerrainObjectType();

                type.Parse(tSect);

                TreeTypes.Add(typeName, type);
            }

            sect = config["TOTypes"];
            vals = sect.Values;

            foreach (string typeName in vals)
            {
                ConfigurationSection tSect = config[typeName];

                TerrainObjectType type = new TerrainObjectType();

                type.Parse(tSect);

                TOTypes.Add(typeName, type);
            }

            sect = config["CrossingTypes"];
            vals = sect.Values;

            foreach (string typeName in vals)
            {
                ConfigurationSection tSect = config[typeName];

                JunctionType type = new JunctionType();

                type.Parse(tSect);

                CrossingTypes.Add(typeName, type);
            }
        }

        #endregion

        #region 方法

        public void LoadGraphics(Device device, ProgressCallBack cbk)
        {
            Dictionary<string, BuildingType>.ValueCollection vals1 = BuildingTypes.Values;
            Dictionary<string, TerrainObjectType>.ValueCollection vals2 = TreeTypes.Values;

            int total = vals1.Count + vals2.Count;
            int current = 0;

            foreach (BuildingType bld in vals1)
            {
                bld.LoadGraphics(device);
                if (cbk != null)
                    cbk(current++, total);
            }
            string msg = "读取了 {0} 种建筑物模型。";
            EngineConsole.Instance.Write(string.Format(msg, vals1.Count.ToString()), ConsoleMessageType.Information);

            foreach (TerrainObjectType obj in vals2)
            {
                obj.LoadGraphics(device);

                if (cbk != null)
                    cbk(current++, total);
            }
            msg = "读取了 {0} 种地形物体模型。";
            EngineConsole.Instance.Write(string.Format(msg, vals2.Count.ToString()), ConsoleMessageType.Information);

            vals2 = TOTypes.Values;
            foreach (TerrainObjectType obj in vals2)
            {
                obj.LoadGraphics(device);
            }

            Dictionary<string, JunctionType>.ValueCollection vals3 = CrossingTypes.Values;
            foreach (JunctionType obj in vals3)
            {
                obj.LoadGraphics(device);
            }
        }

        public JunctionType GetCrossingType(string typeName)
        {
            JunctionType result;
            CrossingTypes.TryGetValue(typeName, out result);
            return result;
        }

        public BuildingType GetBuildingType(string typeName)
        {
            BuildingType result;
            BuildingTypes.TryGetValue(typeName, out result);
            return result;
        }

        public TerrainObjectType GetTreeType(string typeName)
        {
            TerrainObjectType result;
            TreeTypes.TryGetValue(typeName, out result);
            return result;
        }

        public TerrainObjectType GetTOType(string typeName)
        {
            TerrainObjectType result;
            TOTypes.TryGetValue(typeName, out result);
            return result;
        }

        public void RegisterObjectType(InGameObjectFactory fac)
        {
            factories.Add(fac.TypeTag, fac);
            EngineConsole.Instance.Write("找到物体类型 " + fac.TypeTag, ConsoleMessageType.Information);
        }

        public void UnregisterObjectType(InGameObjectFactory fac)
        {
            factories.Remove(fac.TypeTag);
        }

        public SceneObject DeserializeObject(string typeTag, BinaryDataReader data, SceneDataBase sceneData)
        {
            InGameObjectFactory fac;
            if (factories.TryGetValue(typeTag, out fac))
            {
                return fac.Deserialize(data, sceneData);
            }

            string msg = "无法加载物体：{0}。不支持这种类型。";
            EngineConsole.Instance.Write(string.Format(msg, typeTag), ConsoleMessageType.Error);
            throw new NotSupportedException(typeTag);
        }

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
                Dictionary<string, BuildingType>.ValueCollection vals1 = BuildingTypes.Values;
                Dictionary<string, TerrainObjectType>.ValueCollection vals2 = TreeTypes.Values;

                int total = vals1.Count + vals2.Count;

                foreach (BuildingType bld in vals1)
                {
                    bld.Dispose();
                }
                foreach (TerrainObjectType obj in vals2)
                {
                    obj.Dispose();
                }
                vals2 = TOTypes.Values;
                foreach (TerrainObjectType obj in vals2)
                {
                    obj.Dispose();
                }

                Dictionary<string, JunctionType>.ValueCollection vals3 = CrossingTypes.Values;
                foreach (JunctionType obj in vals3)
                {
                    obj.Dispose();
                }

                BuildingTypes.Clear();
                TreeTypes.Clear();
                TOTypes.Clear();
                CrossingTypes.Clear();

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
