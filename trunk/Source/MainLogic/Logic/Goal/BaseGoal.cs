using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic.Goal
{
    public enum GoalState
    {
        InQueue,
        Processing,
        Finished,
        Disabled
    }

    /// <summary>
    /// 描述一个物体的目标
    /// </summary>
    public class BaseGoal
    {
        #region Fields
        /// <summary>
        /// 得到是否初始化
        /// </summary>
        protected bool isActived;
        public bool IsActived
        {
            get { return isActived; }
        }

        /// <summary>
        /// 得到或设置子目标
        /// </summary>
        protected Queue<BaseGoal> goalSeq;
        public Queue<BaseGoal> GoalSeq
        {
            get { return goalSeq; }
            set { goalSeq = value; }
        }

        /// <summary>
        /// 得到是否目标是不可细分的目标
        /// </summary>
        public bool IsAtomGoal
        {
            get { return isAtomGoal; }
        }
        protected bool isAtomGoal;


        /// <summary>
        /// 得到目标的状态
        /// </summary>
        protected GoalState state;
        public GoalState State
        {
            get { return state; }
            set { state = value; }
        }

        /// <summary>
        /// 得到目标的拥有者
        /// </summary>
        protected DynamicObject owner;
        public DynamicObject Owner
        {
            get { return owner; }
        }

        /// <summary>
        /// 得到目标管理器
        /// </summary>
        protected GoalManager manager;
        public GoalManager Manager
        {
            get { return manager; }
        }

        /// <summary>
        /// 得到或设置目标的父目标
        /// </summary>
        protected BaseGoal fatherGoal;
        public BaseGoal FatherGoal
        {
            get { return fatherGoal; }
            set { fatherGoal = value; }
        }
        #endregion

        #region Constructors
        public BaseGoal(DynamicObject obj, GoalManager mgr,BaseGoal fatherGoal)
        {
            owner = obj;
            manager = mgr;
            goalSeq = new Queue<BaseGoal>();
            this.fatherGoal = fatherGoal;
        }
        #endregion


        #region Methods
        /// <summary>
        /// 激活的时候调用
        /// </summary>
        public virtual void Activate()
        {

        }

        /// <summary>
        /// 执行目标的操作
        /// </summary>
        public virtual void Process(float dt)
        {

        }

        /// <summary>
        /// 结束的时候调用
        /// </summary>
        public virtual void Terminate()
        {

        }

        /// <summary>
        /// 添加一个子目标
        /// </summary>
        public virtual void AddSubGoal(BaseGoal goal)
        {
            goalSeq.Enqueue(goal);
        }
        #endregion
    }
}
