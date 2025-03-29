using System.Collections.Generic;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

public class ParenthesesToken : BaseToken
{
    public List<BaseToken>? Tokens { get; set; } = null;
}