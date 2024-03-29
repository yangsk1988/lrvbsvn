/*
  Bullet for XNA Copyright (c) 2007-2009 Vsevolod Klementjev & Mikhail Pashnin http://www.codeplex.com/xnadevru
  Bullet original C++ version Copyright (c) 2003-2007 Erwin Coumans http://bulletphysics.com

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.CollisionModel.Broadphase;

namespace VirtualBicycle.CollisionModel.Dispatch
{
	/// <summary>
	/// EmptyAlgorithm is a stub for unsupported collision pairs.
	/// The dispatcher can dispatch a persistent btEmptyAlgorithm to avoid a search every frame.
	/// </summary>
	[Serializable]
	public class EmptyAlgorithm : CollisionAlgorithm
	{
		public EmptyAlgorithm(CollisionAlgorithmConstructionInfo collisionAlgorithmConstructionInfo)
			: base(collisionAlgorithmConstructionInfo) { }

		public override void ProcessCollision(CollisionObject bodyA, CollisionObject bodyB, DispatcherInfo dispatchInfo, ManifoldResult resultOut) { }

		public override float CalculateTimeOfImpact(CollisionObject bodyA, CollisionObject bodyB, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
		{
			return 1f;
		}

		[Serializable]
		public class CreateFunc : CollisionAlgorithmCreateFunction
		{
			public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo collisionAlgorithmConstructionInfo, CollisionObject bodyA, CollisionObject bodyB)
			{
				return new EmptyAlgorithm(collisionAlgorithmConstructionInfo);
			}
		};
	}
}
