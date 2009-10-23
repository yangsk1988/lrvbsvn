using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle.RenderSystem
{
    /// <summary>
    ///  创建图形API资源的抽象工厂。可以创建纹理，Shader，缓冲等等。
    /// </summary>
    public abstract class ObjectFactory
    {
        public RenderSystem RenderSystem
        {
            get;
            private set;
        }

        protected ObjectFactory(RenderSystem rs)
        {
            RenderSystem = rs;
        }

        public abstract Texture CreateReferenceTexture(Texture texture);

        public abstract Texture CreateTexture(System.Drawing.Bitmap bmp, TextureUsage usage);
        public abstract Texture CreateTexture(Image image, TextureUsage usage);        
        public abstract Texture CreateTexture(ImageLoader image, TextureUsage usage);
        public abstract Texture CreateTexture(int width, int height, int levelCount, TextureUsage usage, PixelFormat format);

        //public abstract Texture CreateTexture(Surface[] surfaces);

        /// <summary>
        ///  创建一个纹理，如果depth>1，则创建体积纹理
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        /// <param name="levelCount"></param>
        /// <param name="usage"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract Texture CreateTexture(int width, int height, int depth, int levelCount, TextureUsage usage, PixelFormat format);
        public abstract Texture CreateTexture(int length, int levelCount, TextureUsage usage, PixelFormat format);


        public abstract RenderTarget CreateRenderTarget(int width, int height, PixelFormat clrFmt, DepthFormat depthFmt);
        public abstract RenderTarget CreateRenderTarget(Surface backBuffer, Surface depthBuffer, int width, int height);

        //public abstract Sprite CreateSprite();

        public abstract IndexBuffer CreateIndexBuffer(IndexBufferType type, int count, BufferUsage usage, bool useSysMem);
        public abstract VertexBuffer CreateVertexBuffer(int vertexCount, VertexDeclaration vtxDecl, BufferUsage usage, bool useSysMem);

        public VertexBuffer CreateVertexBuffer(int vertexCount, VertexDeclaration vtxDecl, BufferUsage usage)
        {
            return CreateVertexBuffer(vertexCount, vtxDecl, usage, false);
        }
        public IndexBuffer CreateIndexBuffer(IndexBufferType type, int count, BufferUsage usage)
        {
            return CreateIndexBuffer(type, count, usage, false);
        }

        //public abstract Line CreateLine();
        //public abstract Font CreateFont(System.Drawing.Font font);

        public abstract VertexDeclaration CreateVertexDeclaration(VertexElement[] elements);

        public abstract StateBlock CreateStateBlock(StateBlockType type);

        public abstract VertexShader CreateVertexShader(string code, Macro[] defines, Include include, string profile, string functionName);
        public abstract PixelShader CreatePixelShader(string code, Macro[] defines, Include include, string profile, string functionName);

    }

}
