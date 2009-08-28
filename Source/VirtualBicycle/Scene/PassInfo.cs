using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Collections;
using VirtualBicycle.Graphics;
using VirtualBicycle.Graphics.Effects;

namespace VirtualBicycle.Scene
{
    /// <summary>
    ///  渲染场景前检测可见物体时，<see cref="BatchData"/>对象用来存储要渲染的物体的
    /// </summary>
    public class BatchData
    {
        /// <summary>
        ///  按效果批次分组，一个效果批次有一个RenderOperation列表
        /// </summary>
        public Dictionary<string, FastList<RenderOperation>> BatchTable;

        /// <summary>
        ///  按效果批次名称查询效果的哈希表
        /// </summary>
        public Dictionary<string, ModelEffect> Effects;

        public Dictionary<string, Dictionary<MeshMaterial, Dictionary<GeomentryData, FastList<RenderOperation>>>> InstanceTable;

        public FastList<SceneObject> VisibleObjects;

        public int RenderedObjectCount;
    }

}
