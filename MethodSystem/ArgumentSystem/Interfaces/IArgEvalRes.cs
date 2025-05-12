using System;
using SER.Helpers.ResultStructure;

namespace SER.MethodSystem.ArgumentSystem.Interfaces;

public interface IArgEvalRes
{
    public bool IsStatic { get; }
    public Func<Result> GetResult { get; }
}