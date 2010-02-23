using VirtualBicycle.MathLib;

namespace VirtualBicycle.Graphics.ParticleSystem
{
    /// <summary>
    /// Custom vertex structure for drawing point sprite particles.
    /// </summary>
    public struct ParticleVertex
    {
        /// <summary>
        /// Stores the starting position of the particle.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Stores the starting velocity of the particle.
        /// </summary>
        public Vector3 Velocity;

        /// <summary>
        /// Four random values, used to make each particle look slightly different.
        /// </summary>
        public Color Random;

        /// <summary>
        /// The time (in seconds) at that this particle was created.
        /// </summary>
        public float Time;


        /// <summary>
        /// Describe the layout of this vertex structure.
        /// </summary>
        public static readonly VertexElement[] VertexElements =
        {
            new VertexElement( 0, VertexElementFormat.Vector3,
                                    VertexElementUsage.Position, 0),

            new VertexElement(12, VertexElementFormat.Vector3,
                                     VertexElementUsage.Normal, 0),

            new VertexElement(24, VertexElementFormat.Color,
                                     VertexElementUsage.Color, 0),

            new VertexElement( 28, VertexElementFormat.Single,
                                     VertexElementUsage.TextureCoordinate, 0),
        };


        /// <summary>
        /// Describe the size of this vertex structure.
        /// </summary>
        public const int SizeInBytes = 32;
    }
}
