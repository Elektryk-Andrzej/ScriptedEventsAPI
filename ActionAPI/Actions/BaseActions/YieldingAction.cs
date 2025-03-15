using System.Collections.Generic;

namespace ScriptedEventsAPI.ActionAPI.Actions.BaseActions;

public abstract class YieldingAction : BaseAction
{
    public abstract IEnumerator<float> Execute();
}