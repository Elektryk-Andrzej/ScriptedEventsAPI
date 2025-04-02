using System;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Interfaces;
using ScriptedEventsAPI.OtherStructures.ResultStructure;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;

public class ArgEvalRes<T> : IArgEvalRes
{
    private ConversionResult? _dynConRes = null;
    
    public bool IsStatic { get; }
    public Func<T> GetValue { get; }
    public Func<Result> GetResult { get; }

    public ArgEvalRes(ConversionResult staticResult)
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

    public ArgEvalRes(Func<ConversionResult> dynamicResult)
    {
        IsStatic = false;
        GetValue = () =>
        {
            _dynConRes ??= dynamicResult();
            return _dynConRes.Value!;
        };
        GetResult = () =>
        {
            _dynConRes ??= dynamicResult();
            return _dynConRes.Result;
        };
    }

    public static implicit operator ArgEvalRes<T>(Func<ConversionResult> dynamicResult)
    {
        return new(dynamicResult);
    }
    
    public static implicit operator ArgEvalRes<T>(ConversionResult staticResult)
    {
        return new(staticResult);
    }

    public class ConversionResult
    {
        public required T Value { get; init; }
        public required Result Result { get; init; }
    }
}