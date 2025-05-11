using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Structures;
using ScriptedEventsAPI.ScriptSystem;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;

namespace ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;

public class IntAmountArgument(string name, int minValue) : BaseMethodArgument(name)
{
    public override string OperatingValueDescription => "Whole number (integer) value e.g. 420";
    
    public ArgumentEvaluation<int> GetConvertSolution(BaseToken token, Script scr)
    {
        return DefaultConvertSolution(token, scr, InternalConvert);
    }

    private ArgumentEvaluation<int>.EvalRes InternalConvert(string value)
    {
        if (!int.TryParse(value, out var result))
            return new()
            {
                Result = Rs.Add($"Value '{value}' cannot be interpreted as an integer."),
                Value = 0
            };

        if (result < minValue)
            return new()
            {
                Result = Rs.Add($"Value {result} is lower than allowed minimum value {minValue}."),
                Value = 0
            };

        return new()
        {
            Result = true,
            Value = result
        };
    }
}