using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;
using VirtualBicycle.MathLib;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic.Traffic
{
    public enum TrackTexCoordMode
    {
        Symmetry,
        DisSymmetery
    }

    public enum TrackVertexType
    {
        //上表面左边
        UpLeftSide,

        //上表面左中
        UpLeftMiddle,

        //上表面正中
        UpMiddle,

        //上表面右中
        UpRightMiddle,

        //上表面右边
        UpRightSide
    }

    public unsafe struct HeightMaps
    {
        public float* LeftUp;
        public float* RightUp;
        public float* LeftDown;
        public float* RightDown;

        public float this[int ux, int uy]
        {
            get
            {
                float* relativeMap = LeftUp;
                int rx = ux;
                int ry = uy;
                //根据相对值,得出到底是在哪个Cluster中
                if ((ux < Cluster.ClusterSize) && (uy < Cluster.ClusterSize))
                {
                    relativeMap = LeftUp;
                }
                else if ((ux < Cluster.ClusterSize) && (uy >= Cluster.ClusterSize))
                {
                    relativeMap = LeftDown;
                }
                else if ((ux >= Cluster.ClusterSize) && (uy < Cluster.ClusterSize))
                {
                    relativeMap = RightUp;
                }
                else if ((ux >= Cluster.ClusterSize) && (uy >= Cluster.ClusterSize))
                {
                    relativeMap = RightDown;
                }

                //根据相对值,计算得出在所属的Cluster中的相对坐标
                if (uy >= Cluster.ClusterSize)
                {
                    ry = uy - Cluster.ClusterSize;
                }

                if (ux >= Cluster.ClusterSize)
                {
                    rx = ux - Cluster.ClusterSize;
                }

                return relativeMap[ry * Cluster.ClusterSize + rx];
            }
            set
            {
                float* relativeMap;
                int rx = ux;
                int ry = uy;
                //根据相对值,得出到底是在哪个Cluster中
                if ((ux < Cluster.ClusterSize) && (uy < Cluster.ClusterSize))
                {
                    relativeMap = LeftUp;

                    if (uy >= Cluster.ClusterSize)
                    {
                        ry = uy - Cluster.ClusterSize;
                    }

                    if (ux >= Cluster.ClusterSize)
                    {
                        rx = ux - Cluster.ClusterSize;
                    }

                    relativeMap[ry * Cluster.ClusterSize + rx] = value;
                }

                if ((ux < Cluster.ClusterSize) && (uy >= Cluster.ClusterSize - 1))
                {
                    relativeMap = LeftDown;

                    if (uy >= Cluster.ClusterSize)
                    {
                        ry = uy - Cluster.ClusterSize;
                    }

                    if (ux >= Cluster.ClusterSize)
                    {
                        rx = ux - Cluster.ClusterSize;
                    }

                    relativeMap[ry * Cluster.ClusterSize + rx] = value;
                }

                if ((ux >= Cluster.ClusterSize - 1) && (uy < Cluster.ClusterSize))
                {
                    relativeMap = RightUp;

                    if (uy >= Cluster.ClusterSize)
                    {
                        ry = uy - Cluster.ClusterSize;
                    }

                    if (ux >= Cluster.ClusterSize)
                    {
                        rx = ux - Cluster.ClusterSize;
                    }

                    relativeMap[ry * Cluster.ClusterSize + rx] = value;
                }

                if ((ux >= Cluster.ClusterSize - 1) && (uy >= Cluster.ClusterSize - 1))
                {
                    relativeMap = RightDown;

                    if (uy >= Cluster.ClusterSize)
                    {
                        ry = uy - Cluster.ClusterSize;
                    }

                    if (ux >= Cluster.ClusterSize)
                    {
                        rx = ux - Cluster.ClusterSize;
                    }

                    relativeMap[ry * Cluster.ClusterSize + rx] = value;
                }
            }
        }
    }

    public class RoadBuilder
    {
        #region Fields

        /// <summary>
        /// 单元路径的顶点上限数
        /// </summary>
        const int MaxPointsCount = 10;

        /// <summary>
        /// 每个Width对应的左右点对之间的距离
        /// 比如说如果UnitWidth = 5,
        ///     那么道路中间的点与道路左中和右中的点的距离就是5
        /// </summary>
        const float UnitWidth = 1f;

        /// <summary>
        /// 道路的厚度
        /// </summary>
        const float UnitRoadHeight = 0.5f;

        /// <summary>
        /// 道路平滑的平均次数
        /// </summary>
        const int SmoothSampleCount = 5;

        /// <summary>
        /// 道路点最少高于附近地表的值
        /// </summary>
        const float MaxHeightOverTerrain = 0.1f;
        /// <summary>
        /// 道路的贴图坐标方式:对称/非对称
        /// </summary>
        public static TrackTexCoordMode trackTexCoordMode = TrackTexCoordMode.Symmetry;

        /// <summary>
        /// 贴图坐标x方向的代表的真实长度值
        /// </summary>
        public float trackTexPeriodLength = 6f;

        /// <summary>
        /// 贴图中y方向的一条道路的宽度对应的贴图坐标的区间[0,x]
        /// </summary>
        public float trackTexPeriodWidth = 24f;

        /// <summary>
        /// 每个单元格的长度
        /// </summary>
        public float CellUnit
        {
            get;
            set;
        }

        /// <summary>
        /// 高度的系数
        /// </summary>
        public float HeightScale
        {
            get;
            set;
        }

        /// <summary>
        /// Cluster左上角的偏移量
        /// </summary>
        public Vector3 ClusterOffset
        {
            get;
            set;
        }

        /// <summary>
        /// 高度图块,1块或者是4块,平整地形时候用
        /// </summary>
        public HeightMaps ClusterBlocks
        {
            get;
            set;
        }

        #endregion

        #region 属性

        public TCPort? HeadConnector
        {
            get;
            set;
        }

        public TCPort? TailConnector
        {
            get;
            set;
        }

        #endregion

        public RoadBuilder()
        {

        }

        #region Methods
        /// <summary>
        /// 根据输入的顶点,输出一个Model的集合
        /// 包含多条道路
        /// inputPoints的长度必需大于2
        /// </summary>
        public Model[] BuildModels(Device device, RoadLine trackLine)
        {
            trackLine.Interpose(this.HeadConnector, this.TailConnector);

            #region Fields

            //插值后,经过处理,加上Right,Up等内容的样条序列列表,再分割而成的定点序列列表
            List<OutputTracklineList> tracklineOutputDataList;

            //返回的模型数组
            Model[] result;
            #endregion

            #region 得到输入点插值后的输入点序列
            //根据输入点,得到插值后的样条
            RoadlineInputData tracklineInterposedData = new RoadlineInputData(trackLine.InterposedPoints);
            #endregion

            #region 得到插值后,经过处理,加上Right,Up等内容的样条序列列表
            OutputTracklineList output = GetOutputTrackline(tracklineInterposedData);
            tracklineInterposedData = null;
            #endregion

            #region 根据插值后的点,根据长度拆分,输出一个样条列表集合
            tracklineOutputDataList = GetOutputTracklineSplited(output);
            #endregion

            #region 根据拆分后,处理后的OutputTrackVertex序列,生成模型序列
            result = new Model[tracklineOutputDataList.Count];
            for (int i = 0; i < tracklineOutputDataList.Count; i++)
            {
                result[i] = BuildModel(device, tracklineOutputDataList[i]);
            }

            return result;
            #endregion
        }


        /// <summary>
        /// 根据输入的样条,得到拆分后的样条序列
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        List<OutputTracklineList> GetOutputTracklineSplited(OutputTracklineList input)
        {
            //用于存储分段后的点
            int roadSegCount = input.TracklineOutputList.Count / MaxPointsCount + 1;
            List<OutputTracklineList> output = new List<OutputTracklineList>(roadSegCount);
            OutputTracklineList oneSect = new OutputTracklineList(MaxPointsCount);
            //当前存储的一段点的数量
            int totalVertexCount = 0;

            for (int i = 0; i < input.TracklineOutputList.Count; i++)
            {
                totalVertexCount++;
                oneSect.TracklineOutputList.Add(input.TracklineOutputList[i]);

                if (totalVertexCount >= MaxPointsCount)
                {
                    output.Add(oneSect);
                    oneSect = new OutputTracklineList(MaxPointsCount);
                    totalVertexCount = 1;
                    oneSect.TracklineOutputList.Add(input.TracklineOutputList[i]);
                }
            }

            if (oneSect.TracklineOutputList.Count > 0)
            {
                output.Add(oneSect);
            }

            return output;
        }

        /// <summary>
        /// 根据输入的顶点序列,得到一个输出顶点序列
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        OutputTracklineList GetOutputTrackline(RoadlineInputData input)
        {
            OutputTracklineList output = new OutputTracklineList(input.TracklineInputList.Count);

            float roadLength = 0f; //公路从起点到当前点的距离值

            //一段输出序列
            List<OutputTrackVertex> outputOneSect = new List<OutputTrackVertex>(input.TracklineInputList.Count);

            //得到当前处理的顶点序列
            List<RoadNode> inputOneSect = input.TracklineInputList;

            #region 根据新的样条序列生成Up,Right,Dir,UV
            //遍历输入的每个顶点
            for (int i = 0; i < inputOneSect.Count; i++)
            {
                OutputTrackVertex outputTrackVertex;
                outputTrackVertex.Position = inputOneSect[i].Position;

                //outputTrackVertex.Position.Y += 0.1f;
                outputTrackVertex.Position += Vector3.UnitY * UnitRoadHeight;

                outputTrackVertex.Width = inputOneSect[i].Width;
                outputTrackVertex.Twist = inputOneSect[i].Twist;

                //生成道路的长度值
                if (i > 0)
                {
                    roadLength += Vector3.Distance(inputOneSect[i].Position, inputOneSect[i - 1].Position);
                }
                outputTrackVertex.Length = roadLength;
                if (i < inputOneSect.Count - 1)
                {

                    outputTrackVertex.Dir = inputOneSect[i + 1].Position - inputOneSect[i].Position;
                    outputTrackVertex.Dir.Normalize();

                    Quaternion q = Quaternion.RotationAxis(outputTrackVertex.Dir, outputTrackVertex.Twist);

                    Vector3 rotY = MathEx.QuaternionRotate(q, Vector3.UnitY);

                    outputTrackVertex.Right = Vector3.Cross(outputTrackVertex.Dir, rotY);
                    outputTrackVertex.Right.Normalize();

                    outputTrackVertex.Up = Vector3.Cross(outputTrackVertex.Right, outputTrackVertex.Dir);
                    outputTrackVertex.Up.Normalize();

                    //to do:  UV还没写
                    outputOneSect.Add(outputTrackVertex);
                }
                else
                {
                    outputTrackVertex.Dir = outputOneSect[i - 1].Dir;
                    outputTrackVertex.Right = outputOneSect[i - 1].Right;
                    outputTrackVertex.Up = outputOneSect[i - 1].Up;
                    outputOneSect.Add(outputTrackVertex);
                }
            }

            //对生成样条的Right,Up,和Dir进行平滑处理
            for (int i = 0; i < outputOneSect.Count; i++)
            {
                OutputTrackVertex newValue = outputOneSect[i];
                for (int j = -SmoothSampleCount; j <= SmoothSampleCount; j++)
                {
                    if ((i + j >= 0) && (i + j < outputOneSect.Count))
                    {
                        newValue.Up += outputOneSect[i + j].Up;
                        newValue.Right += outputOneSect[i + j].Right;
                        newValue.Dir += outputOneSect[i + j].Dir;
                    }
                }

                newValue.Up.Normalize();
                newValue.Right.Normalize();
                newValue.Dir.Normalize();
                outputOneSect[i] = newValue;
            }
            #endregion

            output.TracklineOutputList = outputOneSect;
            return output;
        }

        /// <summary>
        /// 根据输入的顶点类型,以及长度,宽度等数据,得到该顶点的贴图坐标
        /// </summary>
        /// <param name="vertexType">顶点类型</param>
        /// <param name="length">从起点到当前点的长度</param>
        /// <param name="width">当前道路的宽度</param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        void GetRoadTexCoord(TrackVertexType vertexType, float length, float width, out float u, out float v)
        {
            u = (float)length / trackTexPeriodLength;
            v = 0;
            //首先考虑对称的情况
            if (trackTexCoordMode == TrackTexCoordMode.Symmetry)
            {
                switch (vertexType)
                {
                    case TrackVertexType.UpMiddle:
                        v = 0;
                        break;
                    case TrackVertexType.UpLeftSide:
                    case TrackVertexType.UpRightSide:
                        v = 4 * width / trackTexPeriodWidth;
                        break;
                    case TrackVertexType.UpLeftMiddle:
                    case TrackVertexType.UpRightMiddle:
                        v = 2 * width / trackTexPeriodWidth;
                        break;
                }
            }
            else
            {
                switch (vertexType)
                {
                    case TrackVertexType.UpMiddle:
                        v = 0.5f;
                        break;
                    case TrackVertexType.UpLeftSide:
                        v = 0f;
                        break;
                    case TrackVertexType.UpLeftMiddle:
                        v = 0.25f;
                        break;
                    case TrackVertexType.UpRightMiddle:
                        v = 0.75f;
                        break;
                    case TrackVertexType.UpRightSide:
                        v = 1f;
                        break;
                }
            }
        }

        /// <summary>
        /// 根据输入的样条(经过处理后的),生成一个模型
        /// </summary>
        /// <param name="device"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Model BuildModel(Device device, OutputTracklineList input)
        {
            //顶点缓冲源数据
            VertexPNT1[] vbContent;

            //索引缓冲
            int[] ibContent;

            //输入样条的顶点数量
            int InputTrackVertexCount = input.TracklineOutputList.Count;

            //得到该样条生成模型的顶点缓冲和索引缓冲
            vbContent = BuildVertexBufferContent(input.TracklineOutputList);
            ibContent = BuildIndexBufferContent(InputTrackVertexCount, vbContent);

            //根据顶点缓冲和索引缓冲生成模型
            MeshMaterial[][] mats = new MeshMaterial[1][];
            mats[0] = new MeshMaterial[1];
            mats[0][0] = new MeshMaterial(device);

            mats[0][0].mat = MeshMaterial.DefaultMatColor;
            mats[0][0].CullMode = Cull.Counterclockwise;

            GameMesh mesh = new GameMesh(device, vbContent, ibContent, mats);

            Model model = new Model(device, new GameMesh[] { mesh });

            return model;
        }

        /// <summary>
        /// 根据输入的样条序列生成样条的VertexBuffer
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        VertexPNT1[] BuildVertexBufferContent(List<OutputTrackVertex> input)
        {
            int vbContentLength = input.Count * 11 + 8;
            int curIndex = 0;
            //顶点缓冲源数据
            VertexPNT1[] vbContent = new VertexPNT1[vbContentLength];


            VertexPNT1 frontLeftUp = new VertexPNT1();
            VertexPNT1 frontLeftDown = new VertexPNT1();
            VertexPNT1 frontRightUp = new VertexPNT1();
            VertexPNT1 frontRightDown = new VertexPNT1();
            VertexPNT1 backLeftUp = new VertexPNT1();
            VertexPNT1 backLeftDown = new VertexPNT1();
            VertexPNT1 backRightUp = new VertexPNT1();
            VertexPNT1 backRightDown = new VertexPNT1();

            List<OutputTrackVertex> outputOneSect = input;

            #region 根据样条数据生成顶点缓冲
            for (int j = 0; j < outputOneSect.Count; j++)
            {
                OutputTrackVertex outputTrackVertex = outputOneSect[j];

                VertexPNT1 downLeft;
                VertexPNT1 leftDown;
                VertexPNT1 leftUp;
                VertexPNT1 upLeftSide;
                VertexPNT1 upLeftMiddle;
                VertexPNT1 upMiddle;
                VertexPNT1 upRightMiddle;
                VertexPNT1 upRightSide;
                VertexPNT1 rightUp;
                VertexPNT1 rightDown;
                VertexPNT1 downRight;

                float width = outputTrackVertex.Width / 4;

                upMiddle.pos = outputTrackVertex.Position;
                upMiddle.n = outputTrackVertex.Up;
                GetRoadTexCoord(TrackVertexType.UpMiddle, outputTrackVertex.Length, width,
                    out upMiddle.u, out upMiddle.v);

                upLeftMiddle.pos = upMiddle.pos - outputTrackVertex.Right * UnitWidth * width;
                upLeftMiddle.n = outputTrackVertex.Up;
                GetRoadTexCoord(TrackVertexType.UpLeftMiddle, outputTrackVertex.Length, width,
                   out upLeftMiddle.u, out upLeftMiddle.v);

                upLeftSide.pos = upLeftMiddle.pos - outputTrackVertex.Right * UnitWidth * width;
                upLeftSide.n = outputTrackVertex.Up;
                GetRoadTexCoord(TrackVertexType.UpLeftSide, outputTrackVertex.Length, width,
                    out upLeftSide.u, out upLeftSide.v);

                upRightMiddle.pos = upMiddle.pos + outputTrackVertex.Right * UnitWidth * width;
                upRightMiddle.n = outputTrackVertex.Up;
                GetRoadTexCoord(TrackVertexType.UpRightMiddle, outputTrackVertex.Length, width,
                    out upRightMiddle.u, out upRightMiddle.v);

                upRightSide.pos = upRightMiddle.pos + outputTrackVertex.Right * UnitWidth * width;
                upRightSide.n = outputTrackVertex.Up;
                GetRoadTexCoord(TrackVertexType.UpRightSide, outputTrackVertex.Length, width,
                    out upRightSide.u, out upRightSide.v);

                leftUp.pos = upLeftSide.pos;
                leftUp.n = -outputTrackVertex.Right;
                leftUp.u = 0;
                leftUp.v = 0;

                leftDown.pos = leftUp.pos - UnitRoadHeight * outputTrackVertex.Up;
                leftDown.n = -outputTrackVertex.Right;
                leftDown.u = 0;
                leftDown.v = 0;

                downLeft.pos = leftDown.pos;
                downLeft.n = -outputTrackVertex.Up;
                downLeft.u = 0;
                downLeft.v = 0;

                rightUp.pos = upRightSide.pos;
                rightUp.n = outputTrackVertex.Right;
                rightUp.u = 0;
                rightUp.v = 0;

                rightDown.pos = rightUp.pos - UnitRoadHeight * outputTrackVertex.Up;
                rightDown.n = outputTrackVertex.Right;
                rightDown.u = 0;
                rightDown.v = 0;

                downRight.pos = rightDown.pos;
                downRight.n = -outputTrackVertex.Up; ;
                downRight.u = 0;
                downRight.v = 0;

                vbContent[curIndex] = downLeft;
                curIndex++;
                vbContent[curIndex] = leftDown;
                curIndex++;
                vbContent[curIndex] = leftUp;
                curIndex++;
                vbContent[curIndex] = upLeftSide;
                curIndex++;
                vbContent[curIndex] = upLeftMiddle;
                curIndex++;
                vbContent[curIndex] = upMiddle;
                curIndex++;
                vbContent[curIndex] = upRightMiddle;
                curIndex++;
                vbContent[curIndex] = upRightSide;
                curIndex++;
                vbContent[curIndex] = rightUp;
                curIndex++;
                vbContent[curIndex] = rightDown;
                curIndex++;
                vbContent[curIndex] = downRight;
                curIndex++;

                if (j == 0)
                {
                    frontLeftUp.pos = upLeftSide.pos;
                    frontLeftUp.n = -outputTrackVertex.Dir;
                    frontLeftUp.u = 0;
                    frontLeftUp.v = 0;

                    frontLeftDown.pos = leftDown.pos;
                    frontLeftDown.n = -outputTrackVertex.Dir;
                    frontLeftDown.u = 0;
                    frontLeftDown.v = 0;

                    frontRightUp.pos = upRightSide.pos;
                    frontRightUp.n = -outputTrackVertex.Dir;
                    frontRightUp.u = 0;
                    frontRightUp.v = 0;

                    frontRightDown.pos = rightDown.pos;
                    frontRightDown.n = -outputTrackVertex.Dir;
                    frontRightDown.u = 0;
                    frontRightDown.v = 0;
                }

                if (j == outputOneSect.Count - 1)
                {
                    backLeftUp.pos = upLeftSide.pos;
                    backLeftUp.n = -outputTrackVertex.Dir;
                    backLeftUp.u = 0;
                    backLeftUp.v = 0;

                    backLeftDown.pos = leftDown.pos;
                    backLeftDown.n = -outputTrackVertex.Dir;
                    backLeftDown.u = 0;
                    backLeftDown.v = 0;

                    backRightUp.pos = upRightSide.pos;
                    backRightUp.n = -outputTrackVertex.Dir;
                    backRightUp.u = 0;
                    backRightUp.v = 0;

                    backRightDown.pos = rightDown.pos;
                    backRightDown.n = -outputTrackVertex.Dir;
                    backRightDown.u = 0;
                    backRightDown.v = 0;
                }
            }


            vbContent[curIndex] = frontLeftDown;
            curIndex++;
            vbContent[curIndex] = frontLeftUp;
            curIndex++;
            vbContent[curIndex] = frontRightUp;
            curIndex++;
            vbContent[curIndex] = frontRightDown;
            curIndex++;
            vbContent[curIndex] = backLeftDown;
            curIndex++;
            vbContent[curIndex] =  backLeftUp;
            curIndex++;
            vbContent[curIndex] = backRightUp;
            curIndex++;
            vbContent[curIndex] = backRightDown;
            curIndex++;
            #endregion

            return vbContent;
        }

        /// <summary>
        /// 根据输入的样条数目上次样条的Indexbuffer
        /// </summary>
        /// <param name="inputTrackVertexCount"></param>
        /// <returns></returns>
        int[] BuildIndexBufferContent(int inputTrackVertexCount, VertexPNT1[] vbContent)
        {
            int ibContentLength = (inputTrackVertexCount - 1) * 42 + 12;
            int[] ibContent = new int[ibContentLength];
            int curIndex = 0;
            #region 根据点的排布情况生成索引缓冲
            //关于是使用顺时针或者逆时针还没有弄清楚
            for (int jj = 0; jj < inputTrackVertexCount - 1; jj++)
            {
                int j = jj * 11;
                //生成表面的三角形索引
                for (int k = j; k <= j + 3; k++)
                {
                    ibContent[curIndex] = k + 14;
                    curIndex++;
                    ibContent[curIndex] = k + 4;
                    curIndex++;
                    ibContent[curIndex] = k + 3;
                    curIndex++;


                    SmoothTerrain(vbContent[k + 14], vbContent[k + 4], vbContent[k + 3]);
                    ibContent[curIndex] = k + 4;
                    curIndex++;
                    ibContent[curIndex] = k + 14;
                    curIndex++;
                    ibContent[curIndex] = k + 15;
                    curIndex++;

                    SmoothTerrain(vbContent[k + 4], vbContent[k + 14], vbContent[k + 15]);
                }
                //生成左侧面索引

                ibContent[curIndex] = j + 12;
                curIndex++;
                ibContent[curIndex] = j + 2;
                curIndex++;
                ibContent[curIndex] = j + 1;
                curIndex++;


                ibContent[curIndex] = j + 13;
                curIndex++;
                ibContent[curIndex] = j + 2;
                curIndex++;
                ibContent[curIndex] = j + 12;
                curIndex++;

                //生成右侧面索引

                ibContent[curIndex] = j + 8;
                curIndex++;
                ibContent[curIndex] = j + 19;
                curIndex++;
                ibContent[curIndex] = j + 20;
                curIndex++;


                ibContent[curIndex] = j + 8;
                curIndex++;
                ibContent[curIndex] = j + 20;
                curIndex++;
                ibContent[curIndex] = j + 9;
                curIndex++;

                //生成下表面索引
                ibContent[curIndex] = j + 0;
                curIndex++;
                ibContent[curIndex] = j + 10;
                curIndex++;
                ibContent[curIndex] = j + 21;
                curIndex++;

                ibContent[curIndex] = j + 0;
                curIndex++;
                ibContent[curIndex] = j + 21;
                curIndex++;
                ibContent[curIndex] = j + 11;
                curIndex++;


            }

            int s = inputTrackVertexCount * 11;

            //如果是第一个面,那么加上"前盖子"
            ibContent[curIndex] = s + 1;
            curIndex++;
            ibContent[curIndex] = s + 3;
            curIndex++;
            ibContent[curIndex] = s;
            curIndex++;

            ibContent[curIndex] = s + 1;
            curIndex++;
            ibContent[curIndex] = s + 2;
            curIndex++;
            ibContent[curIndex] = s + 3;
            curIndex++;

            //为道路结束的地方加上"后盖子"
            ibContent[curIndex] = s + 5;
            curIndex++;
            ibContent[curIndex] = s + 4;
            curIndex++;
            ibContent[curIndex] = s + 6;
            curIndex++;

            ibContent[curIndex] = s + 4;
            curIndex++;
            ibContent[curIndex] = s + 7;
            curIndex++;
            ibContent[curIndex] = s + 6;
            curIndex++;

            #endregion

            return ibContent;
        }

        public unsafe void SmoothTerrain(VertexPNT1 pointa, VertexPNT1 pointb, VertexPNT1 pointc)
        {
            SmoothTerrain(pointa.pos, pointb.pos, pointc.pos);
        }

        /// <summary>
        /// 根据生成的顶点缓冲平滑高度图
        /// </summary>
        /// <param name="heightMaps">高度图指针,最多为4张子地图</param>
        /// <param name="cellUnit">单元格宽度系数</param>
        /// <param name="heightScale">高度系数</param>
        /// <param name="offset">偏移量</param>
        /// <param name="vbContent">需要检查的点</param>
        public unsafe void SmoothTerrain(Vector3 pointa, Vector3 pointb, Vector3 pointc)
        {
            ////平滑的时候,搜索的坐标距离
            //const int dOffset = 5;
            ////平滑的时候,每单位曼哈顿距离的影响因子
            //const float effectFactor = 0.05f;

            //const float zero = 1e-6f;
            int clusterCount;
            //Dictionary<Point, float> heightMapModified = new Dictionary<Point, float>(); 
            //const float widthEffectFactor = 1.2f;

            HeightMaps heightMaps = ClusterBlocks;
            float cellUnit = CellUnit;
            float heightScale = HeightScale;
            Vector3 offset = ClusterOffset;

            if (heightMaps.LeftDown == null)
            {
                clusterCount = 4;
            }
            else
            {
                clusterCount = 1;
            }

            int minX = int.MaxValue;
            int minY = int.MaxValue;

            int maxX = 0;
            int maxY = 0;

            //得到在4个Cluster中的坐标相对值
            Vector3 relativePos = pointa - offset;
            int ux = (int)(relativePos.X / cellUnit);
            int uy = (int)(relativePos.Z / cellUnit);

            if (ux < minX)
                minX = ux;
            else if (ux > maxX)
                maxX = ux;

            if (uy < minY)
                minY = uy;
            else if (uy > maxY)
                maxY = uy;


            relativePos = pointb - offset;
            ux = (int)(relativePos.X / cellUnit);
            uy = (int)(relativePos.Z / cellUnit);

            if (ux < minX)
                minX = ux;
            else if (ux > maxX)
                maxX = ux;

            if (uy < minY)
                minY = uy;
            else if (uy > maxY)
                maxY = uy;


            relativePos = pointb - offset;
            ux = (int)(relativePos.X / cellUnit);
            uy = (int)(relativePos.Z / cellUnit);

            if (ux < minX)
                minX = ux;
            else if (ux > maxX)
                maxX = ux;

            if (uy < minY)
                minY = uy;
            else if (uy > maxY)
                maxY = uy;

            maxX++; maxY++;

            minX--;
            minY--;

            if (minX < 0)
                minX = 0;
            if (minY < 0)
                minY = 0;

            //float w = width * widthEffectFactor / 4;


            Vector3 ab = pointb - pointa;
            Vector3 bc = pointc - pointb;
            Vector3 ca = pointa - pointc;

            Plane plane = new Plane(pointa, pointb, pointc);

            if (Math.Abs(Vector3.Dot(plane.Normal, Vector3.UnitY)) < 0.2f)
                return;

            float radius = cellUnit * 2;

            //遍历访问每个需要检查的点
            for (int dx = minX; dx <= maxX; dx++)
            {
                for (int dy = minY; dy <= maxY; dy++)
                {
                    //如果这个点在地图中,则检查高度,这里需要特别考虑交界处的情况
                    if (IsPointInMap(dx, dy, Cluster.ClusterSize, clusterCount))
                    {
                        Vector3 point = new Vector3(dx * cellUnit, 0, dy * cellUnit);

                        point.Y = (-plane.D - plane.Normal.Z * point.Z - plane.Normal.X * point.X) / plane.Normal.Y;

                        float res1 = Vector3.Dot(point - pointa, Vector3.Cross(ab, Vector3.UnitY));
                        float res2 = Vector3.Dot(point - pointb, Vector3.Cross(bc, Vector3.UnitY));
                        float res3 = Vector3.Dot(point - pointc, Vector3.Cross(ca, Vector3.UnitY));

                        //float res1 = (vCentre.x - t.vA.x) *( (t.vB.x - t.vA.x)*)


                        if ((res1 >= -radius & res2 >= -radius & res3 >= -radius) ||
                            (res1 <= radius & res2 <= radius & res3 <= radius))
                        {

                            //如果当前地形点的高度高于路径点,则把路径调低
                            if (heightMaps[dx, dy] * heightScale >= point.Y - MaxHeightOverTerrain)
                            {
                                heightMaps[dx, dy] = (point.Y - MaxHeightOverTerrain) / heightScale;
                                //heightMapModified.Add(new Point(dx, dy), heightMaps[dx, dy]);
                            }
                        }
                    }
                }
            }


            //float[] weights = new float[dOffset] { 0.125f, 0.225f, 0.3f, 0.225f, 0.125f };

            ////遍历Dictionary里面的内容,进行平滑运算
            //foreach (KeyValuePair<Point, float> unit in heightMapModified)
            //{
            //    float[] lineResults = new float[dOffset];

            //    for (int dx = 0; dx < dOffset; dx++)
            //    {
            //        for (int dy = dOffset; dy < dOffset; dy++)
            //        {
            //            Point newPoint = new Point(dx - dOffset / 2 + unit.Key.X, dy - dOffset / 2 + unit.Key.Y);
            //            Point previousPoint = unit.Key;
            //            float outvalue;
            //            if (!heightMapModified.TryGetValue(newPoint, out outvalue))
            //            {
            //                if (IsPointInMap(newPoint.X, newPoint.Y, Cluster.ClusterSize, clusterCount))
            //                {
            //                    outvalue = heightMaps[newPoint.X, newPoint.Y];
            //                    //int manhattanDis = MathEx.ManhattanDis(newPoint, previousPoint);
            //                    //float effectValue = manhattanDis * effectFactor;

            //                    //if ((effectValue <= 1 - zero) || (effectValue >= zero))
            //                    //{
            //                    //    float newHeight = effectValue * heightMaps[newPoint.X, newPoint.Y] +
            //                    //        (1 - effectValue) * heightMaps[previousPoint.X, previousPoint.Y];

            //                    //    if (newHeight < heightMaps[newPoint.X, newPoint.Y])
            //                    //    {
            //                    //        heightMaps[newPoint.X, newPoint.Y] = newHeight;
            //                    //    }
            //                    //}
            //                }
            //            }

            //            outvalue = weights[dy] * outvalue;
            //            lineResults[dx] += outvalue;
            //        }
            //    }

            //    float result = 0;
            //    for (int i = 0; i < lineResults.Length; i++)
            //    {
            //        result += weights[i] * lineResults[i];
            //    }

            //    heightMaps[unit.Key.X, unit.Key.Y] = result;
            //}
        }

        bool IsPointInMap(int x, int y, int clusterSize, int clusterCount)
        {
            if (clusterCount == 4)
            {
                if ((x >= 0) && (y >= 0) && (x <= clusterSize * 2 - 2) && (y <= clusterSize * 2 - 2))
                {
                    return true;
                }
            }
            else if (clusterCount == 1)
            {

                if ((x >= 0) && (y >= 0) && (x < clusterSize) && (y < clusterSize))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
