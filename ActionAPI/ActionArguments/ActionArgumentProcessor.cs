using System;
using System.Collections.Generic;
using System.Linq;
using ScriptedEventsAPI.ActionAPI.ActionArguments;
using ScriptedEventsAPI.ActionAPI.Actions;
using ScriptedEventsAPI.ActionAPI.Actions.BaseActions;
using ScriptedEventsAPI.Other;
using ScriptedEventsAPI.Other.OpRes;
using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.ActionAPI;

public class ActionArgumentProcessor(BaseAction action)
{
    public BaseAction Action { get; set; } = action;

    public bool IsValidArgument(BaseToken token, int index, out BaseActionArgument argument) 
    {
        if (index >= Action.RequiredArguments.Length)
        {
            Log.Debug("Provided index is too big.");
            argument = null!;
            return false;
        }
        
        var res = TryConvertGeneral(token, Action.RequiredArguments[index], out argument);
        if (argument is StringArgument stringArgument)
        {
            Log.Debug($"CHECKING OUT VAL {stringArgument.Value}");
        }
        
        return res;
    }

    public static bool TryConvertGeneral<T>(BaseToken token, T argument, out T resultArg)
        where T : BaseActionArgument
    {
        resultArg = null!;
        
        var converters = new Dictionary<Type, Func<BaseToken, string, (OpRes, BaseActionArgument)>>
        {
            { typeof(StringArgument), (t, n) => (TryConvert(t, n, out StringArgument arg), arg) },
            { typeof(DurationArgument), (t, n) => (TryConvert(t, n, out DurationArgument arg), arg) },
            { typeof(TimeSpanArgument), (t, n) => (TryConvert(t, n, out TimeSpanArgument arg), arg) },
        };

        switch (argument)
        {
            case StringArgument stringArgument:
                return Convert(stringArgument, out resultArg);
            case DurationArgument durationArgument:
                return Convert(durationArgument, out resultArg);
            case TimeSpanArgument timeSpanArgument:
                return Convert(timeSpanArgument, out resultArg);
        }

        Log.Debug($"No converter for {typeof(T)} found.");
        return false;

        bool Convert<TConv>(TConv inArg, out T outArg)
            where TConv : BaseActionArgument
        {
            outArg = null!;
            if (!converters.TryGetValue(typeof(TConv), out var converter))
            {
                return false;
            }
            
            var (result, resArg) = converter(token, inArg.Name);
            if (result.HasErrored(out var error) || resArg is not T actionArgumentWithValue)
            {
                Log.Debug($"Converted argument {inArg.Name} to {typeof(TConv)} failed. Reason: {error}");
                return false;
            }
            
            Log.Debug($"arg changed to {typeof(TConv).Name}");

            outArg = actionArgumentWithValue;
            if (outArg is StringArgument stringArgument)
            {
                Log.Debug($"string! {stringArgument.Value}");
            }
            return true;
        }
    }


    public static OpRes TryConvert(BaseToken token, string argName, out DurationArgument argument)
    {
        argument = default!;
        
        if (token is not UnclassifiedValueToken valueToken)
        {
            return false;
        }
        
        if (!float.TryParse(valueToken.AsString, out float duration))
        {
            return false;
        }

        argument = new(argName)
        {
            Value = TimeSpan.FromSeconds(duration)
        };
        
        return true;
    }
    
    public static OpRes TryConvert(BaseToken token, string argName, out StringArgument argument)
    {
        switch (token)
        {
            case ParenthesesToken parenthesesToken:
                argument = new(argName)
                {
                    Value = parenthesesToken.AsString
                };
                break;
            case UnclassifiedValueToken valueToken:
                argument = new(argName)
                {
                    Value = valueToken.AsString
                };
                break;
            default:
                argument = null!;
                return false;
        }
        
        Log.Debug($"arg: '{argument.Value}'");
        return true;
    }
    
    public static OpRes TryConvert(BaseToken token, string argName, out TimeSpanArgument argument)
    {
        argument = null!;
        var unitIndex = token.Representation.FindIndex(char.IsLetter);
        if (unitIndex == -1)
        {
            return new(false, "No unit provided.");
        }
        
        var valuePart = token.Representation.Take(unitIndex).ToArray();
        if (!valuePart.All(char.IsDigit))
        {
            return new(false, $"Value parts ({string.Join("", valuePart)}) only be made of numbers.");
        }
        
        if (!double.TryParse(string.Join("", valuePart), out var valueAsDouble))
        {
            return new(false, $"Value parts ({string.Join("", valuePart)}) only be made of numbers.");
        }

        var unit = token.AsString.Substring(unitIndex);
        TimeSpan? timeSpan = unit switch
        {
            "s" => TimeSpan.FromSeconds(valueAsDouble),
            "ms" => TimeSpan.FromMilliseconds(valueAsDouble),
            "m" => TimeSpan.FromMinutes(valueAsDouble),
            "h" => TimeSpan.FromHours(valueAsDouble),
            "d" => TimeSpan.FromDays(valueAsDouble),
            _ => null
        };

        if (timeSpan is null)
        {
            return new(false, $"Provided unit {unit} is not valid.");
        }

        argument = new(argName)
        {
            Value = timeSpan.Value
        };
        
        Exiled.API.Features.Log.Info($"successs! TimeSpan length is {timeSpan.Value.TotalSeconds}s");
        return true;
    }
}