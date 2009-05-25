#pragma once

#include "ResourceManager.h"

using namespace System;
using namespace System::ComponentModel;


namespace V3
{
	namespace Core
	{
		/// <summary>
		///  定义一个资源的状态
		/// </summary>
		public enum ResourceState
		{
			/// <summary>
			///  未加载
			/// </summary>
			Unloaded,
			/// <summary>
			///  正在加载
			/// </summary>
			Loading,        
			/// <summary>
			///  已经加载
			/// </summary>
			Loaded,
			/// <summary>
			///  正在卸载
			/// </summary>
			Unloading
		};


		/// <summary>
		///  表示一个资源，如纹理、模型等
		/// </summary>
		/// <remarks>
		/// 资源分两种，引用和实体
		/// 引用引用着资源实体
		/// 引用不受管理，但使用时会影响实体的状态
		/// 资源管理器根据实体资源的状态对其进行管理
		/// </remarks>
		public ref class ResourceEntity abstract
		{
		private:
			ResourceState m_resState;
			TimeSpan m_creationTime;
			TimeSpan m_lastAccessed;
			int m_accessTimes;
			int m_referenceCount;
			
			bool m_isUnmanaged;

			float m_useFrequency;

			ResourceManager^ m_manager;

			String^ m_hashString;


		protected:
			ResourceEntity()
				: m_isUnmanaged(true)
			{

			}

			ResourceEntity(ResourceManager^ manager, String^ hashString) : m_hashString(hashString),
																		  m_manager(manager)	
			{
				AllowDynamicLoading = true;
			}

			ResourceEntity::ResourceEntity(ResourceManager^ manager, String^ hashString, bool allowdl) : m_hashString(hashString),
																										 m_manager(manager)
			{
				AllowDynamicLoading = allowdl;
			}

		public:

			/// <summary>
			///  获取一个System.Boolean，表示该资源是否允许动态加载/卸载
			/// </summary>
			[BrowsableAttribute(false)]
			property bool AllowDynamicLoading
			{
				bool get();
				void set(bool value);
			}
			/// <summary>
			///  获取该资源的创建时间
			/// </summary>
			[BrowsableAttribute(false)]
			property TimeSpan CreationTime
			{
				TimeSpan get() { return m_creationTime; }
			}

			/// <summary>
			///  获取上次访问该资源的时间
			/// </summary>
			[BrowsableAttribute(false)]
			property TimeSpan LastAccessedTime
			{
				TimeSpan get() { return m_lastAccessed; }
			}

			/// <summary>
			///  获得管理该资源的资源管理器
			/// </summary>
			[BrowsableAttribute(false)]
			property ResourceManager^ Manager
			{
				ResourceManager^ get() { return m_manager; }
			}

			/// <summary>
			///  获取该资源的使用频率
			/// </summary>
			[BrowsableAttribute(false)]
			property float UseFrequency
			{
				float get() { return m_useFrequency; }
			}

			/// <summary>
			/// 获取资源实体的引用计数
			/// </summary>
			[BrowsableAttribute(false)]
			property int ReferenceCount
			{
				int get() { return m_referenceCount; }
				void set(int value) { m_referenceCount = value; }
			}

			[BrowsableAttribute(false)]
			property String^ HashString
			{
				String^ get() { return m_hashString; }
			}

			void Use(void);
			void Load(void);
			void Unload(void);
			

		};
	}
}