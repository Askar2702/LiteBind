using System;

namespace LiteBindDI
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class LiteInjectAttribute : Attribute { }
}
