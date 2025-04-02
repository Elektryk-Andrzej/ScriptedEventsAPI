using System;
using System.Collections.Generic;
using System.Threading;
using Exiled.API.Features;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Interfaces;
using ScriptedEventsAPI.ActionAPI.ActionExceptions;
using ScriptedEventsAPI.ActionAPI.BaseActions;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;

public class ProvidedArguments(BaseAction action)
{
    private Dictionary<(string name, Type type), IArgEvalRes> Arguments { get; } = [];

    public TimeSpan GetDuration(string argName)
    {
        return GetValue<TimeSpan, DurationArgument>(argName);
    }
    
    public string GetText(string argName)
    {
        return VariableParser.ReplaceVariables(GetValue<string, TextArgument>(argName), action.Script);
    }
    
    public string GetTextUnprocessed(string argName)
    {
        return GetValue<string, TextArgument>(argName);
    }

    public List<Player> GetPlayers(string argName)
    {
        return GetValue<List<Player>, PlayerVariableArgument>(argName);
    }

    public TEnum GetEnum<TEnum>(string argName)
    {
        return GetValue<TEnum, EnumArgument>(argName);
    }

    private TValue GetValue<TValue, TArg>(string argName)
    {
        var res = GetValueInternal(argName, typeof(TArg));
        if (res.GetResult().HasErrored())
        {
            throw new ArgumentFetchException();
        }

        if (res is not ArgEvalRes<TValue> argEvalRes)
        {
            throw new Exception();
        }
        
        return argEvalRes.GetValue();
    }

    private IArgEvalRes GetValueInternal(string argName, Type argType)
    {
        if (!Arguments.TryGetValue((argName, argType), out IArgEvalRes value))
        {
            throw new Exception($"There is no argument registered of type '{argType.Name}' and name '{argName}'.");
        }

        return value;
    }

    public void Add(ArgumentSkeleton skeleton)
    {
        Logger.Debug($"Registering variable {skeleton.Name} of type {skeleton.ArgumentType.Name} for action {action.Name}.");
        Arguments.Add((skeleton.Name, skeleton.ArgumentType), skeleton.Evaluator);
    }

    public int Count => Arguments.Count;
}