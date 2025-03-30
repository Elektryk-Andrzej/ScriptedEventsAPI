using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Contexts;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

public class LiteralVariableToken(Script scr) : BaseContextableToken
{
    public override BaseContext GetResultingContext()
    {
        return new LiteralVariableDefinitionContext(this, scr);
    }
    
    
}