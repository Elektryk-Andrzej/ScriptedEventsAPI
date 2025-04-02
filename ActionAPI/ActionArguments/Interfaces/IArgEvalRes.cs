using System;
using ScriptedEventsAPI.OtherStructures.ResultStructure;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Interfaces;

public interface IArgEvalRes
{
    public bool IsStatic { get; }
    public Func<Result> GetResult { get; }
}