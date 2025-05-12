using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public class FloatAmountArgument(string name, float minValue) : BaseMethodArgument(name)
{
    public override string OperatingValueDescription => "Number (float) value e.g. 21.37";
    
    public ArgumentEvaluation<float> GetConvertSolution(BaseToken token, Script scr)
    {
        return DefaultConvertSolution(token, scr, InternalConvert);
    }

    private ArgumentEvaluation<float>.EvalRes InternalConvert(string value)
    {
        if (!float.TryParse(value, out var result))
            return new()
            {
                Result = Rs.Add($"Value '{value}' cannot be interpreted as a number."),
                Value = 0
            };

        if (result < minValue)
            return new()
            {
                Result = Rs.Add($"Value '{result}' is lower than allowed minimum value {minValue}."),
                Value = 0
            };

        return new()
        {
            Result = true,
            Value = result
        };
    }
}