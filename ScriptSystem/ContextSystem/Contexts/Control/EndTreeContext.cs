using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.ContextSystem.Contexts.Control;

public class TerminationContext : StandardContext
{
    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        return TryAddTokenRes.Error("There can't be anything else on the same line as the context termination line.");
    }

    public override Result VerifyCurrentState()
    {
        return true;
    }

    public override void Execute()
    {
    }
}