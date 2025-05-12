using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.ContextSystem.Contexts.Control;

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