using System;
using ScriptedEventsAPI.Helpers.ResultStructure;

namespace ScriptedEventsAPI.MethodAPI.Arguments.Interfaces;

public interface IArgEvalRes
{
    public bool IsStatic { get; }
    public Func<Result> GetResult { get; }
}