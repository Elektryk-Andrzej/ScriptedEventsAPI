using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.BaseContexts;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.Structures;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;

namespace ScriptedEventsAPI.ScriptSystem.ContextSystem.Contexts;

public class NoOperationContext : StandardContext
{
    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        return TryAddTokenRes.Continue();
    }

    public override Result VerifyCurrentState()
    {
        return true;
    }

    public override void Execute()
    {
    }
}