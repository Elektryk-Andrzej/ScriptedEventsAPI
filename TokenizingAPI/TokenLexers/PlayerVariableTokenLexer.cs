using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI.TokenLexers;

public class PlayerVariableTokenLexer : BaseTokenLexer
{
    public override BaseToken Token { get; set; } = new PlayerVariableToken();

    protected override bool IsValid(char c)
    {
        if (Token.Representation.Count == 0)
        {
            return c == '@';
        }

        return char.IsLetter(c);
    }
}