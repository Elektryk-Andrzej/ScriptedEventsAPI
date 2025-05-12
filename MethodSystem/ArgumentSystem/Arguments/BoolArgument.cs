using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public class BoolArgument(string name) : BaseMethodArgument(name)
{
    public override string OperatingValueDescription => "True/False (bool) value";

    public ArgumentEvaluation<bool> GetConvertSolution(BaseToken token, Script scr)
    {
        return DefaultConvertSolution(token, scr, InternalConvert);
    }

    private static ArgumentEvaluation<bool>.EvalRes InternalConvert(string value)
    {
        if (!bool.TryParse(value, out var result))
        {
            return new()
            {
                Result = $"Value '{value}' is not a valid true/false (boolean) value.",
                Value = false
            };
        }
        
        return new()
        {
            Result = true,
            Value = result
        };
    }
}