using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Structures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ScriptAPI.Contexting.Contexts.Loops;

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