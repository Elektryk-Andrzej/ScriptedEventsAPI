using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public class NumberArgument(string name) : BaseMethodArgument(name)
{
    public override string OperatingValueDescription => "Number (float) value e.g. 21.37";
    
    public ArgumentEvaluation<float> GetConvertSolution(BaseToken token, Script scr)
    {
        return DefaultConvertSolution(token, scr, InternalConvert);
    }

    private static ArgumentEvaluation<float>.EvalRes InternalConvert(string value)
    {
        return float.TryParse(value, out var result)
            ? result
            : $"Value '{value}' is not a number.";
    }
}