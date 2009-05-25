#pragma once

#include "CacheMemoryTable.h"
#include "CacheMemory.h"

using namespace System;
using namespace System::IO;

namespace V3
{
	namespace Core
	{
		class Cache
		{
		private:
			CacheMemoryTable table;
			FileStream^ fileStream;

		public:

			 Cache(void);
			~Cache(void);
		};
	}
}