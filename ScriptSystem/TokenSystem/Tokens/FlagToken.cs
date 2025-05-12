using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Contexts;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.TokenSystem.Tokens;

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