using System.Diagnostics.Contracts;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

public abstract class BaseContextableToken(Script scr) : BaseToken(scr)
{
    [Pure]
    public abstract TryGet<BaseContext> TryGetResultingContext();
}