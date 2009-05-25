using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;

namespace VirtualBicycle.Logic.Competition
{
    public class Vector3SortedXComparer : IComparer<Vector3>
    {
        public int Compare(Vector3 x, Vector3 y)
        {
            // Compare y and x in reverse order.
            if (x.X < y.X)
            {
                return -1;
            }
            else if (x.X == y.X)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }

    public class PassedPointsMgr
    {
        #region Fields
        List<Vector3> xSortedData;
        Dictionary<Vector3, int> vectorToIndex;
        List<BicyclePassedPoints> bicyclePassedPointsData;
        List<Bicycle> bicycles;

        BicyclePassedPoints playerBicyclePP;
        Bicycle playerBicycle;
        List<EndData> endData;

        float usedTime;
        #endregion

        #region Constructor
        public PassedPointsMgr(List<Vector3> path,List<Bicycle> bicycles,List<EndData> endData)
        {
            this.xSortedData = path;
            this.bicycles = bicycles;
            this.endData = endData;
            usedTime = 0f;

            Initialize();
        }
        #endregion

        #region Methods
        private void Initialize()
        {
            //对输入的数据把序列作为Hash表保存到Dictionary中
            vectorToIndex = new Dictionary<Vector3, int>();
            for (int i = 0; i < xSortedData.Count; i++)
            {
                vectorToIndex.Add(xSortedData[i], i);
            }

            //对输入的数据按照x排序
            xSortedData.Sort(new Vector3SortedXComparer());

            ///*******************************TEST*/
            //List<Vector3> vec3s = getNearestVector3s(new Vector3(22f, 0.89f, 185.78f));
            ///*******************************TEST*/


            bicyclePassedPointsData = new List<BicyclePassedPoints>();

            for (int i = 0 ; i < bicycles.Count; i ++)
            {
                BicyclePassedPoints bpp = new BicyclePassedPoints(bicycles[i], this);
                bicyclePassedPointsData.Add(bpp);

                if (bicycles[i].OwnerType == Bicycle.BicycleOwner.Player)
                {
                    playerBicyclePP = bpp;
                    playerBicycle = bicycles[i];
                }
            }

            playerBicycle.Rank = playerBicycle.BicycleMgr.BicycleList.Count;
        }
        /// <summary>
        /// 得到路径中距离当前的点距离小于minConsideredDis的点集合
        /// </summary>
        /// <param name="curPos"></param>
        /// <returns></returns>
        public List<Vector3> getNearestVector3s(Vector3 curPos)
        {
            List<Vector3> vector3s = new List<Vector3>();
            const float minConsideredDis = 4f;
            int index = xSortedData.BinarySearch(curPos,new Vector3SortedXComparer());
            int loopTime = 0;
            if (index < xSortedData.Count)
            {
                if (index < 0)
                {
                    index = ~index;
                }
                int curIndex = index;
               
                while ((Math.Abs(xSortedData[curIndex].X - curPos.X) < minConsideredDis) && (curIndex > 0))
                {
                    loopTime++;
                    float dis = Vector3.Distance(xSortedData[curIndex], curPos);
                    if (dis < minConsideredDis)
                    {
                        vector3s.Add(xSortedData[curIndex]);
                    }

                    if (curIndex > 0)
                    {
                        curIndex--;
                    }
                    else
                    {
                        break;
                    }
                }

                curIndex = index + 1;
                while ((Math.Abs(xSortedData[curIndex].X - curPos.X)  < minConsideredDis) && (curIndex < xSortedData.Count))
                {
                    loopTime++;
                    float dis = Vector3.Distance(xSortedData[curIndex], curPos);
                    if (dis < minConsideredDis)
                    {
                        vector3s.Add(xSortedData[curIndex]);
                    }
                    if (curIndex < xSortedData.Count)
                    {
                        curIndex++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return vector3s;
        }

        public void Update(float dt)
        {
            usedTime += dt;
            //领先于玩家控制的自行车的数量
            int bicylceCountLead = 0;

            //更新每个Bicycle当前到达的点
            for (int i = 0; i < bicyclePassedPointsData.Count; i++)
            {
                bicyclePassedPointsData[i].Update(usedTime);
            }

            int playerLastPassedPointIndex = -1;
            float playerLastPassedPointTime = float.MaxValue;

            if (playerBicyclePP.LastPassedPoint != Vector3.Zero)
            {
                if (!vectorToIndex.TryGetValue(playerBicyclePP.LastPassedPoint,out playerLastPassedPointIndex))
                {
                    throw new Exception("经过点不为零但是在Dictionary中找不到改点的Index");
                }
                if (!playerBicyclePP.VectorToArriveTime.TryGetValue(playerBicyclePP.LastPassedPoint, out playerLastPassedPointTime))
                {
                    throw new Exception("经过点不为零但是在Dictionary中找不到改点的Time");
                }
            }

            //查找每个Bicycle的到达点的时间
            for (int i = 0; i < bicyclePassedPointsData.Count; i++)
            {
                if (bicyclePassedPointsData[i] != playerBicyclePP)
                {
                    int curIndex;
                    if (vectorToIndex.TryGetValue(bicyclePassedPointsData[i].LastPassedPoint, out curIndex))
                    {
                        if (curIndex > playerLastPassedPointIndex)
                        {
                            bicylceCountLead++;
                        }
                        else if (curIndex == playerLastPassedPointIndex)
                        {
                            float curTime;
                            if (!bicyclePassedPointsData[i].VectorToArriveTime.TryGetValue(bicyclePassedPointsData[i].LastPassedPoint, out curTime))
                            {
                                throw new Exception("经过点不为零但是在Dictionary中找不到改点的Time");
                            }
                            if (curTime < playerLastPassedPointTime)
                            {
                                bicylceCountLead++;
                            }
                        }
                    }
                }
            }

            //设置playerBicycle当前的名次
            playerBicycle.Rank = bicylceCountLead++;

            //判断每辆车是否到达了终点
            for (int i = 0; i < bicyclePassedPointsData.Count; i ++)
            {
                if (bicyclePassedPointsData[i].ArriveTargetTime < 0f)
                {
                    Dictionary<Vector3, float> vectorToArriveTime = bicyclePassedPointsData[i].VectorToArriveTime;

                    for (int j = 0; j < endData.Count; j++)
                    {
                        float arriveTime;
                        if (vectorToArriveTime.TryGetValue(endData[j].Position, out arriveTime))
                        {
                            bicyclePassedPointsData[i].ArriveTargetTime = arriveTime;
                            break;
                        }
                    }
                }
            }
        }
        #endregion
    }

    public class BicyclePassedPoints
    {
        #region Fields
        //到达某个点的时间
        private Dictionary<Vector3, float> vectorToArriveTime;
        public Dictionary<Vector3, float> VectorToArriveTime
        {
            get { return vectorToArriveTime; }
        }

        private Bicycle owner;
        public Bicycle Owner
        {
            get
            {
                return Owner;
            }
        }

        //这个可以作为自行车摔倒后的依据
        //同时也可以作为自行车排名的依据
        private Vector3 lastPassedPoint;
        public Vector3 LastPassedPoint
        {
            get { return lastPassedPoint; }
        }

        public float ArriveTargetTime
        {
            get;
            set;
        }

        PassedPointsMgr ppMgr;
        #endregion

        #region Constructor
        public BicyclePassedPoints(Bicycle owner,PassedPointsMgr ppMgr)
        {
            vectorToArriveTime = new Dictionary<Vector3, float>();
            this.owner = owner;
            this.ppMgr = ppMgr;
            lastPassedPoint = Vector3.Zero;
            ArriveTargetTime = -1f;

        }
        #endregion

        #region Methods
        public void Update(float usedTime)
        {
            List<Vector3> vec3List = ppMgr.getNearestVector3s(owner.Position);

            //每次更新的时候都把自行车经过的点的时间都记录下来
            if (vec3List.Count > 0)
            {
                for (int i = 0; i < vec3List.Count; i++)
                {
                    float time;
                    if (!vectorToArriveTime.TryGetValue(vec3List[i], out time))
                    {
                        vectorToArriveTime.Add(vec3List[i], usedTime);
                    }
                }
            }
        }
        #endregion
    }
}
