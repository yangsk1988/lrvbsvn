using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle.Core
{
#if XBOX
    /// <summary>
    /// .net CF不支持没有这个类型，无实际用途，为了让编译器通过用。减少使用Macro易维护。
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class AssemblyFileVersionAttribute : Attribute
    {
        string _version;

        public AssemblyFileVersionAttribute(string version)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }
            this._version = version;
        }

        public string Version
        {
            get { return _version; }
        }
    }

    /// <summary>
    /// .net CF不支持没有这个类型，无实际用途，为了让编译器通过用。减少使用Macro易维护。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MTAThreadAttribute : Attribute
    {
    }

    /// <summary>
    /// .net CF不支持没有这个类型，无实际用途，为了让编译器通过用。减少使用Macro易维护。
    /// </summary>
    [AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class DebuggerDisplayAttribute : Attribute
    {
        string name;
        Type target;
        string targetName;
        string type;
        string value;

        public DebuggerDisplayAttribute(string value)
        {
            if (value == null)
            {
                this.value = "";
            }
            else
            {
                this.value = value;
            }
            this.name = "";
            this.type = "";
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Type Target
        {
            get { return target; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.targetName = value.AssemblyQualifiedName;
                this.target = value;
            }
        }

        public string TargetTypeName
        {
            get { return targetName; }
            set { targetName = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Value
        {
            get { return value; }
        }
    }

    /// <summary>
    /// .net CF不支持没有这个类型，无实际用途，为了让编译器通过用。减少使用Macro易维护。
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class DebuggerTypeProxyAttribute : Attribute
    {
        Type target;
        string targetName;
        string typeName;

        public DebuggerTypeProxyAttribute(string typeName)
        {
            this.typeName = typeName;
        }

        public DebuggerTypeProxyAttribute(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            this.typeName = type.AssemblyQualifiedName;
        }

        public string ProxyTypeName
        {
            get { return typeName; }
        }

        public Type Target
        {
            get { return target; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.targetName = value.AssemblyQualifiedName;
                this.target = value;
            }
        }

        public string TargetTypeName
        {
            get { return targetName; }
            set { targetName = value; }
        }
    }

    /// <summary>
    /// .net CF不支持没有这个类型，无实际用途，为了让编译器通过用。减少使用Macro易维护。
    /// </summary>
    public enum DebuggerBrowsableState
    {
        Collapsed = 0x2,
        Never = 0x0,
        RootHidden = 0x3
    }

    /// <summary>
    /// .net CF不支持没有这个类型，无实际用途，为了让编译器通过用。减少使用Macro易维护。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DebuggerBrowsableAttribute : Attribute
    {
        DebuggerBrowsableState state;

        public DebuggerBrowsableAttribute(DebuggerBrowsableState state)
        {
            if ((state < DebuggerBrowsableState.Never) || (state > DebuggerBrowsableState.RootHidden))
            {
                throw new ArgumentOutOfRangeException("state");
            }
            this.state = state;
        }

        public DebuggerBrowsableState State
        {
            get { return state; }
        }
    }
#endif
}