using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.TokenLexers;

public class UnclassifiedValueTokenLexer(char initChar) : BaseTokenLexer(initChar)
{
    public override BaseToken Token { get; set; } = new UnclassifiedValueToken();
    protected override bool IsNotCompleted(char c) => !char.IsWhiteSpace(c);
}