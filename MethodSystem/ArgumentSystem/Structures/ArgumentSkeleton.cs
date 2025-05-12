using System;
using SER.MethodSystem.ArgumentSystem.Interfaces;

namespace SER.MethodSystem.ArgumentSystem.Structures;

public struct ArgumentSkeleton
{
    public required string Name { get; init; }
    public required Type ArgumentType { get; init; }
    public required IArgEvalRes Evaluator { get; init; }
}