using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Logic.Traffic;

namespace VirtualBicycle.Logic.PathWay
{
    public class AITrafficComponet
    {
        #region Fields
        private List<AIPort> ports;
        public List<AIPort> Ports
        {
            get { return ports; }
            set { ports = value; }
        }

        /// <summary>
        /// 获取或设置该Componet的id
        /// </summary>
        private int uID;
        public int UID
        {
            get { return uID; }
            set { uID = value; }
        }

        /// <summary>
        /// 获取该物体的sourceTC
        /// </summary>
        private ITrafficComponment sourceTC;
        public ITrafficComponment SourceTC
        {
            get { return sourceTC; }
        }
        #endregion

        #region Constructor
        public AITrafficComponet(ITrafficComponment sourceTC)
        {
            this.sourceTC = sourceTC;
            ports = new List<AIPort>();
        }
        #endregion

        #region Methods

        #endregion
    }
}
