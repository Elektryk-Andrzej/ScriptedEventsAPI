﻿using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.TokenLexers;

public class PlayerVariableTokenLexer : BaseTokenLexer
{
    public override BaseToken Token { get; set; } = new PlayerVariableToken();

    protected override bool IsNotCompleted(char c)
    {
        if (Token.Representation.Count == 0)
        {
            return c == '@';
        }

        return char.IsLetter(c);
    }
}