using System.Linq;
using ScriptedEventsAPI.Other.OpRes;
using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI.TokenLexers;

public class ParenthesesTokenLexer : BaseTokenLexer
{
    private int _numberOfOpenParentheses = 1;
    
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

    public override OpRes IsFinalStateValid()
    {
        ((ParenthesesToken)Token).Tokens = Tokenizer.GetTokensFromLine(Token.AsString);
        return _numberOfOpenParentheses == 0;
    }
}