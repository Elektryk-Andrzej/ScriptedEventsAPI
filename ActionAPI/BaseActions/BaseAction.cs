using ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;
using ScriptedEventsAPI.ActionAPI.ActionResponses;
using ScriptedEventsAPI.ScriptAPI;

namespace ScriptedEventsAPI.ActionAPI.BaseActions;

public abstract class BaseAction
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract BaseActionArgument[] ExpectedArguments { get; }
    public ProvidedArguments Args { get; }
    public Script Script { get; set; } = null!;
    public IActionResponse Response { get; protected set; } = new SuccessResponse();

    protected BaseAction()
    {
        Args = new(this);
    }
}