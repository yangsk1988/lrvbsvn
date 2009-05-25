using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle.Scripting
{
    [Serializable]
    public class ScriptException : Exception
    {
        public ScriptException() : base("Script Error") { }
        public ScriptException(string message) : base(message) { }
    }

    [Serializable]
    public class InvalidScriptException : ScriptException
    {
        public InvalidScriptException() { }
        public InvalidScriptException(string message) : base(message) { }
    }

    public enum ScriptSecurityType
    {
        InvaildSecurityAttribute,
        ReflectionPermission,
        RegistryPermission,
        FileIOPermission,
        SecurityPermission,
        PinvokeImpl,
        Unmanaged,
        Native,
        InternalCall,
        ReferenceAssembly

    }

    [Serializable]
    public class ScriptSecurityException : ScriptException
    {

        public ScriptSecurityException(ScriptSecurityType sst, params object[] param)
        {
            switch (sst)
            {
                case ScriptSecurityType.FileIOPermission:
                    break;
                case ScriptSecurityType.ReflectionPermission:
                    break;
                case ScriptSecurityType.RegistryPermission:
                    break;
                case ScriptSecurityType.SecurityPermission:
                    break;
                case ScriptSecurityType.InvaildSecurityAttribute:
                    break;
                case ScriptSecurityType.InternalCall:
                    break;
                case ScriptSecurityType.Native:
                    break;
                case ScriptSecurityType.PinvokeImpl:
                    break;
                case ScriptSecurityType.Unmanaged:
                    break;
                case ScriptSecurityType.ReferenceAssembly:
                    break;
            }
        }
    }
    [Serializable]
    public class ScriptArgumentException : Exception
    {

        public ScriptArgumentException() { }
        public ScriptArgumentException(string message) : base(message) { }
        public ScriptArgumentException(string message, Exception inner) : base(message, inner) { }
        protected ScriptArgumentException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

}
