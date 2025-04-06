using System;

namespace ScriptedEventsAPI.VariableAPI.Structures;

public class LiteralVariable
{
    public required string Name { get; init; }
    public required Func<string> Value { get; init; }
}