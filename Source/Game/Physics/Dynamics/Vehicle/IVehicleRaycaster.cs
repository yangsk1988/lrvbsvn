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
using VirtualBicycle.Physics.MathLib;

namespace VirtualBicycle.Physics.Dynamics
{
    public interface IVehicleRaycaster
    {
        object CastRay(Vector3 from, Vector3 to, out VehicleRaycasterResult result);
    }

    [Serializable]
    public class VehicleRaycasterResult
    {
        private Single _distFraction;
        private Vector3 _hitNormalInWorld;
        private Vector3 _hitPointInWorld;

        public VehicleRaycasterResult()
        {
            _distFraction = -1;
        }

        public float DistFraction
        {
            get { return _distFraction; }
            set { _distFraction = value; }
        }
        public Vector3 HitNormalInWorld
        {
            get { return _hitNormalInWorld; }
            set { _hitNormalInWorld = value; }
        }
        public Vector3 HitPointInWorld
        {
            get { return _hitPointInWorld; }
            set { _hitPointInWorld = value; }
        }
    }
}
