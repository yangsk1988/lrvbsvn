#include "btBulletDynamicsCommon.h"
#include "btBulletCollisionCommon.h"

#pragma once


#pragma managed

using namespace System;

namespace V3.PhysicsEngine.Dynamics
{
	public ref class DynamicsWorld
	{
	private:
		btDiscreteDynamicsWorld* world;
		btDispatcher* dispatcher;
		btAxisSweep3* broadPhase;
		btSequentialImpulseConstraintSolver* conSolver;

	public:

		DynamicsWorld(void)
		{			
			dispatcher = new btDispatcher();
			broadPhase = new btAxisSweep3(
				btVector3(btScalar(-10000), btScalar(-10000), btScalar(-10000)),
				btVector3(btScalar( 10000), btScalar( 10000), btScalar( 10000)), 1024);
			conSolver = new btSequentialImpulseConstraintSolver();

			world = new btDiscreteDynamicsWorld(dispatcher, broadPhase, conSolver, 0); 
		}
	};
}