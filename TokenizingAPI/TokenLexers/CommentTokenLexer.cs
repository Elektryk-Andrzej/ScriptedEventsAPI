using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI.TokenLexers;

public class CommentTokenLexer : BaseTokenLexer
{
    public override BaseToken Token { get; set; } = new CommentToken();
    protected override bool IsValid(char c) => true;
}