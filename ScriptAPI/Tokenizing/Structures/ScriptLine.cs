using System.Collections.Generic;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.Structures;

public struct ScriptLine
{
    public required Script Script { get; init; }
    public required int LineNumber { get; init; }
    public required List<BaseToken> Tokens { get; init; }
}