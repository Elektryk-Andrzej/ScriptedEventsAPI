using ScriptedEventsAPI.ActionAPI.ActionArguments;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;
using ScriptedEventsAPI.ActionAPI.ActionResponses;

namespace ScriptedEventsAPI.ActionAPI.Actions;

public abstract class BaseAction
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract BaseActionArgument[] RequiredArguments { get; }
    public abstract IActionResponse Execute();
    public Arguments ArgumentsProvided = [];
}