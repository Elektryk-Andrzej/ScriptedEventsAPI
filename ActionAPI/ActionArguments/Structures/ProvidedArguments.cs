using System;
using System.Collections.Generic;
using Exiled.API.Features;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;
using ScriptedEventsAPI.ActionAPI.BaseActions;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;

public class ProvidedArguments(BaseAction action)
{
    private Dictionary<(string name, Type type), object> Arguments { get; } = [];

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
        return (TEnum)((Func<object>)GetValue(argName, typeof(EnumArgument)))();
    }

    private TValue GetValue<TValue, TArg>(string argName)
    {
        return ((Func<TValue>)GetValue(argName, typeof(TArg)))();
    }

    private object GetValue(string argName, Type argType)
    {
        if (!Arguments.TryGetValue((argName, argType), out object value))
        {
            throw new KeyNotFoundException($"There is no argument registered of type '{argType.Name}' and name '{argName}'.");
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