using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle.Config
{
    public abstract class Configuration : Dictionary<string, ConfigurationSection>
    {
        string name;

        protected Configuration(string name, IEqualityComparer<string> comparer)
            : base(comparer)
        {
            this.name = name;
        }


        protected Configuration(string name, int cap, IEqualityComparer<string> comparer)
            : base(cap, comparer)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }

        public abstract Configuration Clone();
        //public abstract void AppendContent(ResourceLocation rl); 
        public abstract void Merge(Configuration config);
    }
}
