using System.Collections.Generic;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

public abstract class BaseToken
{
    public string Name => GetType().Name;
    public List<char> Representation { get; } = [];
    public string AsString => string.Join("", Representation);

    public void AddChar(char c)
    {
        Representation.Add(c);
    }
}