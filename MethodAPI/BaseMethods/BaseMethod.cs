using ScriptedEventsAPI.MethodAPI.Arguments;
using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.ScriptAPI;

namespace ScriptedEventsAPI.MethodAPI.BaseMethods;

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
        Args = new(this);
    }

    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract BaseMethodArgument[] ExpectedArguments { get; }
    public ProvidedArguments Args { get; }
    public Script Script { get; set; } = null!;
}