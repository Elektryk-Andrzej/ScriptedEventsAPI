using System.Diagnostics.Contracts;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;

namespace SER.ScriptSystem.TokenSystem;

public static class TokenExtensions
{
    [Pure]
    public static string GetValue(this BaseToken token)
    {
        if (token is ParenthesesToken parentheses) return parentheses.ValueWithoutBraces;

        return token.RawRepresentation;
    }
}