#pragma once

#include "BulletCollision\CollisionShapes\btCollisionShape.h"

namespace V3
{
	namespace PhysicsEngine
	{
		namespace Collision
		{
			public ref class CollisionShapeBase abstract
			{
			private:
				btCollisionShape* m_shape;

			public:

				CollisionShapeBase(btCollisionShape* collShape)
					: m_shape(collShape)
				{
				}
			};
		}
	}
}