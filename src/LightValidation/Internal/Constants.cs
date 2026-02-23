using System.Collections.Generic;
using System.Linq.Expressions;

namespace LightValidation.Internal;

internal static class Constants
{
    public static readonly ParameterExpression ConstantsParameter = Expression.Parameter(typeof(IReadOnlyList<object?>));

    public const string ArgumentsDelimiter = ", ";

    public const char PropertyDelimiter = '.';

    public const char OpeningIndexChar = '[';

    public const char ClosingIndexChar = ']';

    public const char OpeningArgumentsChar = '(';

    public const char ClosingArgumentsChar = ')';
}
