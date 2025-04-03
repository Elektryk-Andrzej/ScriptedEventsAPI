using System;
using System.Collections.Generic;
using Exiled.API.Features;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.Arguments.Interfaces;
using ScriptedEventsAPI.MethodAPI.BaseMethods;
using ScriptedEventsAPI.MethodAPI.Exceptions;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.MethodAPI.Arguments.Structures;

public class ProvidedArguments(BaseMethod method)
{
    private Dictionary<(string name, Type type), IArgEvalRes> Arguments { get; } = [];

    public TimeSpan GetDuration(string argName)
    {
        return GetValue<TimeSpan, DurationArgument>(argName);
    }
    
    public string GetText(string argName)
    {
        return VariableParser.ReplaceVariables(GetValue<string, TextArgument>(argName), method.Script);
    }
    
    public string GetTextUnprocessed(string argName)
    {
        return GetValue<string, TextArgument>(argName);
    }

    public List<Player> GetPlayers(string argName)
    {
        return GetValue<List<Player>, PlayerVariableArgument>(argName);
    }
    
    public Player GetPlayer(string argName)
    {
        return GetValue<Player, SinglePlayerArgument>(argName);
    }

    public TEnum GetEnum<TEnum>(string argName)
    {
        var obj = GetValue<object, EnumArgument>(argName);
        if (obj is not TEnum value)
        {
            throw new DeveloperFuckupException($"Cannot convert {obj.GetType().Name} to {typeof(TEnum).Name}");
        }
        
        return value;
    }

    public string GetOption(string argName)
    {
        return GetValue<string, OptionsArgument>(argName);
    }

    private TValue GetValue<TValue, TArg>(string argName)
    {
        var rs = new ResultStacker($"Fetching argument '{argName}' (value {typeof(TValue).Name}) (argtype {typeof(TArg).Name}) for method '{method.Name}' failed.");
        
        var evaluator = GetValueInternal(argName, typeof(TArg));
        var res = evaluator.GetResult();
        
        if (res.HasErrored())
        {
            Logger.Debug($"Error msg: " + res.ErrorMsg);
            throw new ArgumentFetchException(rs.AddExternal(res).ErrorMsg);
        }

        if (evaluator is not ArgEvalRes<TValue> argEvalRes)
        {
            throw new DeveloperFuckupException(
                rs.AddInternal($"Argument value is not of type {typeof(TValue).Name}").ErrorMsg);
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
        Logger.Debug($"Registering variable {skeleton.Name} of type {skeleton.ArgumentType.Name} for method {method.Name}.");
        Arguments.Add((skeleton.Name, skeleton.ArgumentType), skeleton.Evaluator);
    }

    public int Count => Arguments.Count;
}