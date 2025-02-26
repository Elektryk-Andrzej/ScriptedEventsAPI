using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI.TokenLexers;

public class ControlFlowTokenLexer(char initChar) : BaseTokenLexer(initChar)
{
    public override BaseToken Token { get;set; } = new ControlFlowToken();

    protected override bool IsValid(char c)
    {
        return char.IsLetter(c) && char.IsLower(c);
    }
}