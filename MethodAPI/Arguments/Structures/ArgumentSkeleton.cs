using System;
using ScriptedEventsAPI.MethodAPI.Arguments.Interfaces;

namespace ScriptedEventsAPI.MethodAPI.Arguments.Structures;

public struct ArgumentSkeleton
{
    public required string Name { get; init; }
    public required Type ArgumentType { get; init; }
    public required IArgEvalRes Evaluator { get; init; }
}