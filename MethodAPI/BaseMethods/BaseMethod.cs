using ScriptedEventsAPI.MethodAPI.Arguments;
using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.Arguments.Structures;
using ScriptedEventsAPI.ScriptAPI;

namespace ScriptedEventsAPI.MethodAPI.BaseMethods;

public abstract class BaseMethod
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract BaseMethodArgument[] ExpectedArguments { get; }
    public ProvidedArguments Args { get; }
    public Script Script { get; set; } = null!;

    protected BaseMethod()
    {
        Args = new(this);
    }
}