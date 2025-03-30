using System;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;

public struct ArgumentSkeleton
{
    public required string Name { get; init; }
    public required Type Type { get; init; }
    public required object Value { get; init; }
}