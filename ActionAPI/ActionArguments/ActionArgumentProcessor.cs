using System;
using System.Collections.Generic;
using System.Linq;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;
using ScriptedEventsAPI.ActionAPI.BaseActions;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments;

public class ActionArgumentProcessor(BaseAction action, Script scr)
{
    private Dictionary<Type, Func<BaseToken, string, (Result, BaseActionArgument)>> Converters => new()
    {
        { typeof(TextArgument), (t, n) => (TryConvert(t, n, out TextArgument arg), arg) },
        { typeof(DurationArgument), (t, n) => (TryConvert(t, n, out DurationArgument arg), arg) },
        { typeof(TimeSpanArgument), (t, n) => (TryConvert(t, n, out TimeSpanArgument arg), arg) },
    };

    public Result IsValidArgument(BaseToken token, int index, out BaseActionArgument argument) 
    {
        if (index < action.ExpectedArguments.Length)
        {
            return TryConvertGeneral(token, action.ExpectedArguments[index], out argument);
        }
        
        argument = null!;
        return $"Action {action.Name} does not expect more arguments than {action.ExpectedArguments.Length}.";
    }

    public Result TryConvertGeneral<TArgument>(BaseToken token, TArgument argument, out TArgument resultArg)
        where TArgument : BaseActionArgument
    {
        resultArg = null!;

        return argument switch
        {
            TextArgument stringArgument => Convert(stringArgument, out resultArg),
            DurationArgument durationArgument => Convert(durationArgument, out resultArg),
            TimeSpanArgument timeSpanArgument => Convert(timeSpanArgument, out resultArg),
            _ => $"No converter for {typeof(TArgument)} found."
        };

        Result Convert<TArgConv>(TArgConv inArg, out TArgument outArg)
            where TArgConv : BaseActionArgument
        {
            outArg = null!;
            if (!Converters.TryGetValue(typeof(TArgConv), out var converter))
            {
                return $"No converter for {typeof(TArgConv)} found.";
            }
            
            var (result, resArg) = converter(token, inArg.Name);
            if (result.HasErrored())
            {
                return result;
            }

            if (resArg is not TArgument actionArgumentWithValue)
            {
                throw new Exception($"Converter for {typeof(TArgConv)} returned a value of type not matching {typeof(TArgument)}.");
            }

            outArg = actionArgumentWithValue;
            return true;
        }
    }

    public static Result TryConvert(BaseToken token, string argName, out DurationArgument argument)
    {
        argument = null!;
        
        if (token is not UnclassifiedValueToken valueToken)
        {
            return false;
        }
        
        if (!float.TryParse(valueToken.RawRepresentation, out float duration))
        {
            return false;
        }

        argument = new(argName)
        {
            Value = TimeSpan.FromSeconds(duration)
        };
        
        return true;
    }
    
    public Result TryConvert(BaseToken token, string argName, out TextArgument argument)
    {
        switch (token)
        {
            case LiteralVariableToken literalVariableToken:
                argument = new(argName, () =>
                {
                    var variable = scr.LocalLiteralVariables.FirstOrDefault(v => v.Name == literalVariableToken.RawRepresentation);
                    return variable is not null
                        ? variable.Value
                        : literalVariableToken.WithParentheses;
                });
                
                return true;
            default:
                argument = new(argName, () => token.RawRepresentation);
                return true;
        }
    }
    
    public static Result TryConvert(BaseToken token, string argName, out TimeSpanArgument argument)
    {
        argument = null!;
        var unitIndex = token.RawCharRepresentation.FindIndex(char.IsLetter);
        if (unitIndex == -1)
        {
            return "No unit provided.";
        }
        
        var valuePart = token.RawCharRepresentation.Take(unitIndex).ToArray();
        if (!valuePart.All(char.IsDigit))
        {
            return new(false, $"Value parts ({string.Join("", valuePart)}) only be made of numbers.");
        }
        
        if (!double.TryParse(string.Join("", valuePart), out var valueAsDouble))
        {
            return new(false, $"Value parts ({string.Join("", valuePart)}) only be made of numbers.");
        }

        var unit = token.RawRepresentation.Substring(unitIndex);
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