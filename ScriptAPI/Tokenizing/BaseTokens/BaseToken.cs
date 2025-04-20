using System.Diagnostics.Contracts;
using ScriptedEventsAPI.Helpers.ResultStructure;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

public abstract class BaseToken(Script scr, string initRep = "")
{
    public string TokenName => GetType().Name;
    public string RawRepresentation { get; protected set; } = initRep;
    protected Script Script => scr;

    public void AddChar(char c)
    {
        OnAddingChar(c);
        RawRepresentation += c;
    }

    [Pure]
    public override string ToString()
    {
        return RawRepresentation.Length > 0
            ? $"{TokenName} (value: '{RawRepresentation}')"
            : TokenName;
    }

    [Pure]
    public abstract bool EndParsingOnChar(char c);

    protected virtual void OnAddingChar(char c)
    {
    }

    [Pure]
    public abstract Result IsValidSyntax();
}