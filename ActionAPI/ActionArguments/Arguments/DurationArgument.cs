using System;
using System.Linq;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;

public class DurationArgument(string name) : BaseActionArgument(name)
{
    public static Result TryConvert(BaseToken token, Script scr, out Func<TimeSpan> value)
    {
        value = () => TimeSpan.Zero;

        var processedName = VariableParser.ReplaceVariables(token.RawRepresentation, scr);
        
        var unitIndex = Array.FindIndex(processedName.ToCharArray(), char.IsLetter);
        if (unitIndex == -1)
        {
            return "No unit provided.";
        }
        
        var valuePart = processedName.Take(unitIndex).ToArray();
        if (!valuePart.All(char.IsDigit))
        {
            return $"Value parts ({string.Join("", valuePart)}) only be made of numbers.";
        }
        
        if (!double.TryParse(string.Join("", valuePart), out var valueAsDouble))
        {
            return $"Value parts ({string.Join("", valuePart)}) only be made of numbers.";
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
            return $"Provided unit {unit} is not valid.";
        }
        
        value = () => timeSpan.Value;
        
        Logger.Debug($"successs! TimeSpan length is {timeSpan.Value.TotalSeconds}s");
        return true;
    }
}