using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Media;

namespace VirtualBicycle.RenderSystem.D3D9
{
    internal sealed class D3D9ObjectFactory : ObjectFactory
    {
        D3D9RenderSystem d3dRenderSys;

        public D3D9ObjectFactory(D3D9RenderSystem rs)
            : base(rs)
        {
            d3dRenderSys = rs;

        }

        public override Texture CreateReferenceTexture(Texture texture)
        {
            return new D3D9Texture(d3dRenderSys, (D3D9Texture)texture);
        }
        public override Texture CreateTexture(System.Drawing.Bitmap bmp, TextureUsage usage)
        {
            return new D3D9Texture(d3dRenderSys, bmp, usage);
        }
        public override Texture CreateTexture(int width, int height, int levelCount, TextureUsage usage, PixelFormat format)
        {
            return new D3D9Texture(d3dRenderSys, width, height, levelCount, usage, format);
        }
        public override Texture CreateTexture(ImageLoader image, TextureUsage usage)
        {
            return new D3D9Texture(d3dRenderSys, image, usage);
        }
        public override Texture CreateTexture(Image image, TextureUsage usage)
        {
            return new D3D9Texture(d3dRenderSys, image, usage);
        }
        public override Texture CreateTexture(int width, int height, int depth, int levelCount, TextureUsage usage, PixelFormat format)
        {
            return new D3D9Texture(d3dRenderSys, width, height, depth, levelCount, usage, format);
        }
        public override Texture CreateTexture(int length, int levelCount, TextureUsage usage, PixelFormat format)
        {
            return new D3D9Texture(d3dRenderSys, length, levelCount, usage, format);
        }
        //public override Texture CreateTexture(Surface[] surfaces)
        //{
        //    return new D3D9Texture(d3dRenderSys, (D3D9Surface)surfaces);
        //}
        //public override Sprite CreateSprite()
        //{
        //    return new D3D9Sprite(d3dRenderSys);
        //}

        public override IndexBuffer CreateIndexBuffer(IndexBufferType type, int count, BufferUsage usage, bool useSysMem)
        {
            return new D3D9IndexBuffer(d3dRenderSys, type, count, usage, useSysMem);
        }

        public override VertexBuffer CreateVertexBuffer(int vertexCount, VertexDeclaration vtxDecl, BufferUsage usage, bool useSysMem)
        {
            return new D3D9VertexBuffer(d3dRenderSys, vertexCount, vtxDecl.GetVertexSize(), usage, useSysMem);
        }

        //public override Line CreateLine()
        //{
        //    throw new NotImplementedException();
        //}

        //public override Font CreateFont(System.Drawing.Font font)
        //{
        //    throw new NotImplementedException();
        //}

        public override VertexDeclaration CreateVertexDeclaration(VertexElement[] elements)
        {
            return new D3D9VertexDeclaration(d3dRenderSys, elements);
        }

        public override StateBlock CreateStateBlock(StateBlockType type)
        {
            return new D3D9StateBlock(d3dRenderSys, type);
        } 

        public override PixelShader CreatePixelShader(string code, Macro[] defines, Include include,
            string profile, string functionName)
        {
            return new D3D9PixelShader(d3dRenderSys, code, defines, include, functionName, profile);
        }
        public override VertexShader CreateVertexShader(string code, Macro[] defines, Include include,
            string profile, string functionName)
        {
            return new D3D9VertexShader(d3dRenderSys, code, defines, include, functionName, profile);
        }


        public override RenderTarget CreateRenderTarget(int width, int height, PixelFormat clrFmt, PixelFormat depthFmt)
        {
            return new D3D9RenderTarget(d3dRenderSys, width, height, clrFmt, depthFmt);
        }
        public override RenderTarget CreateRenderTarget(Surface backBuffer, Surface depthBuffer, int width, int height)
        {
            D3D9Surface d3dcb = (D3D9Surface)backBuffer;
            D3D9Surface d3ddb = (D3D9Surface)depthBuffer;

            return new D3D9RenderTarget(d3dRenderSys, d3dcb.D3DSurface, d3ddb.D3DSurface, width, height, backBuffer.ColorFormat, depthBuffer.DepthFormat);
        }
    }
}
