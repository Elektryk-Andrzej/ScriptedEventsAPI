using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.BaseContexts;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.Structures;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;

namespace ScriptedEventsAPI.ScriptSystem.ContextSystem.Contexts.Loops;

public class LoopContinueContext : StandardContext
{
    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        return TryAddTokenRes.Error("The continue keyword does not expect arguments after it.");
    }

    public override Result VerifyCurrentState()
    {
        return true;
    }

    public override void Execute()
    {
        ParentContext?.SendControlMessage(ParentContextControlMessage.ForLoopContinue);
    }
}