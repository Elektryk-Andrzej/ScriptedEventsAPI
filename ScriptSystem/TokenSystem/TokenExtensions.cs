using System.Diagnostics.Contracts;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.Tokens;

namespace ScriptedEventsAPI.ScriptSystem.TokenSystem;

public static class TokenExtensions
{
    [Pure]
    public static string GetValue(this BaseToken token)
    {
        if (token is ParenthesesToken parentheses) return parentheses.ValueWithoutBraces;

        return token.RawRepresentation;
    }
}