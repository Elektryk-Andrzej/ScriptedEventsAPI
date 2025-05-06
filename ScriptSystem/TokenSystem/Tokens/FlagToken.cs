using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.BaseContexts;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.Contexts;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;

namespace ScriptedEventsAPI.ScriptSystem.TokenSystem.Tokens;

public class FlagToken(Script scr) : BaseContextableToken(scr)
{
    public override bool EndParsingOnChar(char c)
    {
        return char.IsWhiteSpace(c);
    }

    public override Result IsValidSyntax()
    {
        return Result.Assert(
            RawRepresentation == "!--",
            $"Script flag should start with '!--', not '{RawRepresentation}'.");
    }

    public override TryGet<BaseContext> TryGetResultingContext()
    {
        return new NoOperationContext();
    }
}