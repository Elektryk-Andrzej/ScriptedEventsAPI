using System.Collections.Generic;

namespace ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;

public abstract class TreeContext : YieldingContext
{
    public List<BaseContext> Children = [];
}