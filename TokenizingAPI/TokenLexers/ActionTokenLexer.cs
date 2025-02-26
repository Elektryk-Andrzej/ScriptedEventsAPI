using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI.TokenLexers;

public class ActionTokenLexer(char initChar) : BaseTokenLexer(initChar)
{
    public override BaseToken Token { get; set; } = new ActionToken();

    protected override bool IsValid(char c)
    {
        return char.IsLetter(c);
    }
}