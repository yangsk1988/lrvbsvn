using System;
using System.Collections.Generic;
using System.Text;

using D3D = SlimDX.Direct3D9;
using System.IO;

namespace VirtualBicycle.Graphics.D3D9
{
    internal class D3D9VertexShader : VertexShader
    {
        class IncludeHelper : D3D.Include
        {
            Include include;

            public IncludeHelper(Include ourInc)
            {
                this.include = ourInc;
            }

            #region Include 成员

            public void Close(Stream stream)
            {
                include.Close(stream);
            }

            public void Open(D3D.IncludeType includeType, string fileName, out Stream stream)
            {
                include.Open(D3D9Utils.ConvertEnum(includeType), fileName, out stream);
            }

            #endregion
        }
    
        D3D.VertexShader vs;
        D3D.Device device;

        public D3D9VertexShader(D3D9RenderSystem rs, string code, Macro[] defines, Include include, string functionName, string profile)
            : base(rs)
        {
            this.device = rs.D3DDevice;

            D3D.Macro[] d3dDefines = new D3D.Macro[defines.Length];
            for (int i = 0; i < defines.Length; i++)
            {
                d3dDefines[i].Definition = defines[i].Defination;
                d3dDefines[i].Name = defines[i].Name;
            }

            D3D.ShaderBytecode byteCode = D3D.ShaderBytecode.Compile(code, d3dDefines, new IncludeHelper(include), 
                functionName, profile, D3D.ShaderFlags.OptimizationLevel3 | D3D.ShaderFlags.PackMatrixRowMajor);

            vs = new D3D.VertexShader(device, byteCode);

            ConstantTable = new D3D9ConstantTable(rs, byteCode.ConstantTable);
        }

        internal D3D.VertexShader D3DVS
        {
            get { return vs; }
        }
    }
}
