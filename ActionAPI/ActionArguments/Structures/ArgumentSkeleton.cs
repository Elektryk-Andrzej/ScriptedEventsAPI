using System;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Interfaces;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;

public struct ArgumentSkeleton
{
    public required string Name { get; init; }
    public required Type ArgumentType { get; init; }
    public required IArgEvalRes Evaluator { get; init; }
}