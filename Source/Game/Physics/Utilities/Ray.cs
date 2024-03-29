﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using VirtualBicycle.Physics.MathLib.Design;
using System.Globalization;

namespace VirtualBicycle.Physics.MathLib
{
    [Serializable, StructLayout(LayoutKind.Sequential), TypeConverter(typeof(RayConverter))]
    public struct Ray : IEquatable<Ray>
    {
        public Vector3 Position;
        public Vector3 Direction;
        public Ray(Vector3 position, Vector3 direction)
        {
            this.Position = position;
            this.Direction = direction;
        }

        public bool Equals(Ray other)
        {
            return (((((this.Position.X == other.Position.X) && (this.Position.Y == other.Position.Y)) && ((this.Position.Z == other.Position.Z) && (this.Direction.X == other.Direction.X))) && (this.Direction.Y == other.Direction.Y)) && (this.Direction.Z == other.Direction.Z));
        }

        public override bool Equals(object obj)
        {
            bool flag = false;
            if ((obj != null) && (obj is Ray))
            {
                flag = this.Equals((Ray)obj);
            }
            return flag;
        }

        public override int GetHashCode()
        {
            return (this.Position.GetHashCode() + this.Direction.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{Position:{0} Direction:{1}}}", new object[] { this.Position.ToString(), this.Direction.ToString() });
        }

        public float? Intersects(BoundingBox box)
        {
            return box.Intersects(this);
        }

        public void Intersects(ref BoundingBox box, out float? result)
        {
            box.Intersects(ref this, out result);
        }


        public float? Intersects(Plane plane)
        {
            float num2 = ((plane.Normal.X * this.Direction.X) + (plane.Normal.Y * this.Direction.Y)) + (plane.Normal.Z * this.Direction.Z);
            if (Math.Abs(num2) < 1E-05f)
            {
                return null;
            }
            float num3 = ((plane.Normal.X * this.Position.X) + (plane.Normal.Y * this.Position.Y)) + (plane.Normal.Z * this.Position.Z);
            float num = (-plane.D - num3) / num2;
            if (num < 0f)
            {
                if (num < -1E-05f)
                {
                    return null;
                }
                num = 0f;
            }
            return new float?(num);
        }

        public void Intersects(ref Plane plane, out float? result)
        {
            float num2 = ((plane.Normal.X * this.Direction.X) + (plane.Normal.Y * this.Direction.Y)) + (plane.Normal.Z * this.Direction.Z);
            if (Math.Abs(num2) < 1E-05f)
            {
                result = 0;
            }
            else
            {
                float num3 = ((plane.Normal.X * this.Position.X) + (plane.Normal.Y * this.Position.Y)) + (plane.Normal.Z * this.Position.Z);
                float num = (-plane.D - num3) / num2;
                if (num < 0f)
                {
                    if (num < -1E-05f)
                    {
                        result = 0;
                        return;
                    }
                    result = 0f;
                }
                result = new float?(num);
            }
        }

        public float? Intersects(BoundingSphere sphere)
        {
            float num5 = sphere.Center.X - this.Position.X;
            float num4 = sphere.Center.Y - this.Position.Y;
            float num3 = sphere.Center.Z - this.Position.Z;
            float num7 = ((num5 * num5) + (num4 * num4)) + (num3 * num3);
            float num2 = sphere.Radius * sphere.Radius;
            if (num7 <= num2)
            {
                return 0f;
            }
            float num = ((num5 * this.Direction.X) + (num4 * this.Direction.Y)) + (num3 * this.Direction.Z);
            if (num < 0f)
            {
                return null;
            }
            float num6 = num7 - (num * num);
            if (num6 > num2)
            {
                return null;
            }
            float num8 = (float)Math.Sqrt((double)(num2 - num6));
            return new float?(num - num8);
        }

        public void Intersects(ref BoundingSphere sphere, out float? result)
        {
            float num5 = sphere.Center.X - this.Position.X;
            float num4 = sphere.Center.Y - this.Position.Y;
            float num3 = sphere.Center.Z - this.Position.Z;
            float num7 = ((num5 * num5) + (num4 * num4)) + (num3 * num3);
            float num2 = sphere.Radius * sphere.Radius;
            if (num7 <= num2)
            {
                result = 0f;
            }
            else
            {
                result = 0;
                float num = ((num5 * this.Direction.X) + (num4 * this.Direction.Y)) + (num3 * this.Direction.Z);
                if (num >= 0f)
                {
                    float num6 = num7 - (num * num);
                    if (num6 <= num2)
                    {
                        float num8 = (float)Math.Sqrt((double)(num2 - num6));
                        result = new float?(num - num8);
                    }
                }
            }
        }

        public static bool operator ==(Ray a, Ray b)
        {
            return (((((a.Position.X == b.Position.X) && (a.Position.Y == b.Position.Y)) && ((a.Position.Z == b.Position.Z) && (a.Direction.X == b.Direction.X))) && (a.Direction.Y == b.Direction.Y)) && (a.Direction.Z == b.Direction.Z));
        }

        public static bool operator !=(Ray a, Ray b)
        {
            if ((((a.Position.X == b.Position.X) && (a.Position.Y == b.Position.Y)) && ((a.Position.Z == b.Position.Z) && (a.Direction.X == b.Direction.X))) && (a.Direction.Y == b.Direction.Y))
            {
                return (a.Direction.Z != b.Direction.Z);
            }
            return true;
        }

        public static implicit operator SlimDX.Ray(Ray ray)
        {
            return new SlimDX.Ray(ray.Position, ray.Direction);
        }
        public static implicit operator Ray(SlimDX.Ray ray)
        {
            return new Ray(ray.Position, ray.Direction);
        }
    }


}
