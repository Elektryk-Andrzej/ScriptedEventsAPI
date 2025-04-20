namespace ScriptedEventsAPI.MethodAPI.BaseMethods;

/// <summary>
///     Represents a standard method that returns a text value
/// </summary>
public abstract class TextReturningStandardMethod : StandardMethod
{
    public string? TextReturn { get; protected set; }
    public abstract string ReturnDescription { get; }
    public override string Description => "if you see this, the developer has forgor to account for this";
}