using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI.TokenLexers;

public class LiteralVariableTokenLexer(char initChar) : BaseTokenLexer(initChar)
{
    public override BaseToken Token { get; set; } = new LiteralVariableToken();

    protected override bool IsValid(char c)
    {
        if (c == '$' && Token.Representation.Count == 0)
        {
            return true;
        }

        return char.IsLetter(c);
    }
}