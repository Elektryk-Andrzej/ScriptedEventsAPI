using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Interfaces;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Structures;
using ScriptedEventsAPI.MethodSystem.BaseMethods;
using ScriptedEventsAPI.MethodSystem.Exceptions;
using ScriptedEventsAPI.VariableSystem;
using UnityEngine;
using Logger = ScriptedEventsAPI.Helpers.Logger;

namespace ScriptedEventsAPI.MethodSystem.ArgumentSystem;

public class ProvidedArguments(BaseMethod method)
{
    private Dictionary<(string name, Type type), IArgEvalRes> Arguments { get; } = [];

    public int Count => Arguments.Count;

    public Color GetColor(string argName)
    {
        return GetValue<Color, ColorArgument>(argName);
    }
    
    public Room[] GetRooms(string argName)
    {
        return GetValue<Room[], RoomsArgument>(argName);
    }
    
    public bool GetBool(string argName)
    {
        return GetValue<bool, BoolArgument>(argName);
    }
    
    public float GetPercentage(string argName)
    {
        return GetValue<float, PercentageArgument>(argName);
    }
    
    public T GetReference<T>(string argName)
    {
        return GetValue<T, ReferenceArgument<T>>(argName);
    }
    
    public Door[] GetDoors(string argName)
    {
        return GetValue<Door[], DoorsArgument>(argName);
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

    public int GetIntAmount(string argName)
    {
        return GetValue<int, IntAmountArgument>(argName);
    }
    
    public float GetFloatAmount(string argName)
    {
        return GetValue<float, FloatAmountArgument>(argName);
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

    public TEnum GetEnum<TEnum>(string argName) where TEnum : Enum
    {
        var obj = GetValue<object, EnumArgument<TEnum>>(argName);
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

    private TValue GetValueWithEvaluator<TValue, TArg>(string argName, out ArgumentEvaluation<TValue> evalRes)
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

        if (evaluator is not ArgumentEvaluation<TValue> argEvalRes)
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
        if (foundArg is null)
        {
            throw new DeveloperFuckupException($"There is no argument registered of type '{nameof(TArg)}' and name '{argName}'.");
        }

        if (!foundArg.IsOptional)
        {
            throw new MissingArgumentException($"Method '{method.Name}' is missing required argument '{argName}'.");
        }

        if (foundArg.DefaultValue is not TValue argValue)
        {
            throw new DeveloperFuckupException(
                $"Argument {argName} of type {nameof(TArg)} for method {method.Name} has its default value set " +
                $"to type {foundArg.DefaultValue?.GetType().Name}, expected of type {typeof(TArg).Name}.");
        }

        return new ArgumentEvaluation<TValue>(argValue);
    }

    public void Add(ArgumentSkeleton skeleton)
    {
        Logger.Debug(
            $"Registering argument {skeleton.Name} of type {skeleton.ArgumentType.Name} for method {method.Name}.");
        Arguments.Add((skeleton.Name, skeleton.ArgumentType), skeleton.Evaluator);
    }
}