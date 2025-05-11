using System;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Structures;
using ScriptedEventsAPI.ScriptSystem;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;
using ScriptedEventsAPI.VariableSystem;

namespace ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;

public abstract class BaseMethodArgument(string name)
{
    public string Name { get; } = name;
    public object? DefaultValue { get; init; } = null;
    private readonly bool _isOptional = false;
    public abstract string OperatingValueDescription { get; }

    public bool IsOptional
    {
        get => _isOptional || DefaultValue != null;
        init => _isOptional = value;
    }

    public string? Description { get; init; } = null;
    
    protected ResultStacker Rs => new($"Converting argument {Name} ({GetType().Name}) failed.");

    protected ArgumentEvaluation<T> DefaultConvertSolution<T>(
        BaseToken token, 
        Script scr, 
        Func<string, ArgumentEvaluation<T>.EvalRes> convertMethod)
    {
        return VariableParser.IsVariableUsedInString(token.RawRepresentation, scr, out var replacedVariablesFunc)
            ? new(() => convertMethod(replacedVariablesFunc()))
            : new(convertMethod(token.RawRepresentation));
    }
}