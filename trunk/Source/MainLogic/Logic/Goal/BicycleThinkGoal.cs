using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic.Goal
{
    /// <summary>
    /// 这个类是Bicycle类的最高层目标类,用来管理下面的目标
    /// </summary>
    public class BicycleThinkGoal : BaseGoal
    {
        #region Fields
        Bicycle bicycle;
        #endregion

        #region Constructors
        public BicycleThinkGoal(DynamicObject obj, GoalManager mgr, BaseGoal fatherGoal)
            : base(obj, mgr,fatherGoal)
        {
            bicycle = (Bicycle)obj;
        }
        #endregion

        #region Methods
        public override void Activate()
        {
            if (!isActived)
            {
                goalSeq = new Queue<BaseGoal>();
                state = GoalState.Processing;
                isActived = true;
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
        }

        public override void Terminate()
        {
            
        }
        #endregion
    }
}
