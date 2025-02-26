using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI.TokenLexers;

public class PlayerVariableTokenLexer(char c) : BaseTokenLexer(c)
{
    public override BaseToken Token { get; set; } = new PlayerVariableToken();

    protected override bool IsValid(char c)
    {
        if (c == '@' && Token.Representation.Count == 0)
        {
            return true;
        }

        return char.IsLetter(c);
    }
}