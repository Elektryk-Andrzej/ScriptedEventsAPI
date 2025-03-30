using System.Collections.Generic;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

public abstract class BaseToken
{
    public string TokenName => GetType().Name;
    public List<char> RawCharRepresentation { get; } = [];
    public string RawRepresentation => string.Join("", RawCharRepresentation);

    public void AddChar(char c)
    {
        RawCharRepresentation.Add(c);
    }

    public override string ToString()
    {
        return $"[Token: {TokenName} | Value: {RawRepresentation}]";
    }
}