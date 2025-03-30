using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.TokenLexers;

public class ControlFlowTokenLexer(char initChar, Script scr) : BaseTokenLexer(initChar)
{
    public override BaseToken Token { get;set; } = new ControlFlowToken(scr);

    protected override bool IsNotCompleted(char c)
    {
        return char.IsLetter(c) && char.IsLower(c);
    }
}