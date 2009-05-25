#pragma once

#include "FileBase.h"

using namespace System;
using namespace System::IO;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;

namespace V3
{
	namespace IO
	{
		ref class Archive;

		[StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
		public value struct ArchiveFileEntry
		{
			unsigned int Id;
			long Offset;
			long Size;
		};

	    /// <summary>
		///  为文件包定义抽象工厂
		/// </summary>
		public ref class ArchiveFactory abstract
		{
		public:
			virtual Archive CreateInstance(String^ file) = 0;
			virtual Archive CreateInstance(FileLocation^ fl) = 0;

			virtual property String^ Type
			{
				String^ get();
			}
		};

		/// <summary>
		///  为文件包定义抽象基类
		/// </summary>
		public ref class Archive abstract
		{
		protected:
			Archive(void);
		public:
			
		};
	}
}