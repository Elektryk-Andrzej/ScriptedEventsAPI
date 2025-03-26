﻿using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.TokenLexers;

public class LiteralVariableTokenLexer : BaseTokenLexer
{
    private int _numberOfOpenParentheses = 1;
    
    public override BaseToken Token { get; set; } = new LiteralVariableToken();

    protected override bool IsNotCompleted(char c)
    {
        return _numberOfOpenParentheses > 0;
    }

    public override void TryAddChar(char c, out bool shouldContinueExecution)
    {
        if (_numberOfOpenParentheses == 0)
        {
            shouldContinueExecution = false;
            return;
        }
        
        switch (c)
        {
            case '{':
                _numberOfOpenParentheses++;
                break;
            case '}':
                _numberOfOpenParentheses--;
                if (_numberOfOpenParentheses == 0)
                {
                    shouldContinueExecution = true;
                    return;
                }
                break;
        }
        
        base.TryAddChar(c, out shouldContinueExecution);
    }

    public override Result IsFinalStateValid()
    {
        // todo: figure something out for this
        //((ParenthesesToken)Token).Tokens = Tokenizer.GetTokensFromLine(Token.AsString);
        return _numberOfOpenParentheses == 0;
    }
}