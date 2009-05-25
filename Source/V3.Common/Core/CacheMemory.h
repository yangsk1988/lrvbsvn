#pragma once

using namespace System;
using namespace V3.IO;

namespace V3
{
	namespace Core
	{
		public enum CacheType
		{
			Static,
			Dynamic
		}

		class CacheMemory
		{
		public:

			long Offset;
			long Size;

			int StartChunkId;
			int ChunkCount;

			CacheType Type;
			String^ ExtFile;

			ResourceLocation^ ResourceLocation;
		};
	}
}