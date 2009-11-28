using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.MathLib;

namespace Apoc3D.Graphics
{
    public struct VertexPBNT1
    {
        static VertexElement[] elements;

        public Vector3 pos;
        public Vector4 blend;
        public byte boneId1;
        public byte boneId2;
        public byte boneId3;
        public byte boneId4;

        public Vector3 n;

        public float u, v;

        static VertexPBNT1() 
        {
            elements = new VertexElement[5];

            elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
            elements[1] = new VertexElement(elements[0].Size, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight);
            elements[2] = new VertexElement(elements[1].Offset + elements[1].Size, VertexElementFormat.Color, VertexElementUsage.BlendIndices);
            elements[3] = new VertexElement(elements[2].Offset + elements[2].Size, VertexElementFormat.Vector3, VertexElementUsage.Normal);
            elements[4] = new VertexElement(elements[3].Offset + elements[3].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);

        }
        /// <summary>
        ///  获取一个VertexElement数组，包含该顶点格式的元素
        /// </summary>
        public static VertexElement[] Elements
        {
            get { return elements; }
        }

        public override int GetHashCode()
        {
            return pos.GetHashCode() ^ n.GetHashCode();
        }
        /// <summary>
        ///  获取该顶点的System.String表达形式
        /// </summary>
        /// <returns>该顶点的System.String表达形式</returns>
        public override string ToString()
        {
            return "Pos: " + pos.ToString() + "N: " + n.ToString();
        }

    }

    public struct VertexPN
    {
        static VertexElement[] elements;

        /// <summary>
        ///  获取或设置顶点的位置
        /// </summary>
        public Vector3 pos;

        /// <summary>
        ///  获取或设置顶点的法向量
        /// </summary>
        public Vector3 n;


        static VertexPN() 
        {
            elements = new VertexElement[2];

            elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
            elements[1] = new VertexElement(elements[0].Size, VertexElementFormat.Vector3, VertexElementUsage.Normal);
        }

        /// <summary>
        ///  获取一个VertexElement数组，包含该顶点格式的元素
        /// </summary>
        public static VertexElement[] Elements
        {
            get { return elements; }
        }

        public override int GetHashCode()
        {
            return pos.GetHashCode() ^ n.GetHashCode();
        }
        /// <summary>
        ///  获取该顶点的System.String表达形式
        /// </summary>
        /// <returns>该顶点的System.String表达形式</returns>
        public override string ToString()
        {
            return "Pos: " + pos.ToString() + "N: " + n.ToString();
        }
    }

    /// <summary>
    ///  定义具有位置、法向量和一组二维纹理坐标的顶点
    /// </summary>
    public struct VertexPNT1
    {
        static VertexElement[] elements;

        /// <summary>
        ///  获取或设置顶点的位置
        /// </summary>
        public Vector3 pos;

        /// <summary>
        ///  获取或设置顶点的法向量
        /// </summary>
        public Vector3 n;

        /// <summary>
        ///  获取或设置纹理坐标u
        /// </summary>
        public float u;

        /// <summary>
        ///  获取或设置纹理坐标v
        /// </summary>
        public float v;


        static VertexPNT1()
        {
            elements = new VertexElement[3];

            elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
            elements[1] = new VertexElement(elements[0].Size, VertexElementFormat.Vector3, VertexElementUsage.Normal);
            elements[2] = new VertexElement(elements[1].Offset + elements[1].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
        }

        /// <summary>
        ///  获取一个VertexElement数组，包含该顶点格式的元素
        /// </summary>
        public static VertexElement[] Elements
        {
            get { return elements; }
        }

        public unsafe static int Size
        {
            get { return sizeof(VertexPNT1); }
        }

        public override int GetHashCode()
        {
            return pos.GetHashCode() ^ n.GetHashCode() ^ u.GetHashCode() ^ v.GetHashCode();        }


        /// <summary>
        ///  获取该顶点的System.String表达形式
        /// </summary>
        /// <returns>该顶点的System.String表达形式</returns>
        public override string ToString()
        {
            return "Pos: " + pos.ToString() + "N: " + n.ToString() + "uv: " + u.ToString() + "," + v.ToString();
        }
    }

    /// <summary>
    ///  定义具有位置、法向量和两组二维纹理坐标的顶点
    /// </summary>
    public struct VertexPNT2
    {
        static VertexElement[] elements;

        /// <summary>
        ///  获取或设置顶点的位置
        /// </summary>
        public Vector3 pos;

        /// <summary>
        ///  获取或设置顶点的法向量
        /// </summary>
        public Vector3 n;

        /// <summary>
        ///  获取或设置第一组的纹理坐标u
        /// </summary>
        public float u1;

        /// <summary>
        ///  获取或设置第一组的纹理坐标v
        /// </summary>
        public float v1;


        /// <summary>
        ///  获取或设置第二组的纹理坐标u
        /// </summary>
        public float u2;

        /// <summary>
        ///  获取或设置第二组的纹理坐标v
        /// </summary>
        public float v2;

        static VertexPNT2()
        {
            elements = new VertexElement[4];

            elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
            elements[1] = new VertexElement(elements[0].Size, VertexElementFormat.Vector3, VertexElementUsage.Normal);
            elements[2] = new VertexElement(elements[1].Offset + elements[1].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
            elements[3] = new VertexElement(elements[2].Offset + elements[2].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1);
        }

        /// <summary>
        ///  获取一个VertexElement数组，包含该顶点格式的元素
        /// </summary>
        public static VertexElement[] Elements
        {
            get { return elements; }
        }


        /// <summary>
        ///  获取该顶点的System.String表达形式
        /// </summary>
        /// <returns>该顶点的System.String表达形式</returns>
        public override string ToString()
        {
            return "Pos: " + pos.ToString() + "N: " + n.ToString()
                + "uv1: " + u1.ToString() + "," + v1.ToString()
                + "uv2: " + u2.ToString() + "," + v2.ToString();
        }
    }

    /// <summary>
    ///  定义具有位置、法向量和三组二维纹理坐标的顶点
    /// </summary>
    public struct VertexPNT3
    {
        static VertexElement[] elements;

        /// <summary>
        ///  获取或设置顶点的位置
        /// </summary>
        public Vector3 pos;

        /// <summary>
        ///  获取或设置顶点的法向量
        /// </summary>
        public Vector3 n;

        /// <summary>
        ///  获取或设置第一组的纹理坐标u
        /// </summary>
        public float u1;
        /// <summary>
        ///  获取或设置第一组的纹理坐标v
        /// </summary>
        public float v1;
        /// <summary>
        ///  获取或设置第二组的纹理坐标u
        /// </summary>
        public float u2;
        /// <summary>
        ///  获取或设置第二组的纹理坐标v
        /// </summary>
        public float v2;
        /// <summary>
        ///  获取或设置第三组的纹理坐标u
        /// </summary>
        public float u3;
        /// <summary>
        ///  获取或设置第三组的纹理坐标v
        /// </summary>
        public float v3;

        static VertexPNT3()
        {
            elements = new VertexElement[5];

            elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
            elements[1] = new VertexElement(elements[0].Size, VertexElementFormat.Vector3, VertexElementUsage.Normal);
            elements[2] = new VertexElement(elements[1].Offset + elements[1].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
            elements[3] = new VertexElement(elements[2].Offset + elements[2].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1);
            elements[4] = new VertexElement(elements[3].Offset + elements[3].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2);
        }

        /// <summary>
        ///  获取一个VertexElement数组，包含该顶点格式的元素
        /// </summary>
        public static VertexElement[] Elements
        {
            get { return elements; }
        }

        /// <summary>
        ///  获取该顶点的System.String表达形式
        /// </summary>
        /// <returns>该顶点的System.String表达形式</returns>
        public override string ToString()
        {
            return "Pos: " + pos.ToString() + "N: " + n.ToString()
                + "uv1: " + u1.ToString() + "," + v1.ToString()
                + "uv2: " + u2.ToString() + "," + v2.ToString()
                + "uv3: " + u3.ToString() + "," + v3.ToString();
        }
    }

    /// <summary>
    ///  定义具有位置、法向量和四组二维纹理坐标的顶点
    /// </summary>
    public struct VertexPNT4
    {
        static VertexElement[] elements;

        /// <summary>
        ///  获取或设置顶点的位置
        /// </summary>
        public Vector3 pos;

        /// <summary>
        ///  获取或设置第一组的纹理坐标u
        /// </summary>
        public Vector3 n;

        /// <summary>
        ///  获取或设置第一组的纹理坐标u
        /// </summary>
        public float u1;
        /// <summary>
        ///  获取或设置第一组的纹理坐标v
        /// </summary>
        public float v1;
        /// <summary>
        ///  获取或设置第二组的纹理坐标u
        /// </summary>
        public float u2;
        /// <summary>
        ///  获取或设置第二组的纹理坐标v
        /// </summary>
        public float v2;
        /// <summary>
        ///  获取或设置第三组的纹理坐标u
        /// </summary>
        public float u3;
        /// <summary>
        ///  获取或设置第三组的纹理坐标v
        /// </summary>
        public float v3;
        /// <summary>
        ///  获取或设置第四组的纹理坐标u
        /// </summary>
        public float u4;
        /// <summary>
        ///  获取或设置第四组的纹理坐标v
        /// </summary>
        public float v4;

        static VertexPNT4() 
        {
            elements = new VertexElement[6];

            elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
            elements[1] = new VertexElement(elements[0].Size, VertexElementFormat.Vector3, VertexElementUsage.Normal);
            elements[2] = new VertexElement(elements[1].Offset + elements[1].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
            elements[3] = new VertexElement(elements[2].Offset + elements[2].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1);
            elements[4] = new VertexElement(elements[3].Offset + elements[3].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2);
            elements[5] = new VertexElement(elements[4].Offset + elements[4].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 3);

        }

        /// <summary>
        ///  获取一个VertexElement数组，包含该顶点格式的元素
        /// </summary>
        public static VertexElement[] Elements
        {
            get { return elements; }
        }


        /// <summary>
        ///  获取该顶点的System.String表达形式
        /// </summary>
        /// <returns>该顶点的System.String表达形式</returns>
        public override string ToString()
        {
            return "Pos: " + pos.ToString() + "N: " + n.ToString()
                + "uv1: " + u1.ToString() + "," + v1.ToString()
                + "uv2: " + u2.ToString() + "," + v2.ToString()
                + "uv3: " + u3.ToString() + "," + v3.ToString()
                + "uv4: " + u4.ToString() + "," + v4.ToString();
        }
    }

    /// <summary>
    ///  定义具有位置和一组二维纹理坐标的顶点
    /// </summary>
    public struct VertexPT1
    {
        static VertexElement[] elements;

        /// <summary>
        ///  获取或设置顶点的位置
        /// </summary>
        public Vector3 pos;

        /// <summary>
        ///  获取或设置纹理坐标u
        /// </summary>
        public float u1;

        /// <summary>
        ///  获取或设置纹理坐标v
        /// </summary>
        public float v1;

        static VertexPT1()
        {
            elements = new VertexElement[2];

            elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
            elements[1] = new VertexElement(elements[0].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
        
        }
        /// <summary>
        ///  获取一个VertexElement数组，包含该顶点格式的元素
        /// </summary>
        public static VertexElement[] Elements
        {
            get { return elements; }
        }

        /// <summary>
        ///  获取该顶点的System.String表达形式
        /// </summary>
        /// <returns>该顶点的System.String表达形式</returns>
        public override string ToString()
        {
            return "Pos: " + pos.ToString() + "uv: " + u1.ToString() + "," + v1.ToString();
        }
    }
    public struct VertexPT2
    {
        static VertexElement[] elements;

        /// <summary>
        ///  获取或设置顶点的位置
        /// </summary>
        public Vector3 pos;
        /// <summary>
        ///  获取或设置第一组的纹理坐标u
        /// </summary>
        public float u1;

        /// <summary>
        ///  获取或设置第一组的纹理坐标v
        /// </summary>
        public float v1;


        /// <summary>
        ///  获取或设置第二组的纹理坐标u
        /// </summary>
        public float u2;

        /// <summary>
        ///  获取或设置第二组的纹理坐标v
        /// </summary>
        public float v2;

        static VertexPT2()
        {
            elements = new VertexElement[3];

            elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
            elements[1] = new VertexElement(elements[0].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
            elements[2] = new VertexElement(elements[1].Offset + elements[1].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1);
        }

        /// <summary>
        ///  获取一个VertexElement数组，包含该顶点格式的元素
        /// </summary>
        public static VertexElement[] Elements
        {
            get { return elements; }
        }

        /// <summary>
        ///  获取该顶点的System.String表达形式
        /// </summary>
        /// <returns>该顶点的System.String表达形式</returns>
        public override string ToString()
        {
            return "Pos: " + pos.ToString()
                + "uv1: " + u1.ToString() + "," + v1.ToString()
                + "uv2: " + u2.ToString() + "," + v2.ToString();
        }
    }
    public struct VertexPT3
    {
        static VertexElement[] elements;

        /// <summary>
        ///  获取或设置顶点的位置
        /// </summary>
        public Vector3 pos;

        /// <summary>
        ///  获取或设置第一组的纹理坐标u
        /// </summary>
        public float u1;
        /// <summary>
        ///  获取或设置第一组的纹理坐标v
        /// </summary>
        public float v1;
        /// <summary>
        ///  获取或设置第二组的纹理坐标u
        /// </summary>
        public float u2;
        /// <summary>
        ///  获取或设置第二组的纹理坐标v
        /// </summary>
        public float v2;
        /// <summary>
        ///  获取或设置第三组的纹理坐标u
        /// </summary>
        public float u3;
        /// <summary>
        ///  获取或设置第三组的纹理坐标v
        /// </summary>
        public float v3;

        static VertexPT3() 
        {
            elements = new VertexElement[4];

            elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
            elements[1] = new VertexElement(elements[0].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
            elements[2] = new VertexElement(elements[1].Offset + elements[1].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1);
            elements[3] = new VertexElement(elements[2].Offset + elements[2].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2);
        }

        /// <summary>
        ///  获取一个VertexElement数组，包含该顶点格式的元素
        /// </summary>
        public static VertexElement[] Elements
        {
            get { return elements; }
        }

        /// <summary>
        ///  获取该顶点的System.String表达形式
        /// </summary>
        /// <returns>该顶点的System.String表达形式</returns>
        public override string ToString()
        {
            return "Pos: " + pos.ToString()
                 + "uv1: " + u1.ToString() + "," + v1.ToString()
                 + "uv2: " + u2.ToString() + "," + v2.ToString()
                 + "uv3: " + u3.ToString() + "," + v3.ToString();
        }
    }
    public struct VertexPT4
    {
        static VertexElement[] elements;

        /// <summary>
        ///  获取或设置顶点的位置
        /// </summary>
        public Vector3 pos;
        /// <summary>
        ///  获取或设置第一组的纹理坐标u
        /// </summary>
        public float u1;
        /// <summary>
        ///  获取或设置第一组的纹理坐标v
        /// </summary>
        public float v1;
        /// <summary>
        ///  获取或设置第二组的纹理坐标u
        /// </summary>
        public float u2;
        /// <summary>
        ///  获取或设置第二组的纹理坐标v
        /// </summary>
        public float v2;
        /// <summary>
        ///  获取或设置第三组的纹理坐标u
        /// </summary>
        public float u3;
        /// <summary>
        ///  获取或设置第三组的纹理坐标v
        /// </summary>
        public float v3;
        /// <summary>
        ///  获取或设置第四组的纹理坐标u
        /// </summary>
        public float u4;
        /// <summary>
        ///  获取或设置第四组的纹理坐标v
        /// </summary>
        public float v4;

        static VertexPT4()
        {
            elements = new VertexElement[5];

            elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
            elements[1] = new VertexElement(elements[0].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
            elements[2] = new VertexElement(elements[1].Offset + elements[1].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1);
            elements[3] = new VertexElement(elements[2].Offset + elements[2].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2);
            elements[4] = new VertexElement(elements[3].Offset + elements[3].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 3);
        }

        /// <summary>
        ///  获取一个VertexElement数组，包含该顶点格式的元素
        /// </summary>
        public static VertexElement[] Elements
        {
            get { return elements; }
        }

        /// <summary>
        ///  获取该顶点的System.String表达形式
        /// </summary>
        /// <returns>该顶点的System.String表达形式</returns>
        public override string ToString()
        {
            return "Pos: " + pos.ToString()
               + "uv1: " + u1.ToString() + "," + v1.ToString()
               + "uv2: " + u2.ToString() + "," + v2.ToString()
               + "uv3: " + u3.ToString() + "," + v3.ToString()
               + "uv4: " + u4.ToString() + "," + v4.ToString();
        }
    }

    /// <summary>
    ///  定义具有位置和颜色的顶点
    /// </summary>
    public struct VertexPC
    {
        static VertexElement[] elements;

        /// <summary>
        ///  获取或设置顶点的位置
        /// </summary>
        public Vector3 pos;

        /// <summary>
        ///  获取或设置顶点颜色
        /// </summary>
        public int diffuse;

        static VertexPC()
        {
            elements = new VertexElement[2];

            elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
            elements[1] = new VertexElement(elements[0].Size, VertexElementFormat.Vector3, VertexElementUsage.Color);
        }

        public VertexPC(Vector3 pos, int diffuse)
        {
            this.pos = pos;
            this.diffuse = diffuse;
        }

        /// <summary>
        ///  获取一个VertexElement数组，包含该顶点格式的元素
        /// </summary>
        public static VertexElement[] Elements
        {
            get { return elements; }
        }

        /// <summary>
        ///  获取该顶点的System.String表达形式
        /// </summary>
        /// <returns>该顶点的System.String表达形式</returns>
        public override string ToString()
        {
            return "Pos: " + pos.ToString() + "clr: " + diffuse.ToString();
        }
    }

    public struct VertexPCT
    {
        static VertexElement[] elements;

        public Vector3 pos;
        public float shit;
        public int diffuse;
        public Vector2 texCoord;

        static VertexPCT()
        {
            elements = new VertexElement[3];

            elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
            elements[1] = new VertexElement(elements[0].Size, VertexElementFormat.Vector3, VertexElementUsage.Color);
            elements[2] = new VertexElement(elements[1].Offset + elements[1].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
        }

        public VertexPCT(float x, float y, float z, float rhw, int color, float u, float v)
        {
            this.pos.X = x;
            this.pos.Y = y;
            this.pos.Z = z;
            this.shit = rhw;
            this.diffuse = color;
            this.texCoord.X = u;
            this.texCoord.Y = v;
        }

        /// <summary>
        ///  获取一个VertexElement数组，包含该顶点格式的元素
        /// </summary>
        public static VertexElement[] Elements
        {
            get { return elements; }
        }
        /// <summary>
        ///  获取该顶点的大小
        /// </summary>
        public static int Size
        {
            get { return sizeof(float) * 7; }
        }
    }

    /// <summary>
    ///   定义具有位置的顶点
    /// </summary>
    public struct VertexP
    {
        static VertexElement[] elements;

        /// <summary>
        ///  获取或设置顶点的位置
        /// </summary>
        public Vector3 pos;

        static VertexP()
        {
            elements = new VertexElement[1];

            elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
        }

        /// <summary>
        ///  获取一个VertexElement数组，包含该顶点格式的元素
        /// </summary>
        public static VertexElement[] Elements
        {
            get { return elements; }
        }

        /// <summary>
        ///  获取该顶点的System.String表达形式
        /// </summary>
        /// <returns>该顶点的System.String表达形式</returns>
        public override string ToString()
        {
            return pos.ToString();
        }
    }
}
