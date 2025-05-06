namespace ScriptedEventsAPI.MethodSystem.BaseMethods;

/// <summary>
/// Represents a standard method that returns a text value.
/// </summary>
public abstract class TextReturningStandardMethod : StandardMethod
{
    public string? TextReturn { get; protected set; }
}