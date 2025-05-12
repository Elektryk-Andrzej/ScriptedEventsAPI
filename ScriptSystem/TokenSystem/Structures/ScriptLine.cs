using System.Collections.Generic;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.TokenSystem.Structures;

public struct ScriptLine
{
    public required Script Script { get; init; }
    public required int LineNumber { get; init; }
    public required List<BaseToken> Tokens { get; init; }
}