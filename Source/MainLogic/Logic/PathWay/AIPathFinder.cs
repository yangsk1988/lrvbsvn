using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using VirtualBicycle.DataStructure;
namespace VirtualBicycle.Logic.PathWay
{
    public class AStarNode : IComparable<AStarNode>
    {
        public float Cost;
        public float H;
        public int CurUID;
        public AStarNode PrevNode;

        public AStarNode(int curUID)
        {
            Cost = 0.0f;
            H = 0.0f;
            CurUID = curUID;
            PrevNode = null;
        }

        public AStarNode(int cur,AStarNode prev)
        {
            CurUID = cur;
            PrevNode = prev;
            Cost = 0.0f;
            H = 0.0f;
        }

        public int CompareTo(AStarNode obj)
        {
            return (Cost + H).CompareTo(obj.Cost + obj.H);
        }
    }

    public class AStarResult
    {
        public List<int> Route;
        public float Cost;

        public AStarResult()
        {
            Route = null;
            Cost = 0.0f;
        }
    }

    public class AIPathFinder
    {
        #region Fields
        //保存PathManager
        private AIPathManager manager;

        //保存路口的hash表
        private bool[] hash;
        #endregion

        #region Constructor
        public AIPathFinder(AIPathManager manager)
        {
            this.manager = manager;
        }
        #endregion

        #region Methods

        #region GetWay
        /// <summary>
        /// 找寻两个Componet之间的道路
        /// </summary>
        /// <param name="sourceComp">源组件</param>
        /// <param name="sourcePos">源位置</param>
        /// <param name="targetComp">目标组件</param>
        /// <param name="targetPos">目标位置</param>
        /// <returns>记录经过的Port编号的列表,从起点到终点</returns>
        public List<int> GetWay(AITrafficComponet sourceComp,Vector3 sourcePos, 
            AITrafficComponet targetComp,Vector3 targetPos)
        {
            List<AIPort> sourceAIPorts = sourceComp.Ports;
            List<AIPort> targetAIPorts = targetComp.Ports;
            AStarResult curResult = new AStarResult();
            AStarResult minimumCostResult = new AStarResult();
            minimumCostResult.Cost = float.MaxValue;

            //算法要点:
            //分枚举位于sourceComp和targetComp的Port
            //找到一条路径使得总的长度最短
            for (int tmpSourceID = 0; tmpSourceID < sourceAIPorts.Count; tmpSourceID++)
            {
                for (int tmpTargetID = 0; tmpTargetID < targetAIPorts.Count; tmpTargetID++)
                {
                    float costStart;
                    float costEnd;
                    float totalCost;
                    curResult = GetWay(sourceAIPorts[tmpSourceID].UID, targetAIPorts[tmpTargetID].UID);
                    //如果找到了一条路
                    if (curResult.Route != null)
                    {
                        //计算当前点到这个SourcePort和TargetPort到目标点的cost
                        costStart = GetCost(sourcePos, tmpSourceID);
                        costEnd = GetCost(tmpTargetID, targetPos);
                        totalCost = costStart + curResult.Cost + costEnd;
                        if (totalCost < minimumCostResult.Cost)
                        {
                            minimumCostResult = curResult;
                        }
                    }
                }
            }

            return minimumCostResult.Route;
        }

        /// <summary>
        /// 获取某个点在摸个AITrafficComponet的索引
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        private int GetIndex(Vector3 pos,AITrafficComponet comp)
        {
            const float minConsideredDis = 10f; 

            if (comp is AIRoad)
            {
                AIRoad road = (AIRoad)comp;
                float minDis = float.MaxValue;
                int curIndex = -1;

                for (int i = 0; i < road.RoadData.RoadMiddle.Count; i++)
                {
                    if (Vector3.Distance(road.RoadData.RoadMiddle[i].Position, pos) < minDis)
                    {
                        minDis = Vector3.Distance(road.RoadData.RoadMiddle[i].Position, pos);
                        curIndex = i;
                    }
                }

                if (minConsideredDis > minDis)
                {
                    return curIndex;
                }
            }

            return -1;
        }

        /// <summary>
        /// 得到从同一个物体中的一个点的另一个点的路线
        /// </summary>
        /// <param name="sourcePos"></param>
        /// <param name="targetPos"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        private List<Vector3> GetWay(Vector3 sourcePos, Vector3 targetPos, AITrafficComponet comp)
        {
            List<Vector3> path = new List<Vector3>();
            if (comp is AIRoad)
            {
                AIRoad road = (AIRoad)comp;
                int curIndex = GetIndex(sourcePos, comp);
                int nextIndex = GetIndex(targetPos, comp);

                if (curIndex < nextIndex)
                {
                    for (int j = curIndex; j <= nextIndex; j++)
                    {
                        path.Add(road.RoadData.RoadMiddle[j].Position);
                    }
                }
                else
                {
                    for (int j = nextIndex; j <= curIndex; j++)
                    {
                        path.Add(road.RoadData.RoadMiddle[j].Position);
                    }
                }
            }
            return path;
        }

        /// <summary>
        /// 得到从一个物体的一个点到另一个物体的一个点的路线
        /// </summary>
        /// <param name="sourcePos"></param>
        /// <param name="sourceComp"></param>
        /// <param name="targetPos"></param>
        /// <param name="targetComp"></param>
        /// <param name="portList"></param>
        /// <returns></returns>
        private List<Vector3> GetWay(Vector3 sourcePos, AITrafficComponet sourceComp, Vector3 targetPos,
            AITrafficComponet targetComp, List<int> portList)
        {
            const float minIntervalDis = 10f;

            List<Vector3> path = new List<Vector3>();

            AIPort port;

            Vector3 curPos = Vector3.Zero;
            Vector3 nextPos = Vector3.Zero;
            AITrafficComponet curComp = null;
            AITrafficComponet nextComp = null;

            float curDis = 0f;
            if (portList.Count != 0)
            {
                for (int i = 0; i < portList.Count; i++)
                {
                    if (i <= 0)
                    {
                        curPos = sourcePos;
                        curComp = sourceComp;
                    }
                    else
                    {
                        port = manager.GetPortOfUID(portList[i - 1]);
                        curPos = port.Position;
                        curComp = port.Owner;
                    }

                    port = manager.GetPortOfUID(portList[i]);
                    nextPos = port.Position;
                    nextComp = port.Owner;

                    //如果是在同一个物体中
                    if (curComp == nextComp)
                    {
                        List<Vector3> pathOfPointPart = GetWay(curPos, nextPos, curComp);
                        Vector3 lastPoint = Vector3.Zero;
                        for (int j = 0; j < pathOfPointPart.Count; j++)
                        {
                            if (lastPoint != Vector3.Zero)
                            {
                                curDis += Vector3.Distance(lastPoint, pathOfPointPart[j]);
                                if (curDis > minIntervalDis)
                                {
                                    path.Add(pathOfPointPart[j]);
                                    curDis = 0f;
                                }
                                lastPoint = pathOfPointPart[j];
                            }
                            else
                            {
                                path.Add(pathOfPointPart[j]);
                            }
                        }
                    }
                }

                port = manager.GetPortOfUID(portList[portList.Count - 1]);
                curPos = port.Position;
                curComp = port.Owner;

                nextPos = targetPos;
                nextComp = targetComp;
            }
            else
            {
                curComp = sourceComp;
                nextComp = sourceComp;
                curPos = sourcePos;
                nextPos = targetPos;
            }


            //如果是在同一个物体中
            if ((curComp == nextComp) || (portList.Count == 0))
            {
                List<Vector3> pathOfPointPart = GetWay(curPos, nextPos, curComp);
                Vector3 lastPoint = Vector3.Zero;
                for (int j = 0; j < pathOfPointPart.Count; j++)
                {
                    if (lastPoint != Vector3.Zero)
                    {
                        curDis += Vector3.Distance(lastPoint, pathOfPointPart[j]);
                        if (curDis > minIntervalDis)
                        {
                            path.Add(pathOfPointPart[j]);
                            curDis = 0f;
                        }
                        lastPoint = pathOfPointPart[j];
                    }
                    else
                    {
                        path.Add(pathOfPointPart[j]);
                        lastPoint = pathOfPointPart[j];
                    }
                }
            }

            return path;
        }

        /// <summary>
        /// 根据起点和终点找出一条点的路径
        /// 目前还很简陋,只能寻找出有道路组成的点路径
        /// </summary>
        /// <param name="sourcePos">起点</param>
        /// <param name="targetPos">终点</param>
        /// <returns></returns>
        public List<Vector3> GetWay(Vector3 sourcePos, Vector3 targetPos)
        {
            const float minConsideredDis = 1f;

            AITrafficComponet sourceComp = null;
            AITrafficComponet targetComp = null;
            
            float minStartDis = float.MaxValue;
            float minEndDis = float.MaxValue;

            //第一步,找到起点和终点所在的Componet
            for (int i = 0; i < manager.AITrafficComponets.Count; i++)
            {
                if (manager.AITrafficComponets[i] is AIRoad)
                {
                    AIRoad road = (AIRoad)manager.AITrafficComponets[i];
                    for (int j = 0; j < road.RoadData.RoadMiddle.Count; j++)
                    {
                        if (Vector3.Distance(road.RoadData.RoadMiddle[j].Position, sourcePos) < minStartDis)
                        {
                            minStartDis = Vector3.Distance(road.RoadData.RoadMiddle[j].Position, sourcePos);
                            sourceComp = manager.AITrafficComponets[i];
                        }

                        if (Vector3.Distance(road.RoadData.RoadMiddle[j].Position, targetPos) < minEndDis)
                        {
                            minEndDis = Vector3.Distance(road.RoadData.RoadMiddle[j].Position, targetPos);
                            targetComp = manager.AITrafficComponets[i];
                        }
                    }
                }
                else if (manager.AITrafficComponets[i] is AIJunction)
                {
                    //add code
                }
            }

            if (minStartDis > minConsideredDis)
            {
                throw new Exception("起点不位于任何一个AITrafficComponet");
            }

            if (minEndDis > minConsideredDis)
            {
                throw new Exception("起点不位于任何一个AITrafficComponet");
            }

            //第二步,对得到的数据进行一次寻路,找出经过的Port列表
            List<int> portList = GetWay(sourceComp, sourcePos, targetComp, targetPos);

            List<Vector3> path = GetWay(sourcePos, sourceComp, targetPos, targetComp, portList);

            return path;
        }

        /// <summary>
        /// 找寻两个Port之间的路
        /// </summary>
        /// <param name="sourcePort">起点Port编号</param>
        /// <param name="targetPort">终点Port编号</param>
        /// <returns></returns>
        public AStarResult GetWay(int sourcePortID, int targetPortID)
        {
            //using AStar
            return AStarPathFind(sourcePortID, targetPortID);
        }
        #endregion

        #region AStar

        #region GetHeuristic
        /// <summary>
        /// 得到两个Port之间的预测函数值(使用直接得到距离)
        /// </summary>
        /// <param name="currentPortID">当前PortID</param>
        /// <param name="targetPortID">目标PortID</param>
        /// <returns></returns>
        public float GetHeuristicDistance(int currentPortID,int targetPortID)
        {
            AIPort targetPort = manager.GetPortOfUID(targetPortID);
            AIPort curPort = manager.GetPortOfUID(currentPortID);

            return Vector3.Distance(targetPort.Position, curPort.Position);
        }

        /// <summary>
        /// 得到两个AStarNode之间的预测函数值
        /// </summary>
        /// <param name="sourcePortNode">当前AstarNode</param>
        /// <param name="targetPortNode">目标AStarNode</param>
        /// <returns></returns>
        public float GetHeuristic(AStarNode sourcePortNode,AStarNode targetPortNode)
        {
            return GetHeuristicDistance(sourcePortNode.CurUID, targetPortNode.CurUID);
        }

        /// <summary>
        /// 得到两个Port之间的预测函数值
        /// 调用相应的预测函数
        /// </summary>
        /// <param name="sourcePortID">当前PortID</param>
        /// <param name="targetPortID">目标PortID</param>
        /// <returns></returns>
        public float GetHeuristic(int sourcePortID,int targetPortID)
        {
            return GetHeuristicDistance(sourcePortID, targetPortID);
        }
        #endregion

        #region GetCost
        /// <summary>
        /// 得到从当前位置到目标Port的路径消耗
        /// </summary>
        /// <param name="curPos"></param>
        /// <param name="targetPortID"></param>
        /// <returns></returns>
        public float GetCost(Vector3 sourcePos, int targetPortID)
        {
            AIPort port = manager.GetPortOfUID(targetPortID);
            return Vector3.Distance(sourcePos, port.Position);
        }

        /// <summary>
        /// 得到从当前Port到目标位置的路径消耗
        /// </summary>
        /// <param name="sourcePortID"></param>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        public float GetCost(int sourcePortID, Vector3 targetPos)
        {
            AIPort port = manager.GetPortOfUID(sourcePortID);
            return Vector3.Distance(port.Position, targetPos);
        }
        #endregion

        #region Get Route
        public List<int> GetRoute(AStarNode node)
        {
            AStarNode curNode = node;
            List<int> route = new List<int>();
            while (curNode != null)
            {
                route.Add(curNode.CurUID);
                curNode = curNode.PrevNode;
            }
            route.Reverse();
            return route;
        }
        #endregion

        public AStarResult AStarPathFind(int sourcePortID, int targetPortID)
        {
            #region Variables
            bool[] hash = new bool[manager.AIPorts.Count];
            AStarResult result = new AStarResult();
            result.Route = new List<int>();
            #endregion

            #region Initialize
            AIPort sourcePort = manager.GetPortOfUID(sourcePortID);
            PriorityQueue<AStarNode> priorQueue;
            priorQueue = new PriorityQueue<AStarNode>();
            AStarNode startingNode = new AStarNode(sourcePortID);

            startingNode.H = GetHeuristic(sourcePortID,targetPortID);
            priorQueue.Push(startingNode);
            hash[startingNode.CurUID] = true;
            #endregion

            #region Starting AStar..
            while (!priorQueue.Empty())
            {
                //第一步,当前队列中的第一个元素出队
                AStarNode headNode = priorQueue.Top();
                AIPort headPort = manager.GetPortOfUID(headNode.CurUID);
                priorQueue.Pop();

                //枚举headPort中的Edges,循环加入优先队列
                List<AIEdge> headEdges = headPort.Edges;
                for (int i = 0; i < headEdges.Count; i++)
                {
                    AIEdge curEdge = headEdges[i];
                    AIPort tailPort = curEdge.TargetPort;
                    //如果这条边的状态是打开,而且另一端的Port没有在队列里面出现过
                    if ((curEdge.EdgeState == EnumEdgeState.Open) && (!hash[tailPort.UID]))
                    {
                        //则将它添加进队列
                        AStarNode tailNode = new AStarNode(tailPort.UID, headNode);
                        hash[tailPort.UID] = true;

                        //计算其Cost和H
                        tailNode.Cost = headNode.Cost + curEdge.Cost;
                        tailNode.H = GetHeuristic(tailNode.CurUID,targetPortID);

                        //检查是否已经找到了目标点
                        if (tailNode.CurUID == targetPortID)
                        {
                            result.Route = GetRoute(tailNode);
                            result.Cost = tailNode.Cost;
                            return result;
                        }
                        priorQueue.Push(tailNode);
                    }
                }
                
            }
            #endregion

            //如果没有找到相应的路径,则返回空
            return result ;
        }
        #endregion

        #endregion
    }
}
