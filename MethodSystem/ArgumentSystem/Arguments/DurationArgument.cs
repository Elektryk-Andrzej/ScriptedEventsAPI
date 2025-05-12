using System;
using System.Linq;
using SER.Helpers;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public class DurationArgument(string name) : BaseMethodArgument(name)
{
    public override string OperatingValueDescription => "Duration e.g. 5.3s";
    
    public ArgumentEvaluation<TimeSpan> GetConvertSolution(BaseToken token, Script scr)
    {
        return DefaultConvertSolution(token, scr, InternalConvert);
    }

    private ArgumentEvaluation<TimeSpan>.EvalRes InternalConvert(string value)
    {
        if (TimeSpan.TryParse(value, out var result) && result.TotalMilliseconds > 0)
        {
            return result;
        }
        
        var unitIndex = Array.FindIndex(value.ToCharArray(), char.IsLetter);
        if (unitIndex == -1)
            return new()
            {
                Result = "No unit provided.",
                Value = TimeSpan.Zero
            };

        var valuePart = value.Take(unitIndex).ToArray();
        if (!valuePart.All(char.IsDigit) || !double.TryParse(string.Join("", valuePart), out var valueAsDouble))
            return new()
            {
                Result = $"Value parts ({string.Join("", valuePart)}) only be made of numbers.",
                Value = TimeSpan.Zero
            };

        if (valueAsDouble < 0)
        {
            return Rs.Add("Duration cannot be negative..");
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
            return new()
            {
                Result = $"Provided unit {unit} is not valid.",
                Value = TimeSpan.Zero
            };

        Logger.Debug($"successs! TimeSpan length is {timeSpan.Value.TotalSeconds}s");
        return new()
        {
            Result = true,
            Value = timeSpan.Value
        };
    }
}