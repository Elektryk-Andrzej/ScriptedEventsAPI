using ScriptedEventsAPI.ActionAPI.ActionArguments;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;

namespace ScriptedEventsAPI.ActionAPI.Actions.BaseActions;

public abstract class BaseAction
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract BaseActionArgument[] RequiredArguments { get; }
    public ProvidedArguments ArgsProvided { get; } = new();
}