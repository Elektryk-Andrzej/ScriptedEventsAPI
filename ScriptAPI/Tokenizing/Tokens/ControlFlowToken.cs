using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Contexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Contexts.ForLoop;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

public class ControlFlowToken(Script scr) : BaseContextableToken
{
    public override BaseContext? GetResultingContext()
    {
        return RawRepresentation.ToLower() switch
        {
            "if" => new IfStatementContext(scr),
            "for" => new ForLoopContext(scr),
            "end" => new TerminationContext(),
            "continue" => new ForLoopContinueContext(),
            _ => null
        };
    }
}