using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.UI.Controls;

namespace VirtualBicycle.UI
{
    public abstract class ControlFactory
    {
        protected ControlFactory(string typeName)
        {
            TypeName = typeName;
        }

        public abstract Control CreateInstance(GameUI gameUI, string name);

        public string TypeName
        {
            get;
            private set;
        }
    }
   
    public class ControlManager
    {
        static ControlManager singleton;

        public static ControlManager Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new ControlManager();
                return singleton;
            }
        }

        Dictionary<string, ControlFactory> factories;

        private ControlManager()
        {
            factories = new Dictionary<string, ControlFactory>(CaseInsensitiveStringComparer.Instance);
        }

        public void RegisterControlType(string typeName, ControlFactory fac)
        {
            factories.Add(typeName, fac);
        }

        public void UnregisterControlType(string typeName)
        {
            factories.Remove(typeName);
        }

        public void UnregisterControlType(ControlFactory fac)
        {
            factories.Remove(fac.TypeName);
        }


        public Control CreateControl(GameUI gameUI, string type, string name)
        {
            ControlFactory fac;
            if (factories.TryGetValue(type, out fac))
            {
                return fac.CreateInstance(gameUI, name);
            }
            else
            {
                throw new NotSupportedException(type);
            }
        }
    }
}
