
#include "CacheMemoryTable.h"

namespace V3
{
	namespace Core
	{
		void CacheMemoryTable::UseMemory(int offset, int size)
		{
			int chunkCount = (int)(size / ChunkSize);
			int rem = (int)(size % ChunkSize);
			if (rem > 0)
			{
				chunkCount++;
			}

			int startChunk = (int)(offset / ChunkSize);
			for (int i = 0; i < chunkCount; i++)
			{
				m_useStatus[startChunk + i] = true;
			}
			m_memoryUsed += chunkCount * ChunkSize;
		}

		void CacheMemoryTable::UseMemoryC(int startChunk, int count)
		{
			for (int i = 0; i < count; i++)
			{
				m_useStatus[startChunk + i] = true;
			}
			m_memoryUsed += count * ChunkSize;
		}

		void CacheMemoryTable::UnuseMemoryC(int startChunk, int count)
		{
			for (int i = 0; i < count; i++)
			{
				m_useStatus[startChunk + i] = false;
			}
			m_memoryUsed -= count * ChunkSize;
		}




		CacheMemoryTable::CacheMemoryTable(void)
		{
		}

		CacheMemoryTable::~CacheMemoryTable(void)
		{
			if (m_useStatus)
			{
				delete[] m_useStatus;
				m_useStatus = 0;
			}
		}
	}
}