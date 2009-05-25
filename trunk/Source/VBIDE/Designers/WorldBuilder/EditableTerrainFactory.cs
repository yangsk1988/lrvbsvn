using System;
using System.Collections.Generic;
using System.Text;
using SlimDX.Direct3D9;
using VBIDE.Editors.EditableObjects;
using VirtualBicycle.IO;
using VirtualBicycle.Logic;
using VirtualBicycle.Scene;

namespace VBIDE.Designers.WorldBuilder
{
    class EditableTerrainFactory : TerrainFactory
    {
        public EditableTerrainFactory(Device device, InGameObjectManager mgr)
            : base(device, mgr)
        {
        }

        public override SceneObject Deserialize(BinaryDataReader data, SceneDataBase sceneData)
        {
            EditableTerrain terrain = new EditableTerrain(Device, sceneData.CellUnit, sceneData.TerrainSettings);
            terrain.LoadData(data);
            return terrain;
        }
    }
}
