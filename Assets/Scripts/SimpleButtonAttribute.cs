using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Method)]
public class SimpleButtonAttribute : PropertyAttribute
{
    public Type[] ArgumentTypes { get; private set; }

    public SimpleButtonAttribute(params Type[] argumentTypes)
    {
        ArgumentTypes = argumentTypes;
    }
}