using System;

namespace SER.MethodSystem.BaseMethods;

/// <summary>
/// Represents a method that returns a reference to an object that cannot be represented fully in text form (not counting players.)
/// </summary>
public abstract class ReferenceReturningMethod : Method
{
    public abstract Type ReturnType { get; }
    public object? ValueReturn { get; protected set; }
}