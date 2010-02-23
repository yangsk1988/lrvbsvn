using System;
using System.Collections.Generic;
using System.Text;

using D3D = SlimDX.Direct3D9;

namespace VirtualBicycle.Graphics.D3D9
{
    internal sealed class D3D9VertexDeclaration : VertexDeclaration
    {
        D3D.VertexDeclaration vtxDecl;


        public D3D9VertexDeclaration(D3D9RenderSystem rs, VertexElement[] elements)
            : base(elements)
        {
            int elemCount = elements.Length;
            D3D.VertexElement[] delements = new D3D.VertexElement[elemCount + 1];
            for (int i = 0; i < elemCount; i++)
            {
                delements[i] = new D3D.VertexElement(0, (short)elements[i].Offset,
                    D3D9Utils.ConvertEnum(elements[i].Type), D3D.DeclarationMethod.Default,
                    D3D9Utils.ConvertEnum(elements[i].Semantic), (byte)elements[i].Index);
            }
            delements[elemCount] = D3D.VertexElement.VertexDeclarationEnd;

            vtxDecl = new D3D.VertexDeclaration(rs.D3DDevice, delements);
        }

        internal D3D.VertexDeclaration D3DVtxDecl
        {
            get { return vtxDecl; }
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                vtxDecl.Dispose();
            }
            vtxDecl = null;
        }
    }
}
