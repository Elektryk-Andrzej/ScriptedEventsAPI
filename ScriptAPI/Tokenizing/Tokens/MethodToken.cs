using ScriptedEventsAPI.MethodAPI.BaseMethods;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Contexts;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

public class MethodToken : BaseContextableToken
{
    public BaseMethod? Method { get; set; } = null;
    public override BaseContext? GetResultingContext()
    {
        return Method is not null
            ? new LineMethodContext(this, Method.Script) 
            : null;
    }
}