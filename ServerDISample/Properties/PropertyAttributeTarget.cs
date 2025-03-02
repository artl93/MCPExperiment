using System;

namespace SampleMCPApp
{
    /// <summary>
    /// Custom attribute target flags to allow using parameter attributes on properties for sample purposes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PropertyAttributeTargetAttribute : Attribute
    {
    }
}