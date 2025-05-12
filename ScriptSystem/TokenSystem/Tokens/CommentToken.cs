using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Contexts;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.TokenSystem.Tokens;

public class CommentToken(Script scr) : BaseContextableToken(scr)
{
    public override bool EndParsingOnChar(char c)
    {
        return false;
    }

    public override Result IsValidSyntax()
    {
        return true;
    }

    public override TryGet<BaseContext> TryGetResultingContext()
    {
        return new NoOperationContext();
    }
}