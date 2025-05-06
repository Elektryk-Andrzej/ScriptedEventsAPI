using System.Collections.Generic;

namespace ScriptedEventsAPI.ScriptSystem.ContextSystem.BaseContexts;

public abstract class YieldingContext : BaseContext
{
    public abstract IEnumerator<float> Execute();
}