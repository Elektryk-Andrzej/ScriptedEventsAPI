using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

public abstract class BaseContextableToken : BaseToken
{
    public abstract BaseContext? GetResultingContext();
}