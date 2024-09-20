namespace LightValidation.Constants;

public static class ErrorCode
{
    public static readonly string EntityCannotBeNull = "entity_cannot_be_null";

    public static readonly string ValueCannotBeNullOrDefault = "value_cannot_be_null_or_default";

    public static readonly string ValueMustBeNullOrDefault = "value_must_be_null_or_default";

    public static readonly string ValueMustSatisfyPredicateCondition = "value_must_satisfy_predicate_condition";

    public static readonly string ValueMustBeDefinedInEnum = "value_must_be_defined_in_enum";

    public static readonly string ValueMustBeEqualToSpecifiedValue = "value_must_be_equal_to_specified_value";

    public static readonly string ValueCannotBeEqualToSpecifiedValue = "value_cannot_be_equal_to_specified_value";

    public static class String
    {
        public static readonly string MustBeValidEmail = "string_must_be_valid_email";

        public static readonly string LengthMustBeExact = "string_length_must_be_exact";

        public static readonly string LengthOutOfRange = "string_length_out_of_range";

        public static readonly string LengthTooLong = "string_length_too_long";

        public static readonly string LengthTooShort = "string_length_too_short";

        public static readonly string CannotBeNullOrEmpty = "string_cannot_be_null_or_empty";

        public static readonly string CannotBeNullOrWhiteSpace = "string_cannot_be_null_or_whitespace";

        public static readonly string MustBeNullOrEmpty = "string_must_be_null_or_empty";

        public static readonly string MustBeNullOrWhiteSpace = "string_must_be_null_or_whitespace";

        public static readonly string MustMatchRegexPattern = "string_must_match_regex_pattern";

        public static readonly string MustBeValidUri = "string_must_be_valid_uri";
    }

    public static class Collection
    {
        public static readonly string CountOutOfRange = "collection_count_out_of_range";

        public static readonly string CountMustBeExact = "collection_count_must_be_exact";

        public static readonly string CountTooMany = "collection_count_too_many";

        public static readonly string CountTooFew = "collection_count_too_few";

        public static readonly string CannotBeNullOrEmpty = "collection_cannot_be_null_or_empty";

        public static readonly string MustBeNullOrEmpty = "collection_must_be_null_or_empty";
    }

    public static class Number
    {
        public static readonly string MustBeBetweenExclusive = "number_must_be_between_exclusive";

        public static readonly string MustBeGreaterThanOrEqualToSpecifiedValue
            = "number_must_be_greater_than_or_equal_to_specified_value";

        public static readonly string MustBeGreaterThanSpecifiedValue = "number_must_be_greater_than_specified_value";

        public static readonly string MustBeBetweenInclusive = "number_must_be_between_inclusive";

        public static readonly string MustBeLessThanOrEqualToSpecifiedValue
            = "number_must_be_less_than_or_equal_to_specified_value";

        public static readonly string MustBeLessThanSpecifiedValue = "number_must_be_less_than_specified_value";
    }
}
