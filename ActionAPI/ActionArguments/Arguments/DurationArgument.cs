using System;
using System.Linq;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;

public class DurationArgument(string name) : BaseActionArgument(name)
{
    public static Result TryConvert(BaseToken token, out Func<TimeSpan> value)
    {
        value = () => TimeSpan.Zero;
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
        
        value = () => timeSpan.Value;
        
        Logger.Debug($"successs! TimeSpan length is {timeSpan.Value.TotalSeconds}s");
        return true;
    }
}