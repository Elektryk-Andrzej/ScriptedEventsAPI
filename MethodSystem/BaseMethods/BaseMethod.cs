using ScriptedEventsAPI.MethodSystem.ArgumentSystem;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.ScriptSystem;

namespace ScriptedEventsAPI.MethodSystem.BaseMethods;

/// <summary>
///     Represents a base method.
/// </summary>
/// <remarks>
///     Do NOT use this to define a SER method, as it has no Execute() method.
///     Use <see cref="StandardMethod" /> or <see cref="YieldingMethod" /> instead.
/// </remarks>
public abstract class BaseMethod
{
    protected BaseMethod()
    {
        Name = GetType().Name;
        Args = new(this);
    }

    public readonly string Name;
    public abstract string Description { get; }
    public abstract BaseMethodArgument[] ExpectedArguments { get; }
    public ProvidedArguments Args { get; }
    public Script Script { get; set; } = null!;
}