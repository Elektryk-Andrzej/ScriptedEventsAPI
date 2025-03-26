using System.Collections.Generic;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

public class ParenthesesToken : BaseToken
{
    public List<BaseToken>? Tokens { get; set; } = null;
}