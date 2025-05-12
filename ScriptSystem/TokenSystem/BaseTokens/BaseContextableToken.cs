using System.Diagnostics.Contracts;
using SER.Helpers;
using SER.ScriptSystem.ContextSystem.BaseContexts;

namespace SER.ScriptSystem.TokenSystem.BaseTokens;

public abstract class BaseContextableToken(Script scr) : BaseToken(scr)
{
    [Pure]
    public abstract TryGet<BaseContext> TryGetResultingContext();
}