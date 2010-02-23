using System;
using System.Runtime.Serialization;
using VirtualBicycle.MathLib;

namespace VirtualBicycle.Graphics.ParticleSystem.Emitters
{
    /// <summary>
    /// Represents an emitter which releases particles in a spray like manner.
    /// </summary>
    public class SprayEmitter<T> : Emitter<T> where T : IParticle, new()
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="budget">The number of particles that will be available to the emitter.</param>
        /// <param name="term">The lifetime of released particles in whole and fractional seconds.</param>
        /// <param name="angle">The angle of the spray in radians.</param>
        /// <param name="spread">The spread/focus of the spray in radians.</param>
        public SprayEmitter(int budget, float term, float angle, float spread)
            : base(budget, term)
        {
            this._angle = angle;
            this._spread = spread;
        }

        private float _angle;       //Angle of the spray.
        private float _spread;      //Spread of the spray.

        /// <summary>
        /// Generates offset and orientation vectors for a released particle.
        /// </summary>
        /// <param name="offset">The offset of the particle from the position of the trigger.</param>
        /// <param name="orientation">The orientation of the particle as a unit vector.</param>
        protected override void GenerateParticleOffsetAndOrientation(out Vector2 offset, out Vector2 orientation)
        {
            offset = Vector2.Zero;

            float rand = (float)Random.NextDouble();

            float dir = this._angle;

            dir += ((this._spread * rand) - this._spread);

            orientation = new Vector2((float)Math.Sin(dir), (float)Math.Cos(dir));
        }
    }
}
