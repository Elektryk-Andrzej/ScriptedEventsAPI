using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI.TokenLexers;

public abstract class BaseTokenLexer
{
    public abstract BaseToken Token { get; set; }
    protected abstract bool IsValid(char c);

    public virtual void TryAddChar(char c, out bool shouldContinueExecution)
    {
        shouldContinueExecution = IsValid(c);
        if (shouldContinueExecution)
        {
            Token.AddChar(c);
        }
    }

    protected BaseTokenLexer(char initChar)
    {
        Token.AddChar(initChar);
    }

    protected BaseTokenLexer()
    {
    }
}