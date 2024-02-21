using LightValidation.Constants;
using Code = LightValidation.Constants.ErrorCode;

namespace LightValidation.Result;

public class NullEntityFailure : RuleFailure
{
    public static readonly NullEntityFailure Default = new()
    {
        PropertyName = RootProperty.Name,
        PropertyValue = null,
        ErrorCode = Code.EntityCannotBeNull,
        ErrorDescription = "Entity cannot be null",
    };
}
