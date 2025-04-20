using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

public class PlayerVariableToken(Script scr) : BaseToken(scr)
{
    public string NameWithoutPrefix => RawRepresentation.Substring(1);

    public override bool EndParsingOnChar(char c)
    {
        return !char.IsLetter(c);
    }

    public override Result IsValidSyntax()
    {
        return Result.Assert(RawRepresentation.Length > 1,
            $"Player variable must have have the the prefix '@' and a name, " +
            $"but '{RawRepresentation}' does not satisfy that.");
    }
}