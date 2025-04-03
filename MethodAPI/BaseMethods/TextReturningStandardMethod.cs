namespace ScriptedEventsAPI.MethodAPI.BaseMethods;

public abstract class TextReturningStandardMethod : StandardMethod
{
    public string? TextReturn { get; protected set; }
    public abstract string ReturnDescription { get; }
}