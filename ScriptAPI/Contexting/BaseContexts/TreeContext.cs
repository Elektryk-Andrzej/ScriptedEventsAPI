using System.Collections.Generic;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.ScriptAPI.Contexting.Structures;

namespace ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;

public abstract class TreeContext : YieldingContext
{
    public readonly List<BaseContext> Children = [];

    public void SendControlMessage(ParentContextControlMessage msg)
    {
        Logger.Debug($"{Name} context has received control message: {msg}");
        OnReceivedControlMessage(msg);
    }

    protected abstract void OnReceivedControlMessage(ParentContextControlMessage msg);
}