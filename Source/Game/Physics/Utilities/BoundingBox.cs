using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using VirtualBicycle.Physics.MathLib.Design;

namespace VirtualBicycle.Physics.MathLib
{
    public enum ContainmentType
    {
        Disjoint,
        Contains,
        Intersects
    }

    [Serializable, StructLayout(LayoutKind.Sequential), TypeConverter(typeof(BoundingSphereConverter))]
    public struct BoundingSphere : IEquatable<BoundingSphere>
    {
        public Vector3 Center;
        public float Radius;
        public BoundingSphere(Vector3 center, float radius)
        {
            if (radius < 0f)
            {
                throw new ArgumentException("FrameworkResources.NegativeRadius");
            }
            this.Center = center;
            this.Radius = radius;
        }

        public bool Equals(BoundingSphere other)
        {
            return ((this.Center == other.Center) && (this.Radius == other.Radius));
        }

        public override bool Equals(object obj)
        {
            bool flag = false;
            if (obj is BoundingSphere)
            {
                flag = this.Equals((BoundingSphere)obj);
            }
            return flag;
        }

        public override int GetHashCode()
        {
            return (this.Center.GetHashCode() + this.Radius.GetHashCode());
        }

        public override string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            return string.Format(currentCulture, "{{Center:{0} Radius:{1}}}", new object[] { this.Center.ToString(), this.Radius.ToString(currentCulture) });
        }

        public static BoundingSphere CreateMerged(BoundingSphere original, BoundingSphere additional)
        {
            BoundingSphere sphere;
            Vector3 vector2;
            Vector3.Subtract(ref additional.Center, ref original.Center, out vector2);
            float num = vector2.Length();
            float radius = original.Radius;
            float num2 = additional.Radius;
            if ((radius + num2) >= num)
            {
                if ((radius - num2) >= num)
                {
                    return original;
                }
                if ((num2 - radius) >= num)
                {
                    return additional;
                }
            }
            Vector3 vector = (Vector3)(vector2 * (1f / num));
            float num5 = MathHelper.Min(-radius, num - num2);
            float num4 = (MathHelper.Max(radius, num + num2) - num5) * 0.5f;
            sphere.Center = original.Center + ((Vector3)(vector * (num4 + num5)));
            sphere.Radius = num4;
            return sphere;
        }

        public static void CreateMerged(ref BoundingSphere original, ref BoundingSphere additional, out BoundingSphere result)
        {
            Vector3 vector2;
            Vector3.Subtract(ref additional.Center, ref original.Center, out vector2);
            float num = vector2.Length();
            float radius = original.Radius;
            float num2 = additional.Radius;
            if ((radius + num2) >= num)
            {
                if ((radius - num2) >= num)
                {
                    result = original;
                    return;
                }
                if ((num2 - radius) >= num)
                {
                    result = additional;
                    return;
                }
            }
            Vector3 vector = (Vector3)(vector2 * (1f / num));
            float num5 = MathHelper.Min(-radius, num - num2);
            float num4 = (MathHelper.Max(radius, num + num2) - num5) * 0.5f;
            result.Center = original.Center + ((Vector3)(vector * (num4 + num5)));
            result.Radius = num4;
        }

        public static BoundingSphere CreateFromBoundingBox(BoundingBox box)
        {
            float num;
            BoundingSphere sphere;
            Vector3.Lerp(ref box.Min, ref box.Max, 0.5f, out sphere.Center);
            Vector3.Distance(ref box.Min, ref box.Max, out num);
            sphere.Radius = num * 0.5f;
            return sphere;
        }

        public static void CreateFromBoundingBox(ref BoundingBox box, out BoundingSphere result)
        {
            float num;
            Vector3.Lerp(ref box.Min, ref box.Max, 0.5f, out result.Center);
            Vector3.Distance(ref box.Min, ref box.Max, out num);
            result.Radius = num * 0.5f;
        }

        public static BoundingSphere CreateFromPoints(IEnumerable<Vector3> points)
        {
            float num;
            float num2;
            Vector3 vector2;
            float num4;
            float num5;
            BoundingSphere sphere;
            Vector3 vector5;
            Vector3 vector6;
            Vector3 vector7;
            Vector3 vector8;
            Vector3 vector9;
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            IEnumerator<Vector3> enumerator = points.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw new ArgumentException("FrameworkResources.BoundingSphereZeroPoints");
            }
            Vector3 vector4 = vector5 = vector6 = vector7 = vector8 = vector9 = enumerator.Current;
            foreach (Vector3 vector in points)
            {
                if (vector.X < vector4.X)
                {
                    vector4 = vector;
                }
                if (vector.X > vector5.X)
                {
                    vector5 = vector;
                }
                if (vector.Y < vector6.Y)
                {
                    vector6 = vector;
                }
                if (vector.Y > vector7.Y)
                {
                    vector7 = vector;
                }
                if (vector.Z < vector8.Z)
                {
                    vector8 = vector;
                }
                if (vector.Z > vector9.Z)
                {
                    vector9 = vector;
                }
            }
            Vector3.Distance(ref vector5, ref vector4, out num5);
            Vector3.Distance(ref vector7, ref vector6, out num4);
            Vector3.Distance(ref vector9, ref vector8, out num2);
            if (num5 > num4)
            {
                if (num5 > num2)
                {
                    Vector3.Lerp(ref vector5, ref vector4, 0.5f, out vector2);
                    num = num5 * 0.5f;
                }
                else
                {
                    Vector3.Lerp(ref vector9, ref vector8, 0.5f, out vector2);
                    num = num2 * 0.5f;
                }
            }
            else if (num4 > num2)
            {
                Vector3.Lerp(ref vector7, ref vector6, 0.5f, out vector2);
                num = num4 * 0.5f;
            }
            else
            {
                Vector3.Lerp(ref vector9, ref vector8, 0.5f, out vector2);
                num = num2 * 0.5f;
            }
            foreach (Vector3 vector10 in points)
            {
                Vector3 vector3;
                vector3.X = vector10.X - vector2.X;
                vector3.Y = vector10.Y - vector2.Y;
                vector3.Z = vector10.Z - vector2.Z;
                float num3 = vector3.Length();
                if (num3 > num)
                {
                    num = (num + num3) * 0.5f;
                    vector2 += (Vector3)((1f - (num / num3)) * vector3);
                }
            }
            sphere.Center = vector2;
            sphere.Radius = num;
            return sphere;
        }

        //public static BoundingSphere CreateFromFrustum(BoundingFrustum frustum)
        //{
        //    if (frustum == null)
        //    {
        //        throw new ArgumentNullException("frustum");
        //    }
        //    return CreateFromPoints(frustum.cornerArray);
        //}

        public bool Intersects(BoundingBox box)
        {
            float num;
            Vector3 vector;
            Vector3.Clamp(ref this.Center, ref box.Min, ref box.Max, out vector);
            Vector3.DistanceSquared(ref this.Center, ref vector, out num);
            return (num <= (this.Radius * this.Radius));
        }

        public void Intersects(ref BoundingBox box, out bool result)
        {
            float num;
            Vector3 vector;
            Vector3.Clamp(ref this.Center, ref box.Min, ref box.Max, out vector);
            Vector3.DistanceSquared(ref this.Center, ref vector, out num);
            result = num <= (this.Radius * this.Radius);
        }

        //public bool Intersects(BoundingFrustum frustum)
        //{
        //    bool flag;
        //    if (null == frustum)
        //    {
        //        throw new ArgumentNullException("frustum", "FrameworkResources.NullNotAllowed");
        //    }
        //    frustum.Intersects(ref this, out flag);
        //    return flag;
        //}

        public PlaneIntersectionType Intersects(Plane plane)
        {
            return plane.Intersects(this);
        }

        public void Intersects(ref Plane plane, out PlaneIntersectionType result)
        {
            plane.Intersects(ref this, out result);
        }

        public float? Intersects(Ray ray)
        {
            return ray.Intersects(this);
        }

        public void Intersects(ref Ray ray, out float? result)
        {
            ray.Intersects(ref this, out result);
        }

        public bool Intersects(BoundingSphere sphere)
        {
            float num3;
            Vector3.DistanceSquared(ref this.Center, ref sphere.Center, out num3);
            float radius = this.Radius;
            float num = sphere.Radius;
            if ((((radius * radius) + ((2f * radius) * num)) + (num * num)) <= num3)
            {
                return false;
            }
            return true;
        }

        public void Intersects(ref BoundingSphere sphere, out bool result)
        {
            float num3;
            Vector3.DistanceSquared(ref this.Center, ref sphere.Center, out num3);
            float radius = this.Radius;
            float num = sphere.Radius;
            result = (((radius * radius) + ((2f * radius) * num)) + (num * num)) > num3;
        }

        public ContainmentType Contains(BoundingBox box)
        {
            Vector3 vector;
            if (!box.Intersects(this))
            {
                return ContainmentType.Disjoint;
            }
            float num = this.Radius * this.Radius;
            vector.X = this.Center.X - box.Min.X;
            vector.Y = this.Center.Y - box.Max.Y;
            vector.Z = this.Center.Z - box.Max.Z;
            if (vector.LengthSquared() > num)
            {
                return ContainmentType.Intersects;
            }
            vector.X = this.Center.X - box.Max.X;
            vector.Y = this.Center.Y - box.Max.Y;
            vector.Z = this.Center.Z - box.Max.Z;
            if (vector.LengthSquared() > num)
            {
                return ContainmentType.Intersects;
            }
            vector.X = this.Center.X - box.Max.X;
            vector.Y = this.Center.Y - box.Min.Y;
            vector.Z = this.Center.Z - box.Max.Z;
            if (vector.LengthSquared() > num)
            {
                return ContainmentType.Intersects;
            }
            vector.X = this.Center.X - box.Min.X;
            vector.Y = this.Center.Y - box.Min.Y;
            vector.Z = this.Center.Z - box.Max.Z;
            if (vector.LengthSquared() > num)
            {
                return ContainmentType.Intersects;
            }
            vector.X = this.Center.X - box.Min.X;
            vector.Y = this.Center.Y - box.Max.Y;
            vector.Z = this.Center.Z - box.Min.Z;
            if (vector.LengthSquared() > num)
            {
                return ContainmentType.Intersects;
            }
            vector.X = this.Center.X - box.Max.X;
            vector.Y = this.Center.Y - box.Max.Y;
            vector.Z = this.Center.Z - box.Min.Z;
            if (vector.LengthSquared() > num)
            {
                return ContainmentType.Intersects;
            }
            vector.X = this.Center.X - box.Max.X;
            vector.Y = this.Center.Y - box.Min.Y;
            vector.Z = this.Center.Z - box.Min.Z;
            if (vector.LengthSquared() > num)
            {
                return ContainmentType.Intersects;
            }
            vector.X = this.Center.X - box.Min.X;
            vector.Y = this.Center.Y - box.Min.Y;
            vector.Z = this.Center.Z - box.Min.Z;
            if (vector.LengthSquared() > num)
            {
                return ContainmentType.Intersects;
            }
            return ContainmentType.Contains;
        }

        public void Contains(ref BoundingBox box, out ContainmentType result)
        {
            bool flag;
            box.Intersects(ref this, out flag);
            if (!flag)
            {
                result = ContainmentType.Disjoint;
            }
            else
            {
                Vector3 vector;
                float num = this.Radius * this.Radius;
                result = ContainmentType.Intersects;
                vector.X = this.Center.X - box.Min.X;
                vector.Y = this.Center.Y - box.Max.Y;
                vector.Z = this.Center.Z - box.Max.Z;
                if (vector.LengthSquared() <= num)
                {
                    vector.X = this.Center.X - box.Max.X;
                    vector.Y = this.Center.Y - box.Max.Y;
                    vector.Z = this.Center.Z - box.Max.Z;
                    if (vector.LengthSquared() <= num)
                    {
                        vector.X = this.Center.X - box.Max.X;
                        vector.Y = this.Center.Y - box.Min.Y;
                        vector.Z = this.Center.Z - box.Max.Z;
                        if (vector.LengthSquared() <= num)
                        {
                            vector.X = this.Center.X - box.Min.X;
                            vector.Y = this.Center.Y - box.Min.Y;
                            vector.Z = this.Center.Z - box.Max.Z;
                            if (vector.LengthSquared() <= num)
                            {
                                vector.X = this.Center.X - box.Min.X;
                                vector.Y = this.Center.Y - box.Max.Y;
                                vector.Z = this.Center.Z - box.Min.Z;
                                if (vector.LengthSquared() <= num)
                                {
                                    vector.X = this.Center.X - box.Max.X;
                                    vector.Y = this.Center.Y - box.Max.Y;
                                    vector.Z = this.Center.Z - box.Min.Z;
                                    if (vector.LengthSquared() <= num)
                                    {
                                        vector.X = this.Center.X - box.Max.X;
                                        vector.Y = this.Center.Y - box.Min.Y;
                                        vector.Z = this.Center.Z - box.Min.Z;
                                        if (vector.LengthSquared() <= num)
                                        {
                                            vector.X = this.Center.X - box.Min.X;
                                            vector.Y = this.Center.Y - box.Min.Y;
                                            vector.Z = this.Center.Z - box.Min.Z;
                                            if (vector.LengthSquared() <= num)
                                            {
                                                result = ContainmentType.Contains;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //public ContainmentType Contains(BoundingFrustum frustum)
        //{
        //    if (null == frustum)
        //    {
        //        throw new ArgumentNullException("frustum", FrameworkResources.NullNotAllowed);
        //    }
        //    if (!frustum.Intersects(this))
        //    {
        //        return ContainmentType.Disjoint;
        //    }
        //    float num2 = this.Radius * this.Radius;
        //    foreach (Vector3 vector2 in frustum.cornerArray)
        //    {
        //        Vector3 vector;
        //        vector.X = vector2.X - this.Center.X;
        //        vector.Y = vector2.Y - this.Center.Y;
        //        vector.Z = vector2.Z - this.Center.Z;
        //        if (vector.LengthSquared() > num2)
        //        {
        //            return ContainmentType.Intersects;
        //        }
        //    }
        //    return ContainmentType.Contains;
        //}

        public ContainmentType Contains(Vector3 point)
        {
            if (Vector3.DistanceSquared(point, this.Center) >= (this.Radius * this.Radius))
            {
                return ContainmentType.Disjoint;
            }
            return ContainmentType.Contains;
        }

        public void Contains(ref Vector3 point, out ContainmentType result)
        {
            float num;
            Vector3.DistanceSquared(ref point, ref this.Center, out num);
            result = (num < (this.Radius * this.Radius)) ? ContainmentType.Contains : ContainmentType.Disjoint;
        }

        public ContainmentType Contains(BoundingSphere sphere)
        {
            float num3;
            Vector3.Distance(ref this.Center, ref sphere.Center, out num3);
            float radius = this.Radius;
            float num = sphere.Radius;
            if ((radius + num) < num3)
            {
                return ContainmentType.Disjoint;
            }
            if ((radius - num) < num3)
            {
                return ContainmentType.Intersects;
            }
            return ContainmentType.Contains;
        }

        public void Contains(ref BoundingSphere sphere, out ContainmentType result)
        {
            float num3;
            Vector3.Distance(ref this.Center, ref sphere.Center, out num3);
            float radius = this.Radius;
            float num = sphere.Radius;
            result = ((radius + num) >= num3) ? (((radius - num) >= num3) ? ContainmentType.Contains : ContainmentType.Intersects) : ContainmentType.Disjoint;
        }

        internal void SupportMapping(ref Vector3 v, out Vector3 result)
        {
            float num2 = v.Length();
            float num = this.Radius / num2;
            result.X = this.Center.X + (v.X * num);
            result.Y = this.Center.Y + (v.Y * num);
            result.Z = this.Center.Z + (v.Z * num);
        }

        public BoundingSphere Transform(Matrix matrix)
        {
            BoundingSphere sphere = new BoundingSphere();
            sphere.Center = Vector3.Transform(this.Center, matrix);
            float num = ((matrix.M11 + matrix.M22) + matrix.M33) / 3f;
            sphere.Radius = this.Radius * num;
            return sphere;
        }

        public void Transform(ref Matrix matrix, out BoundingSphere result)
        {
            result.Center = Vector3.Transform(this.Center, matrix);
            float num = ((matrix.M11 + matrix.M22) + matrix.M33) / 3f;
            result.Radius = this.Radius * num;
        }

        public static bool operator ==(BoundingSphere a, BoundingSphere b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(BoundingSphere a, BoundingSphere b)
        {
            if (!(a.Center != b.Center))
            {
                return (a.Radius != b.Radius);
            }
            return true;
        }
        public static implicit operator SlimDX.BoundingSphere(BoundingSphere v)
        {
            return new SlimDX.BoundingSphere(v.Center, v.Radius);
        }
        public static implicit operator BoundingSphere(SlimDX.BoundingSphere v)
        {
            return new BoundingSphere(v.Center, v.Radius);
        }
    }


    [Serializable, StructLayout(LayoutKind.Sequential), TypeConverter(typeof(BoundingBoxConverter))]
    public struct BoundingBox : IEquatable<BoundingBox>
    {
        public const int CornerCount = 8;
        public Vector3 Min;
        public Vector3 Max;
        public Vector3[] GetCorners()
        {
            return new Vector3[] { new Vector3(this.Min.X, this.Max.Y, this.Max.Z), new Vector3(this.Max.X, this.Max.Y, this.Max.Z), new Vector3(this.Max.X, this.Min.Y, this.Max.Z), new Vector3(this.Min.X, this.Min.Y, this.Max.Z), new Vector3(this.Min.X, this.Max.Y, this.Min.Z), new Vector3(this.Max.X, this.Max.Y, this.Min.Z), new Vector3(this.Max.X, this.Min.Y, this.Min.Z), new Vector3(this.Min.X, this.Min.Y, this.Min.Z) };
        }

        public void GetCorners(Vector3[] corners)
        {
            if (corners == null)
            {
                throw new ArgumentNullException("corners");
            }
            if (corners.Length < 8)
            {
                throw new ArgumentOutOfRangeException("corners", "FrameworkResources.NotEnoughCorners");
            }
            corners[0].X = this.Min.X;
            corners[0].Y = this.Max.Y;
            corners[0].Z = this.Max.Z;
            corners[1].X = this.Max.X;
            corners[1].Y = this.Max.Y;
            corners[1].Z = this.Max.Z;
            corners[2].X = this.Max.X;
            corners[2].Y = this.Min.Y;
            corners[2].Z = this.Max.Z;
            corners[3].X = this.Min.X;
            corners[3].Y = this.Min.Y;
            corners[3].Z = this.Max.Z;
            corners[4].X = this.Min.X;
            corners[4].Y = this.Max.Y;
            corners[4].Z = this.Min.Z;
            corners[5].X = this.Max.X;
            corners[5].Y = this.Max.Y;
            corners[5].Z = this.Min.Z;
            corners[6].X = this.Max.X;
            corners[6].Y = this.Min.Y;
            corners[6].Z = this.Min.Z;
            corners[7].X = this.Min.X;
            corners[7].Y = this.Min.Y;
            corners[7].Z = this.Min.Z;
        }

        public BoundingBox(Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
        }

        public bool Equals(BoundingBox other)
        {
            return ((this.Min == other.Min) && (this.Max == other.Max));
        }

        public override bool Equals(object obj)
        {
            bool flag = false;
            if (obj is BoundingBox)
            {
                flag = this.Equals((BoundingBox)obj);
            }
            return flag;
        }

        public override int GetHashCode()
        {
            return (this.Min.GetHashCode() + this.Max.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{Min:{0} Max:{1}}}", new object[] { this.Min.ToString(), this.Max.ToString() });
        }

        public static BoundingBox CreateMerged(BoundingBox original, BoundingBox additional)
        {
            BoundingBox box;
            Vector3.Min(ref original.Min, ref additional.Min, out box.Min);
            Vector3.Max(ref original.Max, ref additional.Max, out box.Max);
            return box;
        }

        public static void CreateMerged(ref BoundingBox original, ref BoundingBox additional, out BoundingBox result)
        {
            Vector3 vector;
            Vector3 vector2;
            Vector3.Min(ref original.Min, ref additional.Min, out vector2);
            Vector3.Max(ref original.Max, ref additional.Max, out vector);
            result.Min = vector2;
            result.Max = vector;
        }

        public static BoundingBox CreateFromSphere(BoundingSphere sphere)
        {
            BoundingBox box;
            box.Min.X = sphere.Center.X - sphere.Radius;
            box.Min.Y = sphere.Center.Y - sphere.Radius;
            box.Min.Z = sphere.Center.Z - sphere.Radius;
            box.Max.X = sphere.Center.X + sphere.Radius;
            box.Max.Y = sphere.Center.Y + sphere.Radius;
            box.Max.Z = sphere.Center.Z + sphere.Radius;
            return box;
        }

        public static void CreateFromSphere(ref BoundingSphere sphere, out BoundingBox result)
        {
            result.Min.X = sphere.Center.X - sphere.Radius;
            result.Min.Y = sphere.Center.Y - sphere.Radius;
            result.Min.Z = sphere.Center.Z - sphere.Radius;
            result.Max.X = sphere.Center.X + sphere.Radius;
            result.Max.Y = sphere.Center.Y + sphere.Radius;
            result.Max.Z = sphere.Center.Z + sphere.Radius;
        }

        public static BoundingBox CreateFromPoints(IEnumerable<Vector3> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException();
            }
            bool flag = false;
            Vector3 vector3 = new Vector3(float.MaxValue);
            Vector3 vector2 = new Vector3(float.MinValue);
            foreach (Vector3 vector in points)
            {
                Vector3 vector4 = vector;
                Vector3.Min(ref vector3, ref vector4, out vector3);
                Vector3.Max(ref vector2, ref vector4, out vector2);
                flag = true;
            }
            if (!flag)
            {
                throw new ArgumentException("FrameworkResources.BoundingBoxZeroPoints");
            }
            return new BoundingBox(vector3, vector2);
        }

        public bool Intersects(BoundingBox box)
        {
            if ((this.Max.X < box.Min.X) || (this.Min.X > box.Max.X))
            {
                return false;
            }
            if ((this.Max.Y < box.Min.Y) || (this.Min.Y > box.Max.Y))
            {
                return false;
            }
            return ((this.Max.Z >= box.Min.Z) && (this.Min.Z <= box.Max.Z));
        }

        public void Intersects(ref BoundingBox box, out bool result)
        {
            result = false;
            if ((((this.Max.X >= box.Min.X) && (this.Min.X <= box.Max.X)) && ((this.Max.Y >= box.Min.Y) && (this.Min.Y <= box.Max.Y))) && ((this.Max.Z >= box.Min.Z) && (this.Min.Z <= box.Max.Z)))
            {
                result = true;
            }
        }

        //public bool Intersects(BoundingFrustum frustum)
        //{
        //    if (null == frustum)
        //    {
        //        throw new ArgumentNullException("frustum", FrameworkResources.NullNotAllowed);
        //    }
        //    return frustum.Intersects(this);
        //}

        public PlaneIntersectionType Intersects(Plane plane)
        {
            Vector3 vector;
            Vector3 vector2;
            vector2.X = (plane.Normal.X >= 0f) ? this.Min.X : this.Max.X;
            vector2.Y = (plane.Normal.Y >= 0f) ? this.Min.Y : this.Max.Y;
            vector2.Z = (plane.Normal.Z >= 0f) ? this.Min.Z : this.Max.Z;
            vector.X = (plane.Normal.X >= 0f) ? this.Max.X : this.Min.X;
            vector.Y = (plane.Normal.Y >= 0f) ? this.Max.Y : this.Min.Y;
            vector.Z = (plane.Normal.Z >= 0f) ? this.Max.Z : this.Min.Z;
            float num = ((plane.Normal.X * vector2.X) + (plane.Normal.Y * vector2.Y)) + (plane.Normal.Z * vector2.Z);
            if ((num + plane.D) > 0f)
            {
                return PlaneIntersectionType.Front;
            }
            num = ((plane.Normal.X * vector.X) + (plane.Normal.Y * vector.Y)) + (plane.Normal.Z * vector.Z);
            if ((num + plane.D) < 0f)
            {
                return PlaneIntersectionType.Back;
            }
            return PlaneIntersectionType.Intersecting;
        }

        public void Intersects(ref Plane plane, out PlaneIntersectionType result)
        {
            Vector3 vector;
            Vector3 vector2;
            vector2.X = (plane.Normal.X >= 0f) ? this.Min.X : this.Max.X;
            vector2.Y = (plane.Normal.Y >= 0f) ? this.Min.Y : this.Max.Y;
            vector2.Z = (plane.Normal.Z >= 0f) ? this.Min.Z : this.Max.Z;
            vector.X = (plane.Normal.X >= 0f) ? this.Max.X : this.Min.X;
            vector.Y = (plane.Normal.Y >= 0f) ? this.Max.Y : this.Min.Y;
            vector.Z = (plane.Normal.Z >= 0f) ? this.Max.Z : this.Min.Z;
            float num = ((plane.Normal.X * vector2.X) + (plane.Normal.Y * vector2.Y)) + (plane.Normal.Z * vector2.Z);
            if ((num + plane.D) > 0f)
            {
                result = PlaneIntersectionType.Front;
            }
            else
            {
                num = ((plane.Normal.X * vector.X) + (plane.Normal.Y * vector.Y)) + (plane.Normal.Z * vector.Z);
                if ((num + plane.D) < 0f)
                {
                    result = PlaneIntersectionType.Back;
                }
                else
                {
                    result = PlaneIntersectionType.Intersecting;
                }
            }
        }

        public float? Intersects(Ray ray)
        {
            float num = 0f;
            float maxValue = float.MaxValue;
            if (Math.Abs(ray.Direction.X) < 1E-06f)
            {
                if ((ray.Position.X < this.Min.X) || (ray.Position.X > this.Max.X))
                {
                    return null;
                }
            }
            else
            {
                float num11 = 1f / ray.Direction.X;
                float num8 = (this.Min.X - ray.Position.X) * num11;
                float num7 = (this.Max.X - ray.Position.X) * num11;
                if (num8 > num7)
                {
                    float num14 = num8;
                    num8 = num7;
                    num7 = num14;
                }
                num = MathHelper.Max(num8, num);
                maxValue = MathHelper.Min(num7, maxValue);
                if (num > maxValue)
                {
                    return null;
                }
            }
            if (Math.Abs(ray.Direction.Y) < 1E-06f)
            {
                if ((ray.Position.Y < this.Min.Y) || (ray.Position.Y > this.Max.Y))
                {
                    return null;
                }
            }
            else
            {
                float num10 = 1f / ray.Direction.Y;
                float num6 = (this.Min.Y - ray.Position.Y) * num10;
                float num5 = (this.Max.Y - ray.Position.Y) * num10;
                if (num6 > num5)
                {
                    float num13 = num6;
                    num6 = num5;
                    num5 = num13;
                }
                num = MathHelper.Max(num6, num);
                maxValue = MathHelper.Min(num5, maxValue);
                if (num > maxValue)
                {
                    return null;
                }
            }
            if (Math.Abs(ray.Direction.Z) < 1E-06f)
            {
                if ((ray.Position.Z < this.Min.Z) || (ray.Position.Z > this.Max.Z))
                {
                    return null;
                }
            }
            else
            {
                float num9 = 1f / ray.Direction.Z;
                float num4 = (this.Min.Z - ray.Position.Z) * num9;
                float num3 = (this.Max.Z - ray.Position.Z) * num9;
                if (num4 > num3)
                {
                    float num12 = num4;
                    num4 = num3;
                    num3 = num12;
                }
                num = MathHelper.Max(num4, num);
                maxValue = MathHelper.Min(num3, maxValue);
                if (num > maxValue)
                {
                    return null;
                }
            }
            return new float?(num);
        }

        public void Intersects(ref Ray ray, out float? result)
        {
            result = 0;
            float num = 0f;
            float maxValue = float.MaxValue;
            if (Math.Abs(ray.Direction.X) < 1E-06f)
            {
                if ((ray.Position.X < this.Min.X) || (ray.Position.X > this.Max.X))
                {
                    return;
                }
            }
            else
            {
                float num11 = 1f / ray.Direction.X;
                float num8 = (this.Min.X - ray.Position.X) * num11;
                float num7 = (this.Max.X - ray.Position.X) * num11;
                if (num8 > num7)
                {
                    float num14 = num8;
                    num8 = num7;
                    num7 = num14;
                }
                num = MathHelper.Max(num8, num);
                maxValue = MathHelper.Min(num7, maxValue);
                if (num > maxValue)
                {
                    return;
                }
            }
            if (Math.Abs(ray.Direction.Y) < 1E-06f)
            {
                if ((ray.Position.Y < this.Min.Y) || (ray.Position.Y > this.Max.Y))
                {
                    return;
                }
            }
            else
            {
                float num10 = 1f / ray.Direction.Y;
                float num6 = (this.Min.Y - ray.Position.Y) * num10;
                float num5 = (this.Max.Y - ray.Position.Y) * num10;
                if (num6 > num5)
                {
                    float num13 = num6;
                    num6 = num5;
                    num5 = num13;
                }
                num = MathHelper.Max(num6, num);
                maxValue = MathHelper.Min(num5, maxValue);
                if (num > maxValue)
                {
                    return;
                }
            }
            if (Math.Abs(ray.Direction.Z) < 1E-06f)
            {
                if ((ray.Position.Z < this.Min.Z) || (ray.Position.Z > this.Max.Z))
                {
                    return;
                }
            }
            else
            {
                float num9 = 1f / ray.Direction.Z;
                float num4 = (this.Min.Z - ray.Position.Z) * num9;
                float num3 = (this.Max.Z - ray.Position.Z) * num9;
                if (num4 > num3)
                {
                    float num12 = num4;
                    num4 = num3;
                    num3 = num12;
                }
                num = MathHelper.Max(num4, num);
                maxValue = MathHelper.Min(num3, maxValue);
                if (num > maxValue)
                {
                    return;
                }
            }
            result = new float?(num);
        }

        public bool Intersects(BoundingSphere sphere)
        {
            float num;
            Vector3 vector;
            Vector3.Clamp(ref sphere.Center, ref this.Min, ref this.Max, out vector);
            Vector3.DistanceSquared(ref sphere.Center, ref vector, out num);
            return (num <= (sphere.Radius * sphere.Radius));
        }

        public void Intersects(ref BoundingSphere sphere, out bool result)
        {
            float num;
            Vector3 vector;
            Vector3.Clamp(ref sphere.Center, ref this.Min, ref this.Max, out vector);
            Vector3.DistanceSquared(ref sphere.Center, ref vector, out num);
            result = num <= (sphere.Radius * sphere.Radius);
        }

        public ContainmentType Contains(BoundingBox box)
        {
            if ((this.Max.X < box.Min.X) || (this.Min.X > box.Max.X))
            {
                return ContainmentType.Disjoint;
            }
            if ((this.Max.Y < box.Min.Y) || (this.Min.Y > box.Max.Y))
            {
                return ContainmentType.Disjoint;
            }
            if ((this.Max.Z < box.Min.Z) || (this.Min.Z > box.Max.Z))
            {
                return ContainmentType.Disjoint;
            }
            if ((((this.Min.X <= box.Min.X) && (box.Max.X <= this.Max.X)) && ((this.Min.Y <= box.Min.Y) && (box.Max.Y <= this.Max.Y))) && ((this.Min.Z <= box.Min.Z) && (box.Max.Z <= this.Max.Z)))
            {
                return ContainmentType.Contains;
            }
            return ContainmentType.Intersects;
        }

        public void Contains(ref BoundingBox box, out ContainmentType result)
        {
            result = ContainmentType.Disjoint;
            if ((((this.Max.X >= box.Min.X) && (this.Min.X <= box.Max.X)) && ((this.Max.Y >= box.Min.Y) && (this.Min.Y <= box.Max.Y))) && ((this.Max.Z >= box.Min.Z) && (this.Min.Z <= box.Max.Z)))
            {
                result = ((((this.Min.X <= box.Min.X) && (box.Max.X <= this.Max.X)) && ((this.Min.Y <= box.Min.Y) && (box.Max.Y <= this.Max.Y))) && ((this.Min.Z <= box.Min.Z) && (box.Max.Z <= this.Max.Z))) ? ContainmentType.Contains : ContainmentType.Intersects;
            }
        }

        //public ContainmentType Contains(BoundingFrustum frustum)
        //{
        //    if (null == frustum)
        //    {
        //        throw new ArgumentNullException("frustum", FrameworkResources.NullNotAllowed);
        //    }
        //    if (!frustum.Intersects(this))
        //    {
        //        return ContainmentType.Disjoint;
        //    }
        //    foreach (Vector3 vector in frustum.cornerArray)
        //    {
        //        if (this.Contains(vector) == ContainmentType.Disjoint)
        //        {
        //            return ContainmentType.Intersects;
        //        }
        //    }
        //    return ContainmentType.Contains;
        //}

        public ContainmentType Contains(Vector3 point)
        {
            if ((((this.Min.X <= point.X) && (point.X <= this.Max.X)) && ((this.Min.Y <= point.Y) && (point.Y <= this.Max.Y))) && ((this.Min.Z <= point.Z) && (point.Z <= this.Max.Z)))
            {
                return ContainmentType.Contains;
            }
            return ContainmentType.Disjoint;
        }

        public void Contains(ref Vector3 point, out ContainmentType result)
        {
            result = ((((this.Min.X <= point.X) && (point.X <= this.Max.X)) && ((this.Min.Y <= point.Y) && (point.Y <= this.Max.Y))) && ((this.Min.Z <= point.Z) && (point.Z <= this.Max.Z))) ? ContainmentType.Contains : ContainmentType.Disjoint;
        }

        public ContainmentType Contains(BoundingSphere sphere)
        {
            float num2;
            Vector3 vector;
            Vector3.Clamp(ref sphere.Center, ref this.Min, ref this.Max, out vector);
            Vector3.DistanceSquared(ref sphere.Center, ref vector, out num2);
            float radius = sphere.Radius;
            if (num2 > (radius * radius))
            {
                return ContainmentType.Disjoint;
            }
            if (((((this.Min.X + radius) <= sphere.Center.X) && (sphere.Center.X <= (this.Max.X - radius))) && (((this.Max.X - this.Min.X) > radius) && ((this.Min.Y + radius) <= sphere.Center.Y))) && (((sphere.Center.Y <= (this.Max.Y - radius)) && ((this.Max.Y - this.Min.Y) > radius)) && ((((this.Min.Z + radius) <= sphere.Center.Z) && (sphere.Center.Z <= (this.Max.Z - radius))) && ((this.Max.X - this.Min.X) > radius))))
            {
                return ContainmentType.Contains;
            }
            return ContainmentType.Intersects;
        }

        public void Contains(ref BoundingSphere sphere, out ContainmentType result)
        {
            float num2;
            Vector3 vector;
            Vector3.Clamp(ref sphere.Center, ref this.Min, ref this.Max, out vector);
            Vector3.DistanceSquared(ref sphere.Center, ref vector, out num2);
            float radius = sphere.Radius;
            if (num2 > (radius * radius))
            {
                result = ContainmentType.Disjoint;
            }
            else
            {
                result = (((((this.Min.X + radius) <= sphere.Center.X) && (sphere.Center.X <= (this.Max.X - radius))) && (((this.Max.X - this.Min.X) > radius) && ((this.Min.Y + radius) <= sphere.Center.Y))) && (((sphere.Center.Y <= (this.Max.Y - radius)) && ((this.Max.Y - this.Min.Y) > radius)) && ((((this.Min.Z + radius) <= sphere.Center.Z) && (sphere.Center.Z <= (this.Max.Z - radius))) && ((this.Max.X - this.Min.X) > radius)))) ? ContainmentType.Contains : ContainmentType.Intersects;
            }
        }

        internal void SupportMapping(ref Vector3 v, out Vector3 result)
        {
            result.X = (v.X >= 0f) ? this.Max.X : this.Min.X;
            result.Y = (v.Y >= 0f) ? this.Max.Y : this.Min.Y;
            result.Z = (v.Z >= 0f) ? this.Max.Z : this.Min.Z;
        }

        public static bool operator ==(BoundingBox a, BoundingBox b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(BoundingBox a, BoundingBox b)
        {
            if (!(a.Min != b.Min))
            {
                return (a.Max != b.Max);
            }
            return true;
        }
        public static implicit operator SlimDX.BoundingBox(BoundingBox v)
        {
            return new SlimDX.BoundingBox(v.Min, v.Max);
        }
        public static implicit operator BoundingBox(SlimDX.BoundingBox v)
        {
            return new BoundingBox(v.Minimum, v.Maximum);
        }
    }
}
