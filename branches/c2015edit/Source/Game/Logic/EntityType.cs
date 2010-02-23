using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SlimDX.Direct3D9;
using VirtualBicycle.Config;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;

namespace VirtualBicycle.Logic
{
    public class EntityType : ObjectType
    {
        #region 属性

        public string ModelName
        {
            get;
            protected set;
        }

        public string LodModelName
        {
            get;
            protected set;
        }

        public Model Model
        {
            get;
            protected set;
        }

        public Model LodModel
        {
            get;
            protected set;
        }

        #endregion

        #region IConfigurable 成员

        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            ModelName = sect["Model"];
            LodModelName = sect.GetString("LodModel",
                Path.GetFileNameWithoutExtension(ModelName) + "_lod" + Path.GetExtension(ModelName));
        }

        #endregion

        #region 方法

        public void LoadGraphics(Device device)
        {
            string fileName = Path.Combine(Paths.Models, ModelName);

            FileLocation fl = FileSystem.Instance.Locate(fileName, FileLocateRules.Default);
            Model = ModelManager.Instance.CreateInstance(device, fl);

            if (!string.IsNullOrEmpty(LodModelName))
            {
                fileName = Path.Combine(Paths.Models, LodModelName);

                fl = FileSystem.Instance.TryLocate(fileName, FileLocateRules.Default);
                if (fl != null)
                {
                    LodModel = ModelManager.Instance.CreateInstance(device, fl);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Model != null)
                {
                    ModelManager.Instance.DestoryInstance(Model);
                }
                if (LodModel != null)
                {
                    ModelManager.Instance.DestoryInstance(LodModel);
                }
            }
            Model = null;
            LodModel = null;
        }

        #endregion
    }
}
