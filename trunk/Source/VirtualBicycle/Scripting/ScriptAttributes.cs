using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle.Scripting
{
    /// <summary>
    /// 指示一个事件可以被自动绑定上脚本
    /// </summary>
    [AttributeUsage(AttributeTargets.Event, Inherited = true, AllowMultiple = false)]
    internal sealed class ScriptEventAttribute : Attribute
    {
        public string EventDelegateName
        {
            get;
            private set;
        }

        public ScriptEventAttribute(string eventDelegateFieldName)
        {
            EventDelegateName = eventDelegateFieldName;
        }
    }


    [Flags()]
    public enum AssemblyProtectionFlags
    {
        None = 0,
        /// <summary>
        /// 混淆
        /// </summary>
        Obfucase = 1 << 0,
        /// <summary>
        /// 非对称加密
        /// </summary>
        Encrypt = 1 << 2
    }


    /// <summary>
    /// 标记脚本程序集是否受到保护
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public sealed class ProtectedAssemblyAttribute : Attribute
    {
        AssemblyProtectionFlags protectionFlags;

        public ProtectedAssemblyAttribute()
        {

        }

        public bool IsObfucased
        {
            get { return (int)(protectionFlags & AssemblyProtectionFlags.Obfucase) != 0; }
            set
            {
                if (value)
                {
                    protectionFlags |= AssemblyProtectionFlags.Obfucase;
                }
                else
                {
                    protectionFlags ^= AssemblyProtectionFlags.Obfucase;
                }
            }
        }
        public bool IsEncrypted
        {
            get { return (int)(protectionFlags & AssemblyProtectionFlags.Encrypt) != 0; }
            set
            {
                if (value)
                {
                    protectionFlags |= AssemblyProtectionFlags.Encrypt;
                }
                else
                {
                    protectionFlags ^= AssemblyProtectionFlags.Encrypt;
                }
            }
        }

        public AssemblyProtectionFlags Flags
        {
            get { return protectionFlags; }
            set { protectionFlags = value; }
        }
    }

    public enum ScriptBinding
    {
        Auto,
        Manual
    }

    /// <summary>
    /// 设置脚本程序集中的方法 的脚本绑定规则
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ScriptBindingAttribute : Attribute
    {
        ScriptBinding binding;

        // This is a positional argument
        public ScriptBindingAttribute(ScriptBinding flag)
        {
            binding = flag;
        }
        //public ScriptBindingAttribute() { }
        public virtual ScriptBinding BindMode
        {
            get { return binding; }
            //set { binding = value; }
        }
    }
}
