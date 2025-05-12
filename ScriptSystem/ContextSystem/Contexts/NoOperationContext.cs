using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.ContextSystem.Contexts;

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