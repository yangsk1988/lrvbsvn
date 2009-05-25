#pragma once

namespace V3
{
	namespace Core
	{
		class CacheMemoryTable
		{
		private:
			bool* m_useStatus;
			long m_memoryUsed;


			void setAvailableMemory(long value)
			{
				m_memoryUsed = TotalMemory - value;
			}

		public:
			static const long ChunkSize = 16 * 1024;
			static const long ChunkCount = (1048576L) / (ChunkSize / 1024);
			static const long TotalMemory = ChunkSize * ChunkCount;

			
			long getMemoryUsed()
			{
				return m_memoryUsed;
			}

			bool CheckIsUsed(bool chunkId)
			{
				return m_useStatus[chunkId];
			}
			
			long getAvailableMemory()
			{
				return TotalMemory - getMemoryUsed();
			}

			bool IsUsed(int offset)
			{
				return m_useStatus[offset / ChunkSize];
			}

			void UseMemory(int offset, int size);

			void UseMemoryC(int startChunk, int count);

			void UnuseMemoryC(int startChunk, int count);

			CacheMemoryTable(void);
			~CacheMemoryTable(void);
		};
	}
}