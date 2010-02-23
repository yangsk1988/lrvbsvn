using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.MathLib;
using VirtualBicycle.Graphics;

namespace VirtualBicycle.Graphics
{
    /// <summary>
    /// 一个渲染操作由下列三个元素组成:
    ///     变换
    ///     材质
    ///     几何信息
    /// </summary>
    public struct RenderOperation
    {
        public Matrix Transformation;

        public Material Material;

        public GeomentryData Geomentry;        
    }

    public class GeomentryData
    {
        IRenderable sender;

        /// <summary>
        /// 渲染操作的来源
        /// </summary>
        public IRenderable Sender
        {
            get { return sender; }
        }

        ///// <summary>
        /////  获取或设置顶点格式
        ///// </summary>
        //public VertexFormat Format
        //{
        //    get;
        //    set;
        //}
        public int VertexSize
        {
            get;
            set;
        }
        public VertexDeclaration VertexDeclaration
        {
            get;
            set;
        }



        public VertexBuffer VertexBuffer
        {
            get;
            set;
        }
        public IndexBuffer IndexBuffer
        {
            get;
            set;
        }
        public bool UseIndices
        {
            get { return IndexBuffer != null; }
        }
       
        public PrimitiveType PrimitiveType
        {
            get;
            set;
        }
        public int PrimCount
        {
            get;
            set;
        }
        public int VertexCount
        {
            get;
            set;
        }

        public GeomentryData(IRenderable obj)
        {
            sender = obj;
        }

        public int BaseVertex
        {
            get;
            set;
        }
        public int BaseIndexStart
        {
            get;
            set;
        }
    }
}
