#pragma once

namespace V3.Core
{
	class AsyncLoader
	{
	private:
		Queue<ResourceEntity> m_operationQueue;

	public:
		AsyncLoader(void);
		~AsyncLoader(void);
	};
}