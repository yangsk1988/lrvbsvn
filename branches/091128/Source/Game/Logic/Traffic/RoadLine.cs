using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;
using VirtualBicycle.MathLib;
using VirtualBicycle.Scene;

namespace VirtualBicycle.Logic.Traffic
{
    public class RoadLine
    {
        #region 常量

        static readonly string NodeTag = "Node";
        static readonly string NodeCountTag = "NodeCount";

        static readonly string Version2Tag = "Version2";

        static readonly string LNodeTag = "LNode";
        static readonly string LNodeCountTag = "LNodeCount";

        /// <summary>
        /// 最小的顶点距离,第一次插值的时候使用
        /// </summary>
        const float firstMinPointDistance = 1f;

        /// <summary>
        /// 最小的定点距离,第二次插值的时候使用
        /// </summary>
        const float secondMinPointDistance = 1f;

        #endregion

        #region 字段

        /// <summary>
        /// 输入的顶点集合
        /// </summary>
        RoadNode[] inputPoints;

        #endregion

        #region 属性

        /// <summary>
        ///  获取或设置输入的顶点
        /// </summary>
        public RoadNode[] InputPoints
        {
            get { return inputPoints; }
            set { inputPoints = value; }
        }

        public List<RoadNode> InterposedPoints
        {
            get;
            private set;
        }

        #endregion

        #region 构造函数

        public RoadLine(RoadNode[] inputPoints)
        {
            this.inputPoints = inputPoints;
        }

        public RoadLine()
        {
        }

        #endregion

        #region 方法

        /// <summary>
        /// 得到输入点,输出插值后的输入点
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public void Interpose()
        {
            RoadNode[] sourcePoint = inputPoints;

            List<RoadNode> points = new List<RoadNode>(sourcePoint.Length * 4);

            #region 根据输入的顶点序列生成样条
            //CatMull 插值用
            RoadNode p1, p2, p3, p4;

            for (int i = 0; i < sourcePoint.Length; i++)
            {
                points.Add(sourcePoint[i]);
                if (i < sourcePoint.Length - 1)
                {
                    p1 = (i > 0) ? sourcePoint[i - 1] : sourcePoint[i];
                    p2 = sourcePoint[i];
                    p3 = sourcePoint[i + 1];
                    p4 = (i < sourcePoint.Length - 2) ? sourcePoint[i + 2] : sourcePoint[i + 1];

                    //先求出距离
                    float dis = MathEx.Distance(ref p2.Position, ref p3.Position);
                    //根据距离判断需要插值的点的数量
                    int pointCountInterposed = (int)(Math.Ceiling(dis / firstMinPointDistance));

                    RoadNode tempTrackVertex;
                    for (int j = 1; j <= pointCountInterposed; j++)
                    {
                        float amount = (float)j / pointCountInterposed;
                        tempTrackVertex.Position = MathEx.CatmullRom(p1.Position,
                            p2.Position, p3.Position, p4.Position, amount);
                        tempTrackVertex.Twist = MathEx.LinearInterpose(p2.Twist, p3.Twist, amount);
                        tempTrackVertex.Width = MathEx.LinearInterpose(p2.Width, p3.Width, amount);
                        points.Add(tempTrackVertex);
                    }
                }
            }
            InterposedPoints = points;

            #endregion
        }

        private float getMinDisOfAngle(float a)
        {
            double x = Math.Abs(a);

            if (x < 0.3)
            {
                return 10f;
            }
            else if (x < 0.5)
            {
                return 5f;
            }
            else if (x <0.7)
            {
                return 3f;
            }
            else
            {
                return 1f;
            }
        }
        public void Interpose(TCPort? head, TCPort? tail)
        {
            RoadNode[] sourcePoint = inputPoints;

            List<RoadNode> firstInterposePoints = new List<RoadNode>(sourcePoint.Length * 4);
            List<RoadNode> secondInterposePoints = new List<RoadNode>(sourcePoint.Length * 4);


            if (head != null)
            {
                TCPort tmp = head.Value;

                sourcePoint[0].Position = tmp.Position;
                sourcePoint[0].Width = tmp.Width;

                //float t = Vector3.Dot(Vector3.UnitY, tmp.Right);

                sourcePoint[0].Twist = tmp.Twist;// (float)Math.Acos(Vector3.Dot(Vector3.UnitY, tmp.Up));
            }
            if (tail != null)
            {
                TCPort tmp = tail.Value;
                int idx = sourcePoint.Length - 1;

                sourcePoint[idx].Position = tmp.Position;
                sourcePoint[idx].Width = tmp.Width;
                sourcePoint[idx].Twist = tmp.Twist;// (float)Math.Acos(Vector3.Dot(Vector3.UnitY, tmp.Up));
            }


            #region 根据输入的顶点序列生成样条
            //CatMull 插值用
            RoadNode p1, p2, p3, p4;

            //第一次插值
            for (int i = 0; i < sourcePoint.Length; i++)
            {
                firstInterposePoints.Add(sourcePoint[i]);

                if (i < sourcePoint.Length - 1)
                {
                    bool headCon = (head != null && i == 0);
                    bool tailCon = (tail != null && i == sourcePoint.Length - 2);
                    if (headCon || tailCon)
                    {
                        p1 = sourcePoint[i];
                        p2 = sourcePoint[i + 1];

                        //先求出距离
                        float dis = MathEx.Distance(ref p1.Position, ref p2.Position);
                        //根据距离判断需要插值的点的数量
                        int pointCountInterposed = (int)(Math.Ceiling(dis / firstMinPointDistance));

                        RoadNode tempTrackVertex;
                        for (int j = 1; j <= pointCountInterposed; j++)
                        {
                            float amount = (float)j / pointCountInterposed;
                            tempTrackVertex.Position = MathEx.LinearInterpose(p1.Position, p2.Position, amount);
                            tempTrackVertex.Twist = MathEx.LinearInterpose(p1.Twist, p2.Twist, amount);
                            tempTrackVertex.Width = MathEx.LinearInterpose(p1.Width, p2.Width, amount);
                            firstInterposePoints.Add(tempTrackVertex);
                        }
                    }
                    else
                    {
                        p1 = (i > 0) ? sourcePoint[i - 1] : sourcePoint[i];
                        p2 = sourcePoint[i];
                        p3 = sourcePoint[i + 1];
                        p4 = (i < sourcePoint.Length - 2) ? sourcePoint[i + 2] : p3;


                        //先求出距离
                        float dis = MathEx.Distance(ref p2.Position, ref p3.Position);
                        //根据距离判断需要插值的点的数量
                        int pointCountInterposed = (int)(Math.Ceiling(dis / firstMinPointDistance));

                        RoadNode tempTrackVertex;
                        for (int j = 1; j <= pointCountInterposed; j++)
                        {
                            float amount = (float)j / pointCountInterposed;
                            tempTrackVertex.Position = MathEx.CatmullRom(p1.Position,
                                p2.Position, p3.Position, p4.Position, amount);
                            tempTrackVertex.Twist = MathEx.LinearInterpose(p2.Twist, p3.Twist, amount);
                            tempTrackVertex.Width = MathEx.LinearInterpose(p2.Width, p3.Width, amount);
                            firstInterposePoints.Add(tempTrackVertex);
                        }
                    }
                }
            }

            //第二次插值
            float angle = 0f;
            float curLen = 0f;
            for (int i = 0; i < firstInterposePoints.Count; i++)
            {
                if ((i > 1) && (i < firstInterposePoints.Count - 1))
                {
                    Vector3 pA = firstInterposePoints[i].Position;
                    Vector3 pB = firstInterposePoints[i - 1].Position;
                    Vector3 pC = firstInterposePoints[i - 2].Position;
                    curLen += Vector3.Distance(pA, pB);
                    angle += MathEx.Vec3AngleAbs(pA- pC,pB - pC);

                    if ((angle > 0.1f) || (curLen > 5f))
                    {
                        secondInterposePoints.Add(firstInterposePoints[i]);
                        angle = 0f;
                        curLen = 0f;
                    }
                }
                else if ((i == 0) || (i == firstInterposePoints.Count - 1))
                {
                    secondInterposePoints.Add(firstInterposePoints[i]);
                }
            }
                //secondInterposePoints.Add(firstInterposePoints[i]);

                //if (i < firstInterposePoints.Count - 1)
                //{
                //    bool headCon = (head != null && i == 0);
                //    bool tailCon = (tail != null && i == firstInterposePoints.Count - 2);
                //    if (headCon || tailCon)
                //    {
                //        p1 = firstInterposePoints[i];
                //        p2 = firstInterposePoints[i + 1];

                //        //先求出距离
                //        float dis = MathEx.Distance(ref p1.Position, ref p2.Position);
                //        //根据距离判断需要插值的点的数量
                //        int pointCountInterposed = (int)(Math.Ceiling(dis / secondMinPointDistance));

                //        RoadNode tempTrackVertex;
                //        for (int j = 1; j <= pointCountInterposed; j++)
                //        {
                //            float amount = (float)j / pointCountInterposed;
                //            tempTrackVertex.Position = MathEx.LinearInterpose(p1.Position, p2.Position, amount);
                //            tempTrackVertex.Twist = MathEx.LinearInterpose(p1.Twist, p2.Twist, amount);
                //            tempTrackVertex.Width = MathEx.LinearInterpose(p1.Width, p2.Width, amount);
                //            secondInterposePoints.Add(tempTrackVertex);
                //        }
                //    }
                //    else
                //    {
                //        p1 = (i > 0) ? firstInterposePoints[i - 1] : firstInterposePoints[i];
                //        p2 = firstInterposePoints[i];
                //        p3 = firstInterposePoints[i + 1];
                //        p4 = (i < firstInterposePoints.Count - 2) ? firstInterposePoints[i + 2] : p3;

                //        //先求出距离
                //        float dis = MathEx.Distance(ref p2.Position, ref p3.Position);
                //        float angle; 
                //        angle = MathEx.Vec3AngleAbs(p2.Position - p1.Position, p3.Position - p1.Position);
                //        angle += MathEx.Vec3AngleAbs(p3.Position - p2.Position, p4.Position - p3.Position);
                        
                //        float minDis;
                //        //根据距离判断需要插值的点的数量
                //        minDis = getMinDisOfAngle(angle);
                //        int pointCountInterposed = (int)(Math.Ceiling(dis / minDis));

                //        RoadNode tempTrackVertex;
                //        for (int j = 1; j <= pointCountInterposed; j++)
                //        {
                //            float amount = (float)j / pointCountInterposed;
                //            tempTrackVertex.Position = MathEx.CatmullRom(p1.Position,
                //                p2.Position, p3.Position, p4.Position, amount);
                //            tempTrackVertex.Twist = MathEx.LinearInterpose(p2.Twist, p3.Twist, amount);
                //            tempTrackVertex.Width = MathEx.LinearInterpose(p2.Width, p3.Width, amount);
                //            secondInterposePoints.Add(tempTrackVertex);
                //        }
                //    }
                //}
            InterposedPoints = secondInterposePoints;

            #endregion
        }

        public virtual void ReadData(BinaryDataReader data)
        {
            int count = data.GetDataInt32(NodeCountTag);

            inputPoints = new RoadNode[count];

            for (int i = 0; i < count; i++)
            {
                ContentBinaryReader br = data.GetData(NodeTag + i.ToString());

                RoadNode vtx;
                vtx.Position.X = br.ReadSingle();
                vtx.Position.Y = br.ReadSingle();
                vtx.Position.Z = br.ReadSingle();

                vtx.Width = br.ReadSingle();

                vtx.Twist = br.ReadSingle();

                br.Close();

                inputPoints[i] = vtx;
            }

            if (data.Contains(Version2Tag))
            {
                count = data.GetDataInt32(LNodeCountTag);

                InterposedPoints = new List<RoadNode>(count);

                for (int i = 0; i < count; i++)
                {
                    ContentBinaryReader br = data.GetData(LNodeTag + i.ToString());

                    RoadNode vtx;
                    vtx.Position.X = br.ReadSingle();
                    vtx.Position.Y = br.ReadSingle();
                    vtx.Position.Z = br.ReadSingle();

                    vtx.Width = br.ReadSingle();

                    vtx.Twist = br.ReadSingle();
                    br.Close();

                    InterposedPoints.Add(vtx);
                }
            }
            else
            {
                Interpose();
            }
        }
        public virtual void WriteData(BinaryDataWriter data)
        {
            data.AddEntry(NodeCountTag, inputPoints.Length);

            for (int i = 0; i < inputPoints.Length; i++)
            {
                ContentBinaryWriter bw = data.AddEntry(NodeTag + i.ToString());

                RoadNode vtx = inputPoints[i];

                bw.Write(vtx.Position.X);
                bw.Write(vtx.Position.Y);
                bw.Write(vtx.Position.Z);

                bw.Write(vtx.Width);

                bw.Write(vtx.Twist);


                bw.Close();
            }

            data.AddEntry(Version2Tag);
            data.AddEntry(LNodeCountTag, InterposedPoints.Count);

            for (int i = 0; i < InterposedPoints.Count; i++)
            {
                ContentBinaryWriter bw = data.AddEntry(LNodeTag + i.ToString());

                RoadNode vtx = InterposedPoints[i];
                bw.Write(vtx.Position.X);
                bw.Write(vtx.Position.Y);
                bw.Write(vtx.Position.Z);

                bw.Write(vtx.Width);

                bw.Write(vtx.Twist);


                bw.Close();
            }
        }

        #endregion
    }
}
