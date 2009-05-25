using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic.Goal
{
    public class GoalManager
    {
        #region Fields
        /// <summary>
        /// 得到或获取目标序列
        /// </summary>
        private Queue<BaseGoal> goalSeq;
        public Queue<BaseGoal> GoalSeq
        {
            get { return goalSeq; }
            set { goalSeq = value; }
        }

        private DynamicObject owner;
        #endregion
        
        #region Constructor
        public GoalManager(DynamicObject obj)
        {
            goalSeq = new Queue<BaseGoal>();
            owner = obj;
        }
        #endregion

        #region Methods
        public void Update(float dt)
        {
            if (goalSeq.Count > 0)
            {
                BaseGoal currentGoal = goalSeq.Peek();

                //如果没有初始化,则初始化
                if (currentGoal.IsActived)
                {
                    currentGoal.Activate();
                }

                //进行更新
                currentGoal.Process(dt);

                //如果目标已完成或者失效,则Dequeue
                if ((currentGoal.State == GoalState.Finished) || (currentGoal.State == GoalState.Disabled))
                {
                    currentGoal.Terminate();
                }
            }
        }

        /// <summary>
        /// 得到队列中的第一个原子目标
        /// 供状态机类调用
        /// </summary>
        /// <returns></returns>
        public BaseGoal GetFirstAtomGoal()
        {
            BaseGoal goal;
            if (this.goalSeq.Count > 0)
            {
                goal = this.goalSeq.Peek();
                while (goal.IsAtomGoal)
                {
                    goal = goal.GoalSeq.Peek();
                }

                if (goal.IsAtomGoal)
                {
                    return goal;
                }
            }
            return null;
        }
        #endregion
    }
}
