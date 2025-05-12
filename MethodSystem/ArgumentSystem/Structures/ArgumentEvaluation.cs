using System;
using SER.Helpers.ResultStructure;
using SER.MethodSystem.ArgumentSystem.Interfaces;

namespace SER.MethodSystem.ArgumentSystem.Structures;

public class ArgumentEvaluation<T> : IArgEvalRes
{
    public ArgumentEvaluation(EvalRes staticResult)
    {
        IsStatic = true;
        GetValue = () => staticResult.Value;
        GetResult = () => staticResult.Result;
    }

    public ArgumentEvaluation(string errorMsg)
    {
        IsStatic = true;
        GetValue = () => default!;
        GetResult = () => errorMsg;
    }

    public ArgumentEvaluation(T value)
    {
        IsStatic = true;
        GetValue = () => value;
        GetResult = () => true;
    }

    public ArgumentEvaluation(Result result, T value)
    {
        IsStatic = true;
        GetValue = () => value;
        GetResult = () => result;
    }

    public ArgumentEvaluation(Func<EvalRes> dynamicResult)
    {
        IsStatic = false;
        GetValue = () => dynamicResult().Value;
        GetResult = () => dynamicResult().Result;
    }

    public Func<T> GetValue { get; }

    public bool IsStatic { get; }
    public Func<Result> GetResult { get; }

    public class EvalRes
    {
        public required T Value { get; init; }
        public required Result Result { get; init; }

        public static implicit operator EvalRes(string res)
        {
            return new()
            {
                Value = default!,
                Result = res
            };
        }
        
        public static implicit operator EvalRes(Result res)
        {
            return new()
            {
                Value = default!,
                Result = res
            };
        }
        
        public static implicit operator EvalRes(T value)
        {
            return new()
            {
                Value = value,
                Result = true
            };
        }
    }
}