using System.Collections.Generic;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.Structures;

namespace ScriptedEventsAPI.ScriptSystem.ContextSystem.BaseContexts;

public abstract class TreeContext : YieldingContext
{
    public readonly List<BaseContext> Children = [];
    protected bool IsTerminated { get; set; }

    protected override void Terminate(Script scr)
    {
        IsTerminated = true;
        base.Terminate(scr);
    }

    public void SendControlMessage(ParentContextControlMessage msg)
    {
        Logger.Debug($"{Name} context has received control message: {msg}");
        OnReceivedControlMessageFromChild(msg);
    }

    protected abstract void OnReceivedControlMessageFromChild(ParentContextControlMessage msg);
}