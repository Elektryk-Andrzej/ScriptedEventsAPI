using System.Collections.Generic;

namespace ScriptedEventsAPI.TokenizingAPI.Tokens;

public class ParenthesesToken : BaseToken
{
    public List<BaseToken>? Tokens { get; set; } = null;
    
    public override void OnFinished()
    {
        Tokens = Tokenizer.GetTokensFromLine(Representation);
    }
}