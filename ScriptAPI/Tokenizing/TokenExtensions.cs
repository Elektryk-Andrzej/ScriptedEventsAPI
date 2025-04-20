using System.Diagnostics.Contracts;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing;

public static class TokenExtensions
{
    [Pure]
    public static string GetValue(this BaseToken token)
    {
        if (token is ParenthesesToken parentheses) return parentheses.ValueWithoutBraces;

        return token.RawRepresentation;
    }
}