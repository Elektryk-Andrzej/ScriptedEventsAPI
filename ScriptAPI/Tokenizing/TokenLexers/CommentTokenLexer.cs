using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.TokenLexers;

public class CommentTokenLexer : BaseTokenLexer
{
    public override BaseToken Token { get; protected set; } = new CommentToken();
    protected override bool IsNotCompleted(char c) => true;
}