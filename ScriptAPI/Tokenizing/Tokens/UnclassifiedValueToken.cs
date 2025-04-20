using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Structures;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

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