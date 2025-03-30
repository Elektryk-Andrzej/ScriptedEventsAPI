using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.TokenLexers;

public class PlayerVariableTokenLexer() : BaseTokenLexer('@')
{
    public override BaseToken Token { get; set; } = new PlayerVariableToken();

    protected override bool IsNotCompleted(char c)
    {
        return char.IsLetter(c);
    }
}