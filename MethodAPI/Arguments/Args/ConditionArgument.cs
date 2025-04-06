using ScriptedEventsAPI.ConditionAPI;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.MethodAPI.Arguments.Structures;
using ScriptedEventsAPI.MethodAPI.Exceptions;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.MethodAPI.Arguments.Args;

public class ConditionArgument(string name) : BaseMethodArgument(name)
{
    private ConditionEvaluator _evaluator = null!;
    
    public ArgEvalRes<bool> GetConvertSolution(BaseToken token, Script scr)
    {
        if (token is not ParenthesesToken)
        {
            return new(
                $"Condition must be expressed in parantheses, not '{token.RawRepresentation}'."
            );
        }

        _evaluator = new ConditionEvaluator(token.RawRepresentation, scr);
        if (_evaluator.IsValid().HasErrored(out var error))
        {
            return new(error);
        }
        
        return new(DynamicSolver);
    }

    private ArgEvalRes<bool>.ResInfo DynamicSolver()
    {
        Logger.Debug("evaluating condition");
        var res = _evaluator.Evaluate(out var condVal);
        if (res.HasErrored(out var error))
        {
            throw new DeveloperFuckupException(error);
        }
        
        return new()
        {
            Result = res,
            Value = condVal
        };
    }
}