using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections;

namespace VirtualBicycle.Physics.MathLib.Design
{
    internal abstract class MemberPropertyDescriptor : PropertyDescriptor
    {
        // Fields
        private MemberInfo _member;

        // Methods
        public MemberPropertyDescriptor(MemberInfo member)
            : base(member.Name, (Attribute[])member.GetCustomAttributes(typeof(Attribute), true))
        {
            this._member = member;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            MemberPropertyDescriptor descriptor = obj as MemberPropertyDescriptor;
            return ((descriptor != null) && descriptor._member.Equals(this._member));
        }

        public override int GetHashCode()
        {
            return this._member.GetHashCode();
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        // Properties
        public override Type ComponentType
        {
            get
            {
                return this._member.DeclaringType;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
    }

    internal class FieldPropertyDescriptor : MemberPropertyDescriptor
    {
        // Fields
        private FieldInfo _field;

        // Methods
        public FieldPropertyDescriptor(FieldInfo field)
            : base(field)
        {
            this._field = field;
        }

        public override object GetValue(object component)
        {
            return this._field.GetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            this._field.SetValue(component, value);
            this.OnValueChanged(component, EventArgs.Empty);
        }

        // Properties
        public override Type PropertyType
        {
            get
            {
                return this._field.FieldType;
            }
        }
    }



    public class MathTypeConverter : ExpandableObjectConverter
    {
        // Fields
        protected PropertyDescriptorCollection propertyDescriptions;
        protected bool supportStringConvert = true;

        // Methods
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((this.supportStringConvert && (sourceType == typeof(string))) || base.CanConvertFrom(context, sourceType));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));
        }

        internal static string ConvertFromValues<T>(ITypeDescriptorContext context, CultureInfo culture, T[] values)
        {
            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }
            string separator = culture.TextInfo.ListSeparator + " ";
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            string[] strArray = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                strArray[i] = converter.ConvertToString(context, culture, values[i]);
            }
            return string.Join(separator, strArray);
        }

        internal static T[] ConvertToValues<T>(ITypeDescriptorContext context, CultureInfo culture, object value, int arrayCount, params string[] expectedParams)
        {
            string str = value as string;
            if (str == null)
            {
                return null;
            }
            str = str.Trim();
            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }
            string[] strArray = str.Split(new string[] { culture.TextInfo.ListSeparator }, StringSplitOptions.None);
            T[] localArray = new T[strArray.Length];
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            for (int i = 0; i < localArray.Length; i++)
            {
                try
                {
                    localArray[i] = (T)converter.ConvertFromString(context, culture, strArray[i]);
                }
                catch (Exception exception)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "FrameworkResources.InvalidStringFormat", new object[] { string.Join(culture.TextInfo.ListSeparator, expectedParams) }), exception);
                }
            }
            if (localArray.Length != arrayCount)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "FrameworkResources.InvalidStringFormat", new object[] { string.Join(culture.TextInfo.ListSeparator, expectedParams) }));
            }
            return localArray;
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return this.propertyDescriptions;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }


    public class Vector2Converter : MathTypeConverter
    {
        // Methods
        public Vector2Converter()
        {
            Type type = typeof(Vector2);
            base.propertyDescriptions = new PropertyDescriptorCollection(new PropertyDescriptor[] { new FieldPropertyDescriptor(type.GetField("X")), new FieldPropertyDescriptor(type.GetField("Y")) }).Sort(new string[] { "X", "Y" });
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            float[] numArray = MathTypeConverter.ConvertToValues<float>(context, culture, value, 2, new string[] { "X", "Y" });
            if (numArray != null)
            {
                return new Vector2(numArray[0], numArray[1]);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if ((destinationType == typeof(string)) && (value is Vector2))
            {
                Vector2 vector2 = (Vector2)value;
                return MathTypeConverter.ConvertFromValues<float>(context, culture, new float[] { vector2.X, vector2.Y });
            }
            if ((destinationType == typeof(InstanceDescriptor)) && (value is Vector2))
            {
                Vector2 vector = (Vector2)value;
                ConstructorInfo constructor = typeof(Vector2).GetConstructor(new Type[] { typeof(float), typeof(float) });
                if (constructor != null)
                {
                    return new InstanceDescriptor(constructor, new object[] { vector.X, vector.Y });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues == null)
            {
                throw new ArgumentNullException("propertyValues", "FrameworkResources.NullNotAllowed");
            }
            return new Vector2((float)propertyValues["X"], (float)propertyValues["Y"]);
        }
    }

    public class Vector3Converter : MathTypeConverter
    {
        // Methods
        public Vector3Converter()
        {
            Type type = typeof(Vector3);
            base.propertyDescriptions = new PropertyDescriptorCollection(new PropertyDescriptor[] { new FieldPropertyDescriptor(type.GetField("X")), new FieldPropertyDescriptor(type.GetField("Y")), new FieldPropertyDescriptor(type.GetField("Z")) }).Sort(new string[] { "X", "Y", "Z" });
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            float[] numArray = MathTypeConverter.ConvertToValues<float>(context, culture, value, 3, new string[] { "X", "Y", "Z" });
            if (numArray != null)
            {
                return new Vector3(numArray[0], numArray[1], numArray[2]);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if ((destinationType == typeof(string)) && (value is Vector3))
            {
                Vector3 vector2 = (Vector3)value;
                return MathTypeConverter.ConvertFromValues<float>(context, culture, new float[] { vector2.X, vector2.Y, vector2.Z });
            }
            if ((destinationType == typeof(InstanceDescriptor)) && (value is Vector3))
            {
                Vector3 vector = (Vector3)value;
                ConstructorInfo constructor = typeof(Vector3).GetConstructor(new Type[] { typeof(float), typeof(float), typeof(float) });
                if (constructor != null)
                {
                    return new InstanceDescriptor(constructor, new object[] { vector.X, vector.Y, vector.Z });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues == null)
            {
                throw new ArgumentNullException("propertyValues", "FrameworkResources.NullNotAllowed");
            }
            return new Vector3((float)propertyValues["X"], (float)propertyValues["Y"], (float)propertyValues["Z"]);
        }
    }

    public class Vector4Converter : MathTypeConverter
    {
        // Methods
        public Vector4Converter()
        {
            Type type = typeof(Vector4);
            base.propertyDescriptions = new PropertyDescriptorCollection(new PropertyDescriptor[] { new FieldPropertyDescriptor(type.GetField("X")), new FieldPropertyDescriptor(type.GetField("Y")), new FieldPropertyDescriptor(type.GetField("Z")), new FieldPropertyDescriptor(type.GetField("W")) }).Sort(new string[] { "X", "Y", "Z", "W" });
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            float[] numArray = MathTypeConverter.ConvertToValues<float>(context, culture, value, 4, new string[] { "X", "Y", "Z", "W" });
            if (numArray != null)
            {
                return new Vector4(numArray[0], numArray[1], numArray[2], numArray[3]);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if ((destinationType == typeof(string)) && (value is Vector4))
            {
                Vector4 vector2 = (Vector4)value;
                return MathTypeConverter.ConvertFromValues<float>(context, culture, new float[] { vector2.X, vector2.Y, vector2.Z, vector2.W });
            }
            if ((destinationType == typeof(InstanceDescriptor)) && (value is Vector4))
            {
                Vector4 vector = (Vector4)value;
                ConstructorInfo constructor = typeof(Vector4).GetConstructor(new Type[] { typeof(float), typeof(float), typeof(float), typeof(float) });
                if (constructor != null)
                {
                    return new InstanceDescriptor(constructor, new object[] { vector.X, vector.Y, vector.Z, vector.W });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues == null)
            {
                throw new ArgumentNullException("propertyValues", "FrameworkResources.NullNotAllowed");
            }
            return new Vector4((float)propertyValues["X"], (float)propertyValues["Y"], (float)propertyValues["Z"], (float)propertyValues["W"]);
        }
    }

    public class MatrixConverter : MathTypeConverter
    {
        // Methods
        public MatrixConverter()
        {
            Type componentType = typeof(Matrix);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(componentType);
            PropertyDescriptorCollection descriptors = new PropertyDescriptorCollection(new PropertyDescriptor[] { 
            properties.Find("Translation", true), new FieldPropertyDescriptor(componentType.GetField("M11")), new FieldPropertyDescriptor(componentType.GetField("M12")), new FieldPropertyDescriptor(componentType.GetField("M13")), new FieldPropertyDescriptor(componentType.GetField("M14")), new FieldPropertyDescriptor(componentType.GetField("M21")), new FieldPropertyDescriptor(componentType.GetField("M22")), new FieldPropertyDescriptor(componentType.GetField("M23")), new FieldPropertyDescriptor(componentType.GetField("M24")), new FieldPropertyDescriptor(componentType.GetField("M31")), new FieldPropertyDescriptor(componentType.GetField("M32")), new FieldPropertyDescriptor(componentType.GetField("M33")), new FieldPropertyDescriptor(componentType.GetField("M34")), new FieldPropertyDescriptor(componentType.GetField("M41")), new FieldPropertyDescriptor(componentType.GetField("M42")), new FieldPropertyDescriptor(componentType.GetField("M43")), 
            new FieldPropertyDescriptor(componentType.GetField("M44"))
         });
            base.propertyDescriptions = descriptors;
            base.supportStringConvert = false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if ((destinationType == typeof(InstanceDescriptor)) && (value is Matrix))
            {
                Matrix matrix = (Matrix)value;
                ConstructorInfo constructor = typeof(Matrix).GetConstructor(new Type[] { typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float) });
                if (constructor != null)
                {
                    return new InstanceDescriptor(constructor, new object[] { matrix.M11, matrix.M12, matrix.M13, matrix.M14, matrix.M21, matrix.M22, matrix.M23, matrix.M24, matrix.M31, matrix.M32, matrix.M33, matrix.M34, matrix.M41, matrix.M42, matrix.M43, matrix.M44 });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues == null)
            {
                throw new ArgumentNullException("propertyValues", "FrameworkResources.NullNotAllowed");
            }
            return new Matrix((float)propertyValues["M11"], (float)propertyValues["M12"], (float)propertyValues["M13"], (float)propertyValues["M14"], (float)propertyValues["M21"], (float)propertyValues["M22"], (float)propertyValues["M23"], (float)propertyValues["M24"], (float)propertyValues["M31"], (float)propertyValues["M32"], (float)propertyValues["M33"], (float)propertyValues["M34"], (float)propertyValues["M41"], (float)propertyValues["M42"], (float)propertyValues["M43"], (float)propertyValues["M44"]);
        }
    }

    public class QuaternionConverter : MathTypeConverter
    {
        // Methods
        public QuaternionConverter()
        {
            Type type = typeof(Quaternion);
            base.propertyDescriptions = new PropertyDescriptorCollection(new PropertyDescriptor[] { new FieldPropertyDescriptor(type.GetField("X")), new FieldPropertyDescriptor(type.GetField("Y")), new FieldPropertyDescriptor(type.GetField("Z")), new FieldPropertyDescriptor(type.GetField("W")) }).Sort(new string[] { "X", "Y", "Z", "W" });
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            float[] numArray = MathTypeConverter.ConvertToValues<float>(context, culture, value, 4, new string[] { "X", "Y", "Z", "W" });
            if (numArray != null)
            {
                return new Quaternion(numArray[0], numArray[1], numArray[2], numArray[3]);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if ((destinationType == typeof(string)) && (value is Quaternion))
            {
                Quaternion quaternion2 = (Quaternion)value;
                return MathTypeConverter.ConvertFromValues<float>(context, culture, new float[] { quaternion2.X, quaternion2.Y, quaternion2.Z, quaternion2.W });
            }
            if ((destinationType == typeof(InstanceDescriptor)) && (value is Quaternion))
            {
                Quaternion quaternion = (Quaternion)value;
                ConstructorInfo constructor = typeof(Quaternion).GetConstructor(new Type[] { typeof(float), typeof(float), typeof(float), typeof(float) });
                if (constructor != null)
                {
                    return new InstanceDescriptor(constructor, new object[] { quaternion.X, quaternion.Y, quaternion.Z, quaternion.W });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues == null)
            {
                throw new ArgumentNullException("propertyValues", "FrameworkResources.NullNotAllowed");
            }
            return new Quaternion((float)propertyValues["X"], (float)propertyValues["Y"], (float)propertyValues["Z"], (float)propertyValues["W"]);
        }
    }

    public class BoundingSphereConverter : MathTypeConverter
    {
        // Methods
        public BoundingSphereConverter()
        {
            Type type = typeof(BoundingSphere);
            base.propertyDescriptions = new PropertyDescriptorCollection(new PropertyDescriptor[] { new FieldPropertyDescriptor(type.GetField("Center")), new FieldPropertyDescriptor(type.GetField("Radius")) }).Sort(new string[] { "Center", "Radius" });
            base.supportStringConvert = false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if ((destinationType == typeof(InstanceDescriptor)) && (value is BoundingSphere))
            {
                BoundingSphere sphere = (BoundingSphere)value;
                ConstructorInfo constructor = typeof(BoundingSphere).GetConstructor(new Type[] { typeof(Vector3), typeof(float) });
                if (constructor != null)
                {
                    return new InstanceDescriptor(constructor, new object[] { sphere.Center, sphere.Radius });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues == null)
            {
                throw new ArgumentNullException("propertyValues", "FrameworkResources.NullNotAllowed");
            }
            return new BoundingSphere((Vector3)propertyValues["Center"], (float)propertyValues["Radius"]);
        }
    }

    public class BoundingBoxConverter : MathTypeConverter
    {
        // Methods
        public BoundingBoxConverter()
        {
            Type type = typeof(BoundingBox);
            base.propertyDescriptions = new PropertyDescriptorCollection(new PropertyDescriptor[] { new FieldPropertyDescriptor(type.GetField("Min")), new FieldPropertyDescriptor(type.GetField("Max")) }).Sort(new string[] { "Min", "Max" });
            base.supportStringConvert = false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if ((destinationType == typeof(InstanceDescriptor)) && (value is BoundingBox))
            {
                BoundingBox box = (BoundingBox)value;
                ConstructorInfo constructor = typeof(BoundingBox).GetConstructor(new Type[] { typeof(Vector3), typeof(Vector3) });
                if (constructor != null)
                {
                    return new InstanceDescriptor(constructor, new object[] { box.Min, box.Max });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues == null)
            {
                throw new ArgumentNullException("propertyValues", "FrameworkResources.NullNotAllowed");
            }
            return new BoundingBox((Vector3)propertyValues["Min"], (Vector3)propertyValues["Max"]);
        }
    }
    public class RayConverter : MathTypeConverter
    {
        // Methods
        public RayConverter()
        {
            Type type = typeof(Ray);
            base.propertyDescriptions = new PropertyDescriptorCollection(new PropertyDescriptor[] { new FieldPropertyDescriptor(type.GetField("Position")), new FieldPropertyDescriptor(type.GetField("Direction")) }).Sort(new string[] { "Position", "Direction" });
            base.supportStringConvert = false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if ((destinationType == typeof(InstanceDescriptor)) && (value is Ray))
            {
                Ray ray = (Ray)value;
                ConstructorInfo constructor = typeof(Ray).GetConstructor(new Type[] { typeof(Vector3), typeof(Vector3) });
                if (constructor != null)
                {
                    return new InstanceDescriptor(constructor, new object[] { ray.Position, ray.Direction });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues == null)
            {
                throw new ArgumentNullException("propertyValues", "FrameworkResources.NullNotAllowed");
            }
            return new Ray((Vector3)propertyValues["Position"], (Vector3)propertyValues["Direction"]);
        }
    }


}