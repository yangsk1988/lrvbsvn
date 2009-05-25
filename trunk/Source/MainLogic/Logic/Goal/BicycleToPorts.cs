using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Scene;
using VirtualBicycle.Logic.PathWay;
using SlimDX;
using VirtualBicycle.Logic.Traffic;

namespace VirtualBicycle.Logic.Goal
{
    public class BicycleToPorts : BaseGoal
    {
        #region Fields
        private Queue<int> goalPorts;
        public Queue<int> GoalPorts
        {
            get { return goalPorts; }
            set { goalPorts = value; }
        }

        private Bicycle bicycle;

        private AIPathManager pathManager;

        private AIPathFinder pathFinder;

        private AITrafficComponet prevTC;
        private int prevPortUID;

        private int startPort;
        private int endPort;

        private int curGoalPortUID;
        #endregion

        #region Constructor
        public BicycleToPorts(DynamicObject obj, GoalManager mgr, 
            BaseGoal fatherGoal, AIPathManager pathManager,int startPort,int endPort)
            : base(obj, mgr, fatherGoal)
        {
            bicycle = (Bicycle)obj;
            this.pathManager = pathManager;
            this.pathFinder = pathManager.PathFinder;
            this.startPort = startPort;
            this.endPort = endPort;
        }
        #endregion

        #region Methods
        private bool CheckIsCurrentPortReached()
        {
            AIPort curGoalPort = pathManager.GetPortOfUID(curGoalPortUID);
            if (Vector3.Distance(curGoalPort.Position, bicycle.Position) <= 4)
            {
                return true;
            }

            return false;
        }

        public override void Activate()
        {
            AStarResult result;
            isActived = true;
            AIPathFinder pathFinder = pathManager.PathFinder;
            result = pathFinder.GetWay(startPort, endPort);
            goalPorts = new Queue<int>();
            if (result.Route != null)
            {
                for (int i = 0; i < result.Route.Count; i++)
                {
                    goalPorts.Enqueue(result.Route[i]);
                }
            }
            curGoalPortUID = startPort;
        }

        public override void Process(float dt)
        {
            if (goalPorts.Count > 0)
            {
                bool isCurPortReached = CheckIsCurrentPortReached();
                if (!isCurPortReached)
                {
                    if (goalSeq.Count > 0)
                    {
                        BaseGoal currentGoal = goalSeq.Peek();
                        if (!currentGoal.IsActived)
                        {
                            currentGoal.Activate();
                        }

                        currentGoal.Process(dt);

                        if (currentGoal.State == GoalState.Finished)
                        {
                            currentGoal.Terminate();
                        }

                        if ((currentGoal.State == GoalState.Finished) || (currentGoal.State == GoalState.Disabled))
                        {
                            goalSeq.Dequeue();
                        }
                    }
                }
                else
                {
                    //把当前已经移动到的Port信息记录下来
                    AIPort prevPort = pathManager.GetPortOfUID(curGoalPortUID);
                    prevTC = prevPort.Owner;
                    prevPortUID = curGoalPortUID;
                    goalPorts.Dequeue();

                    //清除当前的Goal列表
                    goalSeq.Clear();

                    if (goalPorts.Count > 0)
                    {
                        curGoalPortUID = goalPorts.Peek();
                        AIPort nextPort = pathManager.GetPortOfUID(curGoalPortUID);
                        AITrafficComponet nextTC = nextPort.Owner;

                        Queue<Vector3> goalPoints = new Queue<Vector3>();
                        //如果是在一个TC里面移动
                        if (nextTC == prevTC)
                        {
                            if (prevTC is AIRoad)
                            {
                                AIRoad aiRoad = (AIRoad)prevTC;
                                List<RoadNode> trackLine = aiRoad.RoadData.RoadMiddle;
                                int prevIndex = GetPortAtTrackLine(trackLine, bicycle.Position);
                                int nextIndex = GetPortAtTrackLine(trackLine, nextPort.Position);

                                float deltaDist = 0;
                                Vector3 lastPoint = Vector3.Zero;
                                for (int i = prevIndex; i <= nextIndex - 1; i++)
                                {
                                    if (lastPoint != Vector3.Zero)
                                    {
                                        deltaDist += Vector3.Distance(trackLine[i].Position, lastPoint);
                                    }
                                    if (deltaDist > 4.5f)
                                    {
                                        deltaDist = 0;
                                        goalPoints.Enqueue(trackLine[i].Position);
                                    }
                                    lastPoint = trackLine[i].Position;
                                }
                                goalPoints.Enqueue(trackLine[nextIndex].Position);
                            }
                            else if (prevTC is AIJunction)
                            {
                                goalPoints.Enqueue(nextPort.Position);
                            }
                        }
                        else
                        {
                            goalPoints.Enqueue(nextPort.Position);
                        }

                        //把该MoveToPoints添加到移动列表中
                        goalSeq.Enqueue(new BicylceToPoints(owner, manager, fatherGoal, goalPoints));
                    }
                }
            }
            else
            {
                state = GoalState.Finished;
                Terminate();
            }
        }

        private int GetPortAtTrackLine(List<RoadNode> trackLine, Vector3 pos)
        {
            float minimum = float.MaxValue;
            int bestindex = - 1;
            for (int i = 0; i < trackLine.Count; i++)
            {
                if (Vector3.Distance(trackLine[i].Position, pos) < minimum)
                {
                    minimum = Vector3.Distance(trackLine[i].Position, pos);
                    bestindex = i;
                }
            }
            return bestindex;
        }

        public override void Terminate()
        {
            fatherGoal.AddSubGoal(new BicycleBrake(owner, manager, fatherGoal));
        }
        #endregion
    }
}
