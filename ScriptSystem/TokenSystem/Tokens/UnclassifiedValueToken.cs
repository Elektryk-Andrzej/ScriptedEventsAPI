using SER.Helpers.ResultStructure;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.TokenSystem.Tokens;

public class UnclassifiedValueToken(Script scr, string initRep = "") : BaseToken(scr, initRep)
{
    public override bool EndParsingOnChar(char c)
    {
        return char.IsWhiteSpace(c);
    }

    public override Result IsValidSyntax()
    {
        return true;
    }
}