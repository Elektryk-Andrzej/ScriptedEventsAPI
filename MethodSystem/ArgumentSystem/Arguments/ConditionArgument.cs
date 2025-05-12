using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public class ConditionArgument(string name) : BaseMethodArgument(name)
{
    public override string OperatingValueDescription => "A statement that evaluates to True or False e.g. ({value} = 5)";
    
    public static ArgumentEvaluation<bool> GetConvertSolution(BaseToken token, Script scr)
    {
        if (token is not ParenthesesToken parentheses)
            return new(
                $"Condition must be expressed in parantheses, not '{token.RawRepresentation}'."
            );

        return new(DynamicSolver(parentheses.ValueWithoutBraces, scr));
    }

    private static ArgumentEvaluation<bool>.EvalRes DynamicSolver(string expression, Script scr)
    {
        Logger.Debug($"evaluating expression '{expression}'");
        if (!Condition.TryEval(expression, scr).HasErrored(out var errorMsg, out var result))
        {
            return result;
        }

        return new ResultStacker($"Condition argument '{expression}' is invalid.").Add(errorMsg).ErrorMsg;
    }
}