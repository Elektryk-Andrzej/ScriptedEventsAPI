using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Contexts.Control;
using ScriptedEventsAPI.ScriptAPI.Contexting.Contexts.Loops;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

public class KeywordToken(Script scr) : BaseContextableToken(scr)
{
    private readonly Script _scr = scr;

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
            "if" => new IfStatementContext(_scr),
            "for" => new ForLoopContext(_scr),
            "end" => new TerminationContext(),
            "continue" => new LoopContinueContext(),
            "repeat" => new RepeatLoopContext(_scr),
            "stop" => new StopScriptContext(_scr),
            _ => $"Value '{RawRepresentation}' is not a keyword."
        };
    }
}