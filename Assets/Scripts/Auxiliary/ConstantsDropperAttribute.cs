using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public sealed class ConstantsDropperAttribute : PropertyAttribute
{
    public Type ConstantsType { get; }

    public ConstantsDropperAttribute(Type constantsType)
    {
        ConstantsType = constantsType;
    }
}