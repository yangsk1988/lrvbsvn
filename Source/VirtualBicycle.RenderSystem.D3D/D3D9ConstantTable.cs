using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Media;
using VirtualBicycle.MathLib;
using D3D = SlimDX.Direct3D9;

namespace VirtualBicycle.Graphics.D3D9
{
    internal unsafe class D3D9ConstantTable : ConstantTable
    {
        D3D.ConstantTable constTable;
        D3D.Device device;

        D3D9RenderSystem d3drs;

        Dictionary<string, D3D.EffectHandle> handleBuffer;

        internal D3D9ConstantTable(D3D9RenderSystem rs, D3D.ConstantTable ct)
            : base(rs)
        {
            d3drs = rs;
            device = rs.D3DDevice;
            constTable = ct;

            handleBuffer = new Dictionary<string, D3D.EffectHandle>();
        }


        public D3D.EffectHandle GetHandle(string name)
        {
            D3D.EffectHandle result;
            if (!handleBuffer.TryGetValue(name, out result))
            {
                result = new D3D.EffectHandle(name);
                handleBuffer.Add(name, result);
            }
            return result;
        }



        public override void SetValue(string paramName, Color4F color)
        {
            constTable.SetValue(device, GetHandle(paramName), *(SlimDX.Color4*)&color);
        }

        public override void SetValue(string paramName, Color4F[] color)
        {
            fixed (Color4F* ptr = &color[0])
            {
                constTable.SetVectorArray(device, GetHandle(paramName), new IntPtr(ptr), color.Length);
            }
        }

        public override void SetValue(string paramName, Matrix matrix)
        {
            constTable.SetValue(device, GetHandle(paramName), *(SlimDX.Matrix*)&matrix);
        }

        public override void SetValue(string paramName, Matrix[] matrix)
        {
            fixed (Matrix* ptr = &matrix[0])
            {
                constTable.SetMatrixArray(device, GetHandle(paramName), new IntPtr(ptr), matrix.Length);
            }
        }

        public override void SetValue(string paramName, float value)
        {
            constTable.SetValue(device, GetHandle(paramName), value);
        }

        public override void SetValue(string paramName, float[] value)
        {
            constTable.SetValue(device, GetHandle(paramName), value);
        }


        public override void SetValue(string paramName, Vector4 value)
        {
            constTable.SetValue(device, GetHandle(paramName), *(SlimDX.Vector4*)&value);
        }

        public override void SetValue(string paramName, Vector4[] value)
        {
            fixed (Vector4* ptr = &value[0])
            {
                constTable.SetVectorArray(device, GetHandle(paramName), new IntPtr(ptr), value.Length);
            }
        }

        public override void SetValue(string paramName, bool value)
        {
            constTable.SetValue(device, GetHandle(paramName), value);
        }

        public override void SetValue(string paramName, bool[] value)
        {
            constTable.SetValue(device, GetHandle(paramName), value);
        }

        public override void SetValue(string paramName, int value)
        {
            constTable.SetValue(device, GetHandle(paramName), value);
        }

        public override void SetValue(string paramName, int[] value)
        {
            constTable.SetValue(device, GetHandle(paramName), value);
        }
    }
}
