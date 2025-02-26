using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI.TokenLexers;

public class LiteralValueTokenLexer(char initChar) : BaseTokenLexer(initChar)
{
    public override BaseToken Token { get; set; } = new LiteralValueToken();
    protected override bool IsValid(char c) => !char.IsWhiteSpace(c);
}