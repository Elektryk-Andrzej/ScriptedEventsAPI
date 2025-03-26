using ScriptedEventsAPI.ActionAPI.ActionResponses;

namespace ScriptedEventsAPI.ActionAPI.BaseActions;

public abstract class StandardAction : BaseAction
{
    public abstract IActionResponse Execute();
}