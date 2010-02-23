using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace VirtualBicycle.Design
{
#if XBOX
    /// <summary>
    /// .net CF不支持没有这个类型，无实际用途，为了让编译器通过用。减少使用Macro易维护。
    /// </summary>
    public class ExpandableObjectConverter : TypeConverter { }
   
    /// <summary>
    /// .net CF不支持没有这个类型，无实际用途，为了让编译器通过用。减少使用Macro易维护。
    /// </summary>
    public class UITypeEditor { }
   
    /// <summary>
    /// .net CF不支持没有这个类型，无实际用途，为了让编译器通过用。减少使用Macro易维护。
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class TypeConverterAttribute : Attribute
    {
        public static readonly TypeConverterAttribute Default = new TypeConverterAttribute();

        string typeName;

        public TypeConverterAttribute()
        {
            this.typeName = string.Empty;
        }

        public TypeConverterAttribute(string typeName)
        {
            typeName.ToUpper(CultureInfo.InvariantCulture);
            this.typeName = typeName;
        }

        public TypeConverterAttribute(Type type)
        {
            this.typeName = type.AssemblyQualifiedName;
        }

        public override bool Equals(object obj)
        {
            TypeConverterAttribute attribute = obj as TypeConverterAttribute;
            return ((attribute != null) && (attribute.ConverterTypeName == this.typeName));
        }

        public override int GetHashCode()
        {
            return this.typeName.GetHashCode();
        }

        public string ConverterTypeName
        {
            get { return typeName; }
        }
    }
    
    /// <summary>
    /// .net CF不支持没有这个类型，无实际用途，为了让编译器通过用。减少使用Macro易维护。
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class BrowsableAttribute : Attribute
    {
        public static readonly BrowsableAttribute Default = Yes;
        public static readonly BrowsableAttribute No = new BrowsableAttribute(false);
        public static readonly BrowsableAttribute Yes = new BrowsableAttribute(true);

        bool browsable = true;

        public BrowsableAttribute(bool browsable)
        {
            this.browsable = browsable;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            BrowsableAttribute attribute = obj as BrowsableAttribute;
            return ((attribute != null) && (attribute.Browsable == this.browsable));
        }

        public override int GetHashCode()
        {
            return this.browsable.GetHashCode();
        }

        public override bool IsDefaultAttribute()
        {
            return this.Equals(Default);
        }

        public bool Browsable
        {
            get { return browsable; }
        }

    }
#endif
}