using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Logic.Traffic;

namespace VirtualBicycle.Logic.PathWay
{
    public struct AIRoadData
    {
        /// <summary>
        /// 得到左边的样条
        /// </summary>
        private List<RoadNode> leftSide;
        public List<RoadNode> LeftSide
        {
            get { return leftSide; }
        }

        /// <summary>
        /// 得到右边的样条
        /// </summary>
        private List<RoadNode> rightSide;
        public List<RoadNode> RightSide
        {
            get { return rightSide; }
        }

        private List<RoadNode> roadMiddle;
        public List<RoadNode> RoadMiddle
        {
            get { return roadMiddle; }
            set { roadMiddle = value; }
        }

        public AIRoadData(List<RoadNode> left, List<RoadNode> right)
        {
            leftSide = left;
            rightSide = right;
            roadMiddle = null;
        }
    }

    public class AIRoad : AITrafficComponet
    {
        #region Fields
        /// <summary>
        /// 得到道路的数据信息
        /// </summary>
        private AIRoadData roadData;
        public AIRoadData RoadData
        {
            get { return roadData; }
        }

        #endregion

        #region Constructor
        public AIRoad(ITrafficComponment sourceTC)
            : base(sourceTC)
        {
            Road sourceTrack = (Road)sourceTC;
            RoadLine trackLine = sourceTrack.TrackLine;
            this.roadData = new AIRoadData();
            this.roadData.RoadMiddle = trackLine.InterposedPoints;
            
        }
        #endregion

        #region Methods

        #endregion
    }
}
