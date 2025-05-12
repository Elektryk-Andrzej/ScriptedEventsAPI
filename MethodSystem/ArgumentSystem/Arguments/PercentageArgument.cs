using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public class PercentageArgument(string name) : BaseMethodArgument(name)
{
    public override string OperatingValueDescription => "Percentage value e.g. 69%";
    
    public ArgumentEvaluation<float> GetConvertSolution(BaseToken token, Script scr)
    {
        return DefaultConvertSolution(token, scr, InternalConvert);
    }

    private ArgumentEvaluation<float>.EvalRes InternalConvert(string value)
    {
        if (!value.EndsWith("%"))
        {
            return Rs.Add($"Value '{value}' must end with '%' to be a percentage.");
        }
        
        value = value.Substring(0, value.Length - 1);

        if (!float.TryParse(value, out var result))
        {
            return Rs.Add($"Value '{value}' cannot be interpreted as a number.");
        }

        return result / 100;
    }
}