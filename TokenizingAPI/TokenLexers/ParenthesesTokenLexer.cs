using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI.TokenLexers;

public class ParenthesesTokenLexer : BaseTokenLexer
{
    public int _numberOfOpenParentheses = 1;
    
    public override BaseToken Token { get; set; } = new ParenthesesToken();

    protected override bool IsValid(char c)
    {
        return _numberOfOpenParentheses > 0;
    }

    public override void TryAddChar(char c, out bool shouldContinueExecution)
    {
        switch (c)
        {
            case '(':
                _numberOfOpenParentheses++;
                break;
            case ')':
                _numberOfOpenParentheses--;
                break;
        }
        
        base.TryAddChar(c, out shouldContinueExecution);
    }
}