using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI.TokenLexers;

public class UnclassifiedValueTokenLexer(char initChar) : BaseTokenLexer(initChar)
{
    public override BaseToken Token { get; set; } = new UnclassifiedValueToken();
    protected override bool IsValid(char c) => !char.IsWhiteSpace(c);
}