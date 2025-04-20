using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Structures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ScriptAPI.Contexting.Contexts.Control;

public class StopScriptContext(Script scr) : StandardContext
{
    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        return TryAddTokenRes.Error(
            "`stop` keyword is not expecting any arguments after it.");
    }

    public override Result VerifyCurrentState()
    {
        return true;
    }

    public override void Execute()
    {
        Terminate(scr);
    }
}