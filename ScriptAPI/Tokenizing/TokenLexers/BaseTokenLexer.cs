﻿using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.TokenLexers;

public abstract class BaseTokenLexer
{
    public abstract BaseToken Token { get; set; }
    protected abstract bool IsNotCompleted(char c);

    public virtual void TryAddChar(char c, out bool shouldContinueExecution)
    {
        shouldContinueExecution = IsNotCompleted(c);
        if (shouldContinueExecution)
        {
            Token.AddChar(c);
        }
    }

    protected BaseTokenLexer(char initChar)
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        // AddChar is available for all
        Token.AddChar(initChar);
    }

    protected BaseTokenLexer()
    {
    }

    public override string ToString()
    {
        return GetType().Name;
    }

    public virtual Result IsFinalStateValid()
    {
        return true;
    }
}