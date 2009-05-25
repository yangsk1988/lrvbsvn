using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using VirtualBicycle.Graphics;

namespace VirtualBicycle.Logic.Traffic
{
    /// <summary>
    /// 表示样条
    /// 用于TrackLine的输入
    /// 包含下面的三种信息
    /// </summary>
    public struct RoadNode
    {
        public Vector3 Position;

        public float Width;

        public float Twist;
    }

    /// <summary>
    /// 表示生成之后的样条
    /// 经过TrackLine类插值之后的输出顶点
    /// </summary>
    public struct OutputTrackVertex
    {
        public Vector3 Position;

        public float Twist;

        public float Width;

        public Vector3 Right;

        public Vector3 Dir;

        public Vector3 Up;

        //从道路开始点到当前点的距离值
        public float Length;
    }

    /// <summary>
    /// 赛道曲线输入结构体
    /// </summary>
    public class RoadlineInputData
    {
        public List<RoadNode> TracklineInputList;

        public RoadlineInputData(List<RoadNode> input)
        {
            TracklineInputList = input;
        }

        public RoadlineInputData()
        {
            TracklineInputList = new List<RoadNode>();
        }
    }

    /// <summary>
    /// 赛道曲线输出结构体
    /// </summary>
    public class OutputTracklineList
    {
        public List<OutputTrackVertex> TracklineOutputList;

        public OutputTracklineList()
        {
            TracklineOutputList = new List<OutputTrackVertex>();
        }

        public OutputTracklineList(int len)
        {
            TracklineOutputList = new List<OutputTrackVertex>(len);
        }

        public struct RoadSurface
        {
            private int width;
            public int Width
            {
                get { return width; }
            }

            private int length;
            public int Length
            {
                get { return length; }
            }

            public VertexPNT1[,] Surface;

            public RoadSurface(int width, int length)
            {
                Surface = new VertexPNT1[length, width];
                this.length = length;
                this.width = width;
            }
        }
    }
}