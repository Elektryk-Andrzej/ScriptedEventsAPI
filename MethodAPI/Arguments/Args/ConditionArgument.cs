using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodAPI.Arguments.Structures;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.MethodAPI.Arguments.Args;

public class ConditionArgument(string name) : BaseMethodArgument(name)
{
    public static ArgEvalRes<bool> GetConvertSolution(BaseToken token, Script scr)
    {
        if (token is not ParenthesesToken parentheses)
            return new(
                $"Condition must be expressed in parantheses, not '{token.RawRepresentation}'."
            );

        return new(DynamicSolver(parentheses.ValueWithoutBraces, scr));
    }

    private static ArgEvalRes<bool>.ResInfo DynamicSolver(string expression, Script scr)
    {
        Logger.Debug($"evaluating expression '{expression}'");
        if (!Condition.TryEval(expression, scr).HasErrored(out var errorMsg, out var result))
        {
            return result;
        }

        return new ResultStacker($"Condition argument '{expression}' is invalid.").AddInt(errorMsg).ErrorMsg;
    }
}