using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Contexts;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Structures;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

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
        return new NoopContext();
    }
}