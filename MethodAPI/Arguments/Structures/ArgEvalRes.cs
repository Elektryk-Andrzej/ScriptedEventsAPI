using System;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodAPI.Arguments.Interfaces;

namespace ScriptedEventsAPI.MethodAPI.Arguments.Structures;

public class ArgEvalRes<T> : IArgEvalRes
{
    private ResInfo? _dynConRes = null;

    public ArgEvalRes(ResInfo staticResult)
    {
        IsStatic = true;
        GetValue = () => staticResult.Value;
        GetResult = () => staticResult.Result;
    }

    public ArgEvalRes(string errorMsg)
    {
        IsStatic = true;
        GetValue = () => default!;
        GetResult = () => errorMsg;
    }

    public ArgEvalRes(T value)
    {
        IsStatic = true;
        GetValue = () => value;
        GetResult = () => true;
    }

    public ArgEvalRes(Result result, T value)
    {
        IsStatic = true;
        GetValue = () => value;
        GetResult = () => result;
    }

    public ArgEvalRes(Func<ResInfo> dynamicResult)
    {
        IsStatic = false;
        GetValue = () => dynamicResult().Value;
        GetResult = () => dynamicResult().Result;
    }

    public Func<T> GetValue { get; }

    public bool IsStatic { get; }
    public Func<Result> GetResult { get; }

    public class ResInfo
    {
        public required T Value { get; init; }
        public required Result Result { get; init; }

        public static implicit operator ResInfo(string res)
        {
            return new()
            {
                Value = default!,
                Result = res
            };
        }
        
        public static implicit operator ResInfo(T value)
        {
            return new()
            {
                Value = value,
                Result = true
            };
        }
    }
}