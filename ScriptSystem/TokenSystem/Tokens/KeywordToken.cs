using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Contexts.Control;
using SER.ScriptSystem.ContextSystem.Contexts.Loops;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.TokenSystem.Tokens;

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