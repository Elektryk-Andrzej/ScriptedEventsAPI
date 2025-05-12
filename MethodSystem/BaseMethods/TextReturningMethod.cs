namespace SER.MethodSystem.BaseMethods;

/// <summary>
/// Represents a standard method that returns a text value.
/// </summary>
public abstract class TextReturningMethod : Method
{
    public string? TextReturn { get; protected set; }
}