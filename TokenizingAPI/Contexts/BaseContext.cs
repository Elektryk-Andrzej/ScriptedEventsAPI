using System.Collections.Generic;
using ScriptedEventsAPI.TokenizingAPI.Contexts.Structures;
using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI.Contexts;

public abstract class BaseContext
{
    public string Name => GetType().Name;
    public abstract TryAddTokenRes TryAddToken(BaseToken token);
    public abstract IEnumerator<float> Execute();
    public override string ToString()
    {
        return Name;
    }
}