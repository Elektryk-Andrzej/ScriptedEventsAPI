using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Contexts;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

public class ControlFlowToken : BaseContextableToken
{
    public override BaseContext? GetResultingContext()
    {
        return RawRepresentation.ToLower() switch
        {
            "if" => new IfStatementContext(),
            "end" => new TerminationContext(),
            _ => null
        };
    }
}