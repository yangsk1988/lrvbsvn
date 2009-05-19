#include "BulletCollision/CollisionDispatch/btCollisionWorld.h"
#include "BulletDynamics/ConstraintSolver/btContactSolverInfo.h"

#pragma once
#pragma managed

namespace V3.PhysicsEngine.Dynamics
{
	public ref class CollisionWorld
	{
	protected:
		btCollisionWorld* m_world;
	public:

		CollisionWorld(btCollisionWorld* world)
			: m_world(world)
		{
		}

		property BroadPhaseInterface BroadPhaseInterface
		{
			BroadPhaseInterface get();
			void set(BroadPhaseInterface value);
		}
	};
}