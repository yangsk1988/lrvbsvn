using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic.Goal
{
    public class BicylceToPoints : BaseGoal
    {
        #region Fields
        /// <summary>
        /// 得到或设置目标的点
        /// </summary>
        private Queue<Vector3> goalPoints;
        public Queue<Vector3> GoalPoints
        {
            get { return goalPoints; }
            set { goalPoints = value; }
        }

        private Bicycle bicylce;
        #endregion

        #region Constructor
        public BicylceToPoints(DynamicObject obj, GoalManager mgr, BaseGoal fatherGoal,Queue<Vector3> points) :
            base(obj, mgr,fatherGoal)
        {
            bicylce = (Bicycle)obj;
            goalPoints = points;
        }

        public BicylceToPoints(DynamicObject obj, GoalManager mgr, BaseGoal fatherGoal) :
            base(obj, mgr,fatherGoal)
        {
            bicylce = (Bicycle)obj;
        }
        #endregion

        #region Methods
        public override void Activate()
        {
            isActived = true;
            while (goalPoints.Count > 0)
            {
                Vector3 point = goalPoints.Dequeue();
                
                //将这个点添加到目标点序列中
                AddSubGoal(new BicycleToSinglePoint(owner, manager,point,this));
            }
        }

        public override void Process(float dt)
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
            else
            {
                state = GoalState.Finished;
                Terminate();
            }
        }

        public override void Terminate()
        {
            fatherGoal.AddSubGoal(new BicycleBrake(bicylce, manager, fatherGoal));
        }
        #endregion
    }
}
