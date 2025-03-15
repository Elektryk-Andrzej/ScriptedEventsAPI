using ScriptedEventsAPI.ActionAPI.ActionResponses;

namespace ScriptedEventsAPI.ActionAPI.Actions.BaseActions;

public abstract class StandardAction : BaseAction
{
    public abstract IActionResponse Execute();
}