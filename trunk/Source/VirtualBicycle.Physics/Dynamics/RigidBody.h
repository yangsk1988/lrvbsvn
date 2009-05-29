#pragma once

using namespace System;

namespace V3
{
	namespace PhysicsEngine
	{
		namespace Dynamics
		{
			public ref class RigidBody
			{
			private:
				btRigidBody* rigidbody;

			public:

				RigidBody(float mass)
				{
				}
			};
		}
	}
}