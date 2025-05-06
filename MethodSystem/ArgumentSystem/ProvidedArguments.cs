using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Interfaces;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Structures;
using ScriptedEventsAPI.MethodSystem.BaseMethods;
using ScriptedEventsAPI.MethodSystem.Exceptions;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.MethodSystem.ArgumentSystem;

public class ProvidedArguments(BaseMethod method)
{
    private Dictionary<(string name, Type type), IArgEvalRes> Arguments { get; } = [];

    public int Count => Arguments.Count;

    public T GetReference<T>(string argName)
    {
        return GetValue<T, ReferenceArgument<T>>(argName);
    }
    
    public Door[] GetDoors(string argName)
    {
        return GetValue<Door[], DoorArgument>(argName);
    }
    
    public TimeSpan GetDuration(string argName)
    {
        return GetValue<TimeSpan, DurationArgument>(argName);
    }

    public string GetText(string argName)
    {
        return VariableParser.ReplaceVariablesInContaminatedString(GetValue<string, TextArgument>(argName),
            method.Script);
    }

    public int GetAmount(string argName)
    {
        return GetValue<int, AmountArgument>(argName);
    }

    public string GetUnprocessedText(string argName)
    {
        return GetValue<string, TextArgument>(argName);
    }

    public List<Player> GetPlayers(string argName)
    {
        return GetValue<List<Player>, PlayersArgument>(argName);
    }

    public Player GetSinglePlayer(string argName)
    {
        return GetValue<Player, SinglePlayerArgument>(argName);
    }
    
    public float GetNumber(string argName)
    {
        return GetValue<float, NumberArgument>(argName);
    }

    public Func<bool> GetConditionFunc(string argName)
    {
        GetValueWithEvaluator<bool, ConditionArgument>(argName, out var evalRes);
        return evalRes.GetValue;
    }

    public TEnum GetEnum<TEnum>(string argName)
    {
        var obj = GetValue<object, EnumArgument>(argName);
        if (obj is not TEnum value)
            throw new DeveloperFuckupException($"Cannot convert {obj.GetType().Name} to {typeof(TEnum).Name}");

        return value;
    }

    public string GetOption(string argName)
    {
        return GetValue<string, OptionsArgument>(argName);
    }

    private TValue GetValue<TValue, TArg>(string argName)
    {
        return GetValueWithEvaluator<TValue, TArg>(argName, out _);
    }

    private TValue GetValueWithEvaluator<TValue, TArg>(string argName, out ArgEvalRes<TValue> evalRes)
    {
        var rs = new ResultStacker(
            $"Fetching argument '{argName}' (value {typeof(TValue).Name}) (argtype {typeof(TArg).Name}) " +
            $"for method '{method.Name}' failed.");

        var evaluator = GetValueInternal<TValue, TArg>(argName);
        var res = evaluator.GetResult();

        if (res.HasErrored())
        {
            throw new ArgumentFetchException(rs.Add(res).ErrorMsg);
        }

        if (evaluator is not ArgEvalRes<TValue> argEvalRes)
            throw new DeveloperFuckupException(
                rs.Add($"Argument value is not of type {typeof(TValue).Name}"));

        evalRes = argEvalRes;
        return argEvalRes.GetValue();
    }

    private IArgEvalRes GetValueInternal<TValue, TArg>(string argName)
    {
        if (Arguments.TryGetValue((argName, typeof(TArg)), out var value))
        {
            return value;
        }

        var foundArg = method.ExpectedArguments.FirstOrDefault(arg => arg.Name == argName);
        if (foundArg?.DefaultValue is null)
        {
            throw new Exception($"There is no argument registered of type '{nameof(TArg)}' and name '{argName}'.");
        }

        if (foundArg.DefaultValue is not TValue argValue)
        {
            throw new DeveloperFuckupException(
                $"Argument {argName} of type {nameof(TArg)} for method {method.Name} has its default value set " +
                $"to type {foundArg.DefaultValue.GetType().Name}, expected of type {typeof(TArg).Name}.");
        }

        return new ArgEvalRes<TValue>(argValue);
    }

    public void Add(ArgumentSkeleton skeleton)
    {
        Logger.Debug(
            $"Registering argument {skeleton.Name} of type {skeleton.ArgumentType.Name} for method {method.Name}.");
        Arguments.Add((skeleton.Name, skeleton.ArgumentType), skeleton.Evaluator);
    }
}