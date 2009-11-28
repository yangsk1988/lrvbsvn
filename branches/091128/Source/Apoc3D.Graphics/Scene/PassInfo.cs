using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;

namespace Apoc3D.Scene
{
    /// <summary>
    ///  渲染场景时，PassInfo对象用来从Cluster的场景管理器收集信息，并存储
    /// </summary>
    public class PassInfo
    {
        ///// <summary>
        /////  按效果批次分组，一个效果批次有一个RenderOperation列表
        ///// </summary>
        //public Dictionary<string, FastList<RenderOperation>> batchTable;

        /// <summary>
        ///  按材质分组，一个材质批次有一个RenderOperation列表
        /// </summary>
        public Dictionary<Material, FastList<RenderControl>> batchTable;

        ///// <summary>
        /////  按效果批次名称查询效果的哈希表
        ///// </summary>
        //public Dictionary<string, Effect> effects;

        public Dictionary<Material, Dictionary<GeomentryData, FastList<RenderOperation>>> instanceTable;

        public FastList<SceneObject> visibleObjects;

        public int RenderedObjectCount;
    }

}
