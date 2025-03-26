using System.Collections.Generic;

namespace ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;

public abstract class YieldingContext : BaseContext 
{
    public abstract IEnumerator<float> Execute();
}