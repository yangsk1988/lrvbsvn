using SlimDX.Direct3D10;

namespace VirtualBicycle
{
    /// <summary>
    /// Manages aspects of the graphics device unique to Direct3D10.
    /// </summary>
    public class Direct3D10Manager
    {
        GraphicsDeviceManager manager;

		private Device m_Device;
        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        /// <value>The graphics device.</value>
        public Device Device
        {
			get { return m_Device; }
			internal set { m_Device = value; }
        }

		private SlimDX.DXGI.SwapChain m_SwapChain;
        /// <summary>
        /// Gets the swap chain.
        /// </summary>
        /// <value>The swap chain.</value>
        public SlimDX.DXGI.SwapChain SwapChain
        {
			get { return m_SwapChain; }
			internal set { m_SwapChain = value; }
        }

		private RenderTargetView m_RenderTarget;
        /// <summary>
        /// Gets the render target.
        /// </summary>
        /// <value>The render target.</value>
        public RenderTargetView RenderTarget
        {
			get { return m_RenderTarget; }
			internal set { m_RenderTarget = value; }
        }

		private DepthStencilView m_DepthStencilView;
        /// <summary>
        /// Gets the depth stencil view.
        /// </summary>
        /// <value>The depth stencil view.</value>
        public DepthStencilView DepthStencilView
        {
			get { return m_DepthStencilView; }
			internal set { m_DepthStencilView = value; }
        }

		private Texture2D m_DepthStencil;
        /// <summary>
        /// Gets the depth stencil surface.
        /// </summary>
        /// <value>The depth stencil surface.</value>
        public Texture2D DepthStencil
        {
			get { return m_DepthStencil; }
			internal set { m_DepthStencil = value; }
        }

		private RasterizerState m_RasterizerState;
        /// <summary>
        /// Gets the rasterizer state.
        /// </summary>
        /// <value>The rasterizer state.</value>
        public RasterizerState RasterizerState
        {
			get { return m_RasterizerState; }
			internal set { m_RasterizerState = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Direct3D10Manager"/> class.
        /// </summary>
        /// <param name="manager">The parent manager.</param>
        internal Direct3D10Manager(GraphicsDeviceManager manager)
        {
            this.manager = manager;
        }
    }
}
