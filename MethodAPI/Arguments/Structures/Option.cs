namespace ScriptedEventsAPI.MethodAPI.Arguments.Structures;

public record Option(string Value, string Description = "")
{
    public static implicit operator Option(string value) => new(value);
}