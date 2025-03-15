using System.Collections.Generic;
using ScriptedEventsAPI.Other.OpRes;

namespace ScriptedEventsAPI.TokenizingAPI.Tokens;

public abstract class BaseToken
{
    public string Name => GetType().Name;
    public List<char> Representation { get; } = [];
    public string AsString => string.Join("", Representation);

    public virtual void AddChar(char c)
    {
        Representation.Add(c);
    }
}