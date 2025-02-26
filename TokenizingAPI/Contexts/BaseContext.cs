using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI.Contexts;

public abstract class BaseContext
{
    public string Name => GetType().Name;
    public abstract bool TryAddToken(BaseToken token);
    public abstract void Execute();
}