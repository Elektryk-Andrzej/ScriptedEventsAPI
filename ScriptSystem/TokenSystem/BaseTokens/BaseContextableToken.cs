using System.Diagnostics.Contracts;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.BaseContexts;

namespace ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;

public abstract class BaseContextableToken(Script scr) : BaseToken(scr)
{
    [Pure]
    public abstract TryGet<BaseContext> TryGetResultingContext();
}