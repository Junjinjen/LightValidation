using System;

namespace LightValidation.EntityFramework;

[AttributeUsage(AttributeTargets.Class)]
public sealed class DatabaseRuleAttribute : Attribute
{
}
