using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Logic;
using VirtualBicycle.Collections;
using VirtualBicycle.CollisionModel.Broadphase;
using VirtualBicycle.CollisionModel.Dispatch;

namespace VirtualBicycle
{
    public class VBCollisionDispatcher : CollisionModel.Dispatch.CollisionDispatcher
    {
        public void DefaultNearCallback(ref BroadphasePair collisionPair, CollisionDispatcher dispatcher, DispatcherInfo dispatchInfo)
        {
            CollisionObject collisionObjectA = collisionPair.ProxyA.ClientData as CollisionObject;
            CollisionObject collisionObjectB = collisionPair.ProxyB.ClientData as CollisionObject;

            if (dispatcher.NeedsCollision(collisionObjectA, collisionObjectB))
            {
                //dispatcher will keep algorithms persistent in the collision pair
                if (collisionPair.CollisionAlgorithm == null)
                {
                    collisionPair.CollisionAlgorithm = dispatcher.FindAlgorithm(collisionObjectA, collisionObjectB);
                }

                if (collisionPair.CollisionAlgorithm != null)
                {
                    ManifoldResult contactPointResult = new ManifoldResult(collisionObjectA, collisionObjectB);

                    if (dispatchInfo.DispatchFunction == DispatchFunction.Discrete)
                    {
                        //discrete collision detection query
                        collisionPair.CollisionAlgorithm.ProcessCollision(collisionObjectA, collisionObjectB, dispatchInfo, contactPointResult);
                    }
                    else
                    {
                        //continuous collision detection query, time of impact (toi)
                        float timeOfImpact = collisionPair.CollisionAlgorithm.CalculateTimeOfImpact(collisionObjectA, collisionObjectB, dispatchInfo, contactPointResult);
                        if (dispatchInfo.TimeOfImpact > timeOfImpact)
                            dispatchInfo.TimeOfImpact = timeOfImpact;
                    }
                }
            }
        }
    }
}
