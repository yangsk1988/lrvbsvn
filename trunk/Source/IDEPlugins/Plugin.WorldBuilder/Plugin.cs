using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Plugin.ModelTools;
using VirtualBicycle;
using VirtualBicycle.Graphics.Effects;
using VirtualBicycle.Ide;

namespace Plugin.WorldBuilder
{
    public class Plugin : IPlugin
    {
        #region IPlugin 成员


        public void Load()
        {
            DesignerManager.Instance.RegisterDesigner(new WorldDesignerFactory());

            TemplateManager.Instance.RegisterTemplate(new WorldTemplate());

            IdeLogicModManager.Initialize();


            EffectManager.Instance.RegisterModelEffectType(TerrainRenderingEditEffectFactory.Name,
                new TerrainRenderingEditEffectFactory(GraphicsDevice.Instance.Device));
        }

        public void Unload()
        {
        }

        public string Name
        {
            get { return "World Builder"; }
        }

        public Icon PluginIcon
        {
            get { return null; }
        }

        public bool IsListed
        {
            get { return true; }
        }

        #endregion
    }
}
