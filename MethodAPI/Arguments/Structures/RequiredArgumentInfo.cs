namespace ScriptedEventsAPI.MethodAPI.Arguments.Structures;

public struct RequiredArgumentInfo
{
    public required bool IsRequired { get; init; }
    public required object? Value { get; init; }
    
    public static RequiredArgumentInfo Required()
    {
        return new()
        {
            IsRequired = true,
            Value = null
        };
    }
    
    public static RequiredArgumentInfo NotRequired(object defaultValue)
    {
        return new()
        {
            IsRequired = false,
            Value = defaultValue
        };
    }
}