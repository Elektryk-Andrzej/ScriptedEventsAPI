using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.TokenLexers;

public class ControlFlowTokenLexer(char initChar) : BaseTokenLexer(initChar)
{
    public override BaseToken Token { get;set; } = new ControlFlowToken();

    protected override bool IsNotCompleted(char c)
    {
        return char.IsLetter(c) && char.IsLower(c);
    }
}