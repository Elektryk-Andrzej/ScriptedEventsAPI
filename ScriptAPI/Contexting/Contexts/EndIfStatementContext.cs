using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Structures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ScriptAPI.Contexting.Contexts;

public class TerminationContext : StandardContext
{
    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        return TryAddTokenRes.Error($"There can't be anything else on the same line as the context termination line.");
    }

    public override Result VerifyCurrentState()
    {
        return true;
    }

    public override void Execute()
    {
    }
}