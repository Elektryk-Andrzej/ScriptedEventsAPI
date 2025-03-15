using System.Collections.Generic;
using ScriptedEventsAPI.Other.OpRes;

namespace ScriptedEventsAPI.TokenizingAPI.Tokens;

public class ParenthesesToken : BaseToken
{
    public List<BaseToken>? Tokens { get; set; } = null;
}