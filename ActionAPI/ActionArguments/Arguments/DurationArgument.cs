using System;
using System.Linq;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;

public class DurationArgument(string name) : BaseActionArgument(name)
{
    public static ArgEvalRes<TimeSpan> GetConvertSolution(BaseToken token, Script scr)
    {
        return VariableParser.IsVariableUsedInString(token.RawRepresentation, scr, out var procValFunc) 
            ? new(() => InternalConvert(procValFunc())) 
            : new(InternalConvert(token.RawRepresentation));
    }

    private static ArgEvalRes<TimeSpan>.ResInfo InternalConvert(string value)
    {
        var unitIndex = Array.FindIndex(value.ToCharArray(), char.IsLetter);
        if (unitIndex == -1)
        {
            return new()
            {
                Result = "No unit provided.",
                Value = TimeSpan.Zero
            };
        }
        
        var valuePart = value.Take(unitIndex).ToArray();
        if (!valuePart.All(char.IsDigit))
        {
            return new()
            {
                Result = $"Value parts ({string.Join("", valuePart)}) only be made of numbers.",
                Value = TimeSpan.Zero
            };
        }
        
        if (!double.TryParse(string.Join("", valuePart), out var valueAsDouble))
        {
            return new()
            {
                Result = $"Value parts ({string.Join("", valuePart)}) only be made of numbers.",
                Value = TimeSpan.Zero
            };
        }

        var unit = value.Substring(unitIndex);
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
            return new()
            {
                Result = $"Provided unit {unit} is not valid.",
                Value = TimeSpan.Zero
            };
        }
        
        Logger.Debug($"successs! TimeSpan length is {timeSpan.Value.TotalSeconds}s");
        return new()
        {
            Result = true,
            Value = timeSpan.Value
        };
    }
}