using System;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

public class FlagToken(Script scr) : BaseToken(scr)
{
    public override bool EndParsingOnChar(char c)
    {
        throw new NotImplementedException();
    }

    public override Result IsValidSyntax()
    {
        throw new NotImplementedException();
    }
}