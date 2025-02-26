using System.Collections.Generic;

namespace ScriptedEventsAPI.TokenizingAPI.Tokens;

public abstract class BaseToken
{
    public string Name => GetType().Name;
    public List<char> Representation { get; } = [];

    public virtual void AddChar(char c)
    {
        Representation.Add(c);
    }

    public virtual void OnFinished()
    {
    }
}