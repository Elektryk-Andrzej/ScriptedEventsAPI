using System;
using System.Collections.Generic;
using ScriptedEventsAPI.ConditionAPI.ConditionElements;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodAPI.Exceptions;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.ConditionAPI;

public class ConditionEvaluator(string expression, Script scr)
{
    private Clause? _clause = null;

    public Result IsValid()
    {
        var elements = GetElementsFromExpression(expression);
        return IsValidClause(elements, out _clause);
    }

    public Result Evaluate(out bool didEvaluateTrue)
    {
        didEvaluateTrue = false;
        if (_clause is null)
        {
            var resp = IsValid();
            if (resp.HasErrored()) return resp;
        }

        var (condRes, wasSuccess) = EvaluateCondition();
        didEvaluateTrue = condRes;
        return wasSuccess;
    }

    private static List<IConditionElement> GetElementsFromExpression(string expression)
    {
        List<IConditionElement> history = [];
        foreach (var part in VariableParser.SplitWithCharIgnoringVariables(expression, char.IsWhiteSpace))
        {
            var oper = GetOperator(part);
            if (oper != OperatorType.Invalid)
            {
                history.Add(new Operator
                {
                    OperatorType = oper
                });
                continue;
            }

            history.Add(new Operand
            {
                Value = part
            });
        }

        return history;
    }

    private (bool conditionResult, Result wasSuccess) EvaluateCondition()
    {
        if (_clause is null) throw new DeveloperFuckupException("clause is null");

        var oper = _clause.Operator.OperatorType;
        if (oper is OperatorType.And or OperatorType.Or)
            throw new NotImplementedException("`and` as well as `or` operators are not supported");

        var firstOperand = VariableParser.ReplaceVariablesInContaminatedString(_clause.FirstOperand.Value, scr);
        var secondOperand = VariableParser.ReplaceVariablesInContaminatedString(_clause.SecondOperand.Value, scr);
        Logger.Debug($"Operand 1: {_clause.FirstOperand.Value} -> {firstOperand}");
        Logger.Debug($"Operand 2: {_clause.SecondOperand.Value} -> {secondOperand}");
        
        bool result;
        if (oper is OperatorType.Equal)
        {
            result = firstOperand == secondOperand;
            return (result, true);
        }

        if (!float.TryParse(firstOperand, out var number1))
            return (false, $"Value '{firstOperand}' is not a valid number!");

        if (!float.TryParse(secondOperand, out var number2))
            return (false, $"Value '{secondOperand}' is not a valid number!");

        result = oper switch
        {
            OperatorType.BiggerThan => number1 > number2,
            OperatorType.BiggerThanOrEqual => number1 >= number2,
            OperatorType.SmallerThan => number1 < number2,
            OperatorType.SmallerThanOrEqual => number1 <= number2,
            _ => throw new NotImplementedException($"Operator '{oper}' is not implemented!")
        };

        return (result, true);
    }

    private static Result IsValidClause(List<IConditionElement> elements, out Clause clause)
    {
        clause = null!;
        if (elements.Count != 3)
            return
                $"A condition can only contain exactly [3] elements (1st operand, operator and 2nd operand), but has [{elements.Count}]. " +
                $"Example: (15 > 7), where `15` is 1st operand, `>` is operator and `7` is 2nd operand.";

        if (elements[0] is not Operand firstOperand)
            return $"The 1st element of a condition [{elements[0]}] must be a value! " +
                   $"Example: (15 > 7), where 1st element [ 15 ] is a value.";

        if (elements[1] is not Operator @operator)
            return $"The 2nd element of a condition [{elements[1]}] must be an operator! " +
                   $"Example: (15 > 7), where 2nd element [ > ] is an operator.";

        if (elements[2] is not Operand secondOperand)
            return $"The 3rd element of a condition [{elements[2]}] must be a value! " +
                   $"Example: (15 > 7), where 3rd element [ 7 ] is a value.";

        clause = new()
        {
            FirstOperand = firstOperand,
            Operator = @operator,
            SecondOperand = secondOperand
        };

        return true;
    }

    private static OperatorType GetOperator(string part)
    {
        return part switch
        {
            "=" => OperatorType.Equal,
            "isEqual" => OperatorType.Equal,

            ">" => OperatorType.BiggerThan,
            "isBiggerThan" => OperatorType.BiggerThan,

            ">=" => OperatorType.BiggerThanOrEqual,
            "isBiggerThanOrEqual" => OperatorType.BiggerThanOrEqual,

            "<" => OperatorType.SmallerThan,
            "isSmallerThan" => OperatorType.SmallerThan,

            "<=" => OperatorType.SmallerThanOrEqual,
            "isSmallerThanOrEqual" => OperatorType.SmallerThanOrEqual,

            "and" => OperatorType.And,
            "or" => OperatorType.Or,

            _ => OperatorType.Invalid
        };
    }
}