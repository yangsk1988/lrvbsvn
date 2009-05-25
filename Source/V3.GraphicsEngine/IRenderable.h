#pragma once

namespace V3
{
	namespace GraphicsEngine
	{
		/// <summary>
		///  定义一种通过 获取渲染操作(RenderOperation) 的方式 的一种便于管理的渲染方式
		/// </summary>
		public interface struct IRenderable
		{
	        /// <summary>
			///  获得渲染操作
			/// </summary>
			/// <returns></returns>
			virtual array<RenderOperation>^ GetRenderOperation() = 0;
		};
	}
}
