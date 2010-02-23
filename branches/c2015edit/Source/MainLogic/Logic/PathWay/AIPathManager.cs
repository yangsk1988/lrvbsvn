using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Scene;
using VirtualBicycle.Logic.Traffic;
using SlimDX;

namespace VirtualBicycle.Logic.PathWay
{
    /// <summary>
    /// 管理道路的存储与寻路
    /// </summary>
    public class AIPathManager
    {
        #region Fields
        /// <summary>
        /// 得到交通组件的列表
        /// </summary>
        private List<AITrafficComponet> aiTrafficComponets;
        public List<AITrafficComponet> AITrafficComponets
        {
            get { return aiTrafficComponets; }
        }

        /// <summary>
        /// 得到AIPort的列表
        /// </summary>
        private List<AIPort> aiPorts;
        public List<AIPort> AIPorts
        {
            get { return aiPorts; }
        }

        private List<ITrafficComponment> usedSourceTrafficComp;


        /// <summary>
        /// 用于内部给每个Componet赋值的ID
        /// </summary>
        private int currentComponetID;

        /// <summary>
        /// 用于内部给每个Port赋值的ID
        /// </summary>
        private int currentPortID;

        private SceneObject[] sceneObjects;

        private AIPathFinder pathFinder;
        public AIPathFinder PathFinder
        {
            get { return pathFinder; }
        }
        #endregion

        #region Constructor
        public AIPathManager(SceneObject[] objects)
        {
            this.sceneObjects = objects;

            InitializeTrafficNet();

            pathFinder = new AIPathFinder(this);
        }
        #endregion

        #region Methods
        /// <summary>
        /// 初始化交通网
        /// </summary>
        private bool InitializeTrafficNet()
        {
            usedSourceTrafficComp = new List<ITrafficComponment>();
            Queue<ITrafficComponment> queueSourceTC = new Queue<ITrafficComponment>();
            aiPorts = new List<AIPort>();
            aiTrafficComponets = new List<AITrafficComponet>();
            Road track = null;
            //第一步,查找一条道路出来
            for (int i = 0; i < sceneObjects.Length; i++)
            {
                if (sceneObjects[i].TypeTag == "Path")
                {
                    track = (Road)sceneObjects[i];
                    break;
                }
            }

            if (track == null)
            {
                return false;
            }

            //第二步,根据这条道路的连接信息
            //  把所有的ITrafficComp都变换到AITrafficComponet去
            //首先把这条道路的路径添加到SourceTC队列中
            queueSourceTC.Enqueue(track);
            usedSourceTrafficComp.Add(track);
            while (queueSourceTC.Count > 0)
            {
                //每次执行下面的操作:
                //1.枚举端口
                //2.注册物体
                ITrafficComponment sourceComponet = queueSourceTC.Dequeue();
                if (sourceComponet != null)
                {
                    //枚举端口
                    TCPort[] ports = sourceComponet.GetPorts();
                    List<TCConnectionInfo> connections = sourceComponet.GetConnections();

                    //注册物体
                    AITrafficComponet aiTC = RegisterComponet(sourceComponet);

                    //枚举端口对外情况
                    for (int i = 0; i < ports.Length; i++)
                    {
                        TCPort sourcePort = ports[i];
                        AIPort aiPort = new AIPort(aiTC, sourcePort.Position, sourcePort.Direction, sourcePort.Width, sourcePort.Twist);
                        RegisterPort(aiPort);
                        aiTC.Ports.Add(aiPort);
                    }

                    //添加对外的边和端口节点
                    for (int i = 0; i < connections.Count; i++)
                    {
                        TCConnectionInfo connectionInfo = connections[i];
                        ITrafficComponment targetTC = connectionInfo.TargetComponet;
                        if (!IsUsedSourceTrafficComp(targetTC))
                        {
                            queueSourceTC.Enqueue(targetTC);
                            usedSourceTrafficComp.Add(targetTC);
                        }
                    }
                }
            }

            //第三步,根据这些已生成的AITrafficComponet,添加他们的Edge信息
            for (int i = 0; i < aiTrafficComponets.Count; i++)
            {
                //首先添加对外的端口连接信息
                AITrafficComponet aiSourceTC = aiTrafficComponets[i];
                ITrafficComponment sourceTC = aiSourceTC.SourceTC;
                List<TCConnectionInfo> tcConnections = sourceTC.GetConnections();
                TCPort[] sourcePorts = sourceTC.GetPorts();

                for (int j = 0; j < tcConnections.Count; j++)
                {
                    TCConnectionInfo tcConnectionInfo = tcConnections[j];
                    int sourcePortIndex = tcConnectionInfo.SelfPortIndex;
                    int targetPortIndex = tcConnectionInfo.TargetPortIndex;

                    ITrafficComponment targetTC = tcConnectionInfo.TargetComponet;
                    TCPort[] targetPorts = targetTC.GetPorts();
                    AITrafficComponet aiTargetTC = FindAITrafficComponet(targetTC);

                    AIPort aiSourcePort = FindAIPort(aiSourceTC, sourcePorts[tcConnectionInfo.SelfPortIndex].Position);
                    AIPort aiTargetPort = FindAIPort(aiTargetTC, targetPorts[tcConnectionInfo.TargetPortIndex].Position);
                    float cost = Vector3.Distance(aiSourcePort.Position,aiTargetPort.Position);
                    aiSourcePort.AddEdge(new AIEdge(aiSourcePort,aiTargetPort,cost));
                }

                //然后添加对内的端口连接信息
                List<AIPort> aiSourcePorts = aiSourceTC.Ports;
                for (int portIndexA = 0; portIndexA < aiSourcePorts.Count; portIndexA++)
                {
                    for (int portIndexB = 0; portIndexB < aiSourcePorts.Count; portIndexB++)
                    {
                        if (portIndexA != portIndexB)
                        {
                            AIPort aiPortA = aiSourcePorts[portIndexA];
                            AIPort aiPortB = aiSourcePorts[portIndexB];
                            float cost = Vector3.Distance(aiPortA.Position, aiPortB.Position);
                            aiPortA.AddEdge(new AIEdge(aiPortA, aiPortB, cost));
                        }

                    }
                }
            }

            return true;
        }

        private AITrafficComponet FindAITrafficComponet(ITrafficComponment comp)
        {
            for (int i = 0; i < aiTrafficComponets.Count; i++)
            {
                if (aiTrafficComponets[i].SourceTC == comp)
                {
                    return aiTrafficComponets[i];
                }
            }

            return null;
        }

        private AIPort FindAIPort(AITrafficComponet aiTC, Vector3 pos)
        {
            List<AIPort> ports = aiTC.Ports;
            for (int i = 0; i < ports.Count; i++)
            {
                if (ports[i].Position == pos)
                {
                    return ports[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 查找是否已经在元数据中建立过该组件的交通连接
        /// </summary>
        /// <param name="tc"></param>
        /// <returns></returns>
        private bool IsUsedSourceTrafficComp(ITrafficComponment tc)
        {
            for (int i = 0; i < usedSourceTrafficComp.Count; i++)
            {
                if (usedSourceTrafficComp[i] == tc)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 设置两个道路之间的连接关系
        /// 如果一条边分别对应两个Componet,则设置其状态
        /// </summary>
        /// <param name="compA"></param>
        /// <param name="compB"></param>
        public void SetTwoCompConnection(AITrafficComponet compA, AITrafficComponet compB, EnumEdgeState state)
        {
            List<AIPort> ports;
            //查看从A到B的边
            ports = compA.Ports;
            for (int i = 0; i < ports.Count; i++)
            {
                List<AIEdge> edges = ports[i].Edges;
                for (int j = 0; j < edges.Count; j++)
                {
                    if (edges[j].TargetPort.Owner == compB)
                    {
                        edges[j].EdgeState = state;
                    }
                }
            }

            //查看从B到A的边
            ports = compB.Ports;
            for (int i = 0; i < ports.Count; i++)
            {
                List<AIEdge> edges = ports[i].Edges;
                for (int j = 0; j < edges.Count; j++)
                {
                    if (edges[j].TargetPort.Owner == compA)
                    {
                        edges[j].EdgeState = state;
                    }
                }
            }
        }

        /// <summary>
        /// 将这个Componet的内部的Port(指向属于自己Port)的连接全部设置为State
        /// </summary>
        /// <param name="road"></param>
        /// <param name="state"></param>
        public void SetSingleCompConnect(AITrafficComponet compA, EnumEdgeState state)
        {
            List<AIPort> ports = compA.Ports;
            for (int i = 0; i < ports.Count; i++)
            {
                List<AIEdge> edges = ports[i].Edges;
                for (int j = 0; j < edges.Count; j++)
                {
                    if (edges[j].TargetPort.Owner == compA)
                    {
                        edges[j].EdgeState = state;
                    }
                }
            }
        }

        public AITrafficComponet RegisterComponet(ITrafficComponment componet)
        {
            AITrafficComponet aiTC = null;
            if (componet is Road)
            {
                Road track = (Road)componet;
                aiTC = new AIRoad(componet);
                //add road data
            }
            else if (componet is Junction)
            {
                Junction junction = (Junction)componet;
                aiTC = new AIJunction(componet);
                //add junction data
            }

            if (aiTC != null)
            {
                aiTC.UID = currentComponetID;
                currentComponetID++;
                aiTrafficComponets.Add(aiTC);
            }
            return aiTC;
        }

        public AIPort GetPortOfUID(int uid)
        {
            if (uid < AIPorts.Count)
            {
                if (aiPorts[uid].UID == uid)
                {
                    return aiPorts[uid];
                }
                else
                {
                    throw new Exception("uid与AiPort在List中的索引不一样");
                }
            }
            return null;
        }

        public void RegisterPort(AIPort port)
        {
            port.UID = currentPortID;
            aiPorts.Add(port);
            currentPortID++;
        }
        #endregion
    }
}
