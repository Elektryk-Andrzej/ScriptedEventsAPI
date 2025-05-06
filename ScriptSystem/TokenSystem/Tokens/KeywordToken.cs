using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.BaseContexts;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.Contexts.Control;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.Contexts.Loops;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;

namespace ScriptedEventsAPI.ScriptSystem.TokenSystem.Tokens;

public class KeywordToken(Script scr) : BaseContextableToken(scr)
{
    public override bool EndParsingOnChar(char c)
    {
        return char.IsWhiteSpace(c);
    }

    public override Result IsValidSyntax()
    {
        return true;
    }

    public override TryGet<BaseContext> TryGetResultingContext()
    {
        return RawRepresentation.ToLower() switch
        {
            "if" => new IfStatementContext(Script),
            "for" => new ForLoopContext(Script),
            "end" => new TerminationContext(),
            "continue" => new LoopContinueContext(),
            "repeat" => new RepeatLoopContext(Script),
            "stop" => new StopScriptContext(Script),
            "forever" => new ForeverLoopContext(),
            _ => $"Value '{RawRepresentation}' is not a keyword."
        };
    }
}