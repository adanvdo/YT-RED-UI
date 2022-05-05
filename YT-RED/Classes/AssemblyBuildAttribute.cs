using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace YT_RED.Classes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyBuildAttribute : Attribute
    {
        public string Value { get; set; }
        public AssemblyBuildAttribute() : this("") { }
        public AssemblyBuildAttribute(string value) { Value = value; }
    }
}
