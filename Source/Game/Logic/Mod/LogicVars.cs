using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle.Logic.Mod
{
    public static class LogicVars<T>
    {
        static Dictionary<string, T> varTable = new Dictionary<string, T>();

        public static void Define(string key, T value)
        {
            varTable.Add(key, value);
        }
        public static void SetValue(string key, T value) 
        {
            varTable[key] = value;
        }

        public static bool IsDefined(string key)
        {
            return varTable.ContainsKey(key);
        }

        public static T GetValue(string key)
        {
            return varTable[key];
        }
    }
}
