using System;
using System.Collections.Generic;
using ScriptedEventsAPI.ConditionAPI.ConditionElements;
using ScriptedEventsAPI.OtherStructures;

namespace ScriptedEventsAPI.ConditionAPI;

public struct ConditionEvaluator
{
    public bool Result { get; internal set; }
    public Result WasConditionSuccessful { get; internal set; }
    
    public ConditionEvaluator(string expression)
    {
        var elements = GetElementsFromExpression(expression);
        var isValid = IsValidClause(elements, out var clause);
        if (!isValid)
        {
            Result = false;
            WasConditionSuccessful = isValid;
            return;
        }

        var result = EvaluateCondition(clause);
        WasConditionSuccessful = result.wasSuccess;
        Result = result.conditionResult;
    }

    private static List<IConditionElement> GetElementsFromExpression(string expression)
    {
        List<IConditionElement> history = [];
        foreach (string part in expression.Split([' '], StringSplitOptions.RemoveEmptyEntries))
        {
            var oper = GetOperator(part);
            if (oper != OperatorType.Invalid)
            {
                history.Add(new Operator
                {
                    OperatorType = oper,
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

    private static (bool conditionResult, Result wasSuccess) EvaluateCondition(Clause clause)
    {
        var oper = clause.Operator.OperatorType;
        if (oper is OperatorType.And or OperatorType.Or)
        {
            throw new NotImplementedException("`and` as well as `or` operators are not supported");
        }

        bool result;
        if (oper is OperatorType.Equal)
        {
            result = clause.FirstOperand.Value == clause.SecondOperand.Value;
            return (result, true);
        }

        if (!float.TryParse(clause.FirstOperand.Value, out float number1))
        {
            return (false, $"Value '{clause.FirstOperand.Value}' is not a valid number!");
        }
        
        if (!float.TryParse(clause.SecondOperand.Value, out float number2))
        {
            return (false, $"Value '{clause.SecondOperand.Value}' is not a valid number!");
        }

        result = oper switch
        {
            OperatorType.BiggerThan => number1 > number2,
            OperatorType.BiggerThanOrEqual => number1 >= number2,
            OperatorType.SmallerThan => number1 < number2,
            OperatorType.SmallerThanOrEqual => number1 <= number2,
            _ => throw new NotImplementedException($"Operator '{oper}' is not implemented!"),
        };

        return (result, true);
    }
    
    private static Result IsValidClause(List<IConditionElement> elements, out Clause clause)
    {
        clause = null!;
        if (elements.Count != 3)
        {
            return $"A condition can only contain exactly [3] elements (1st operand, operator and 2nd operand), but has [{elements.Count}]. " +
                   $"Example: (15 > 7), where `15` is 1st operand, `>` is operator and `7` is 2nd operand.";
        }

        if (elements[0] is not Operand firstOperand)
        {
            return $"The 1st element of a condition [{elements[0]}] must be an operand! " +
                  $"Example: (15 > 7), where 1st element `15` is an operand.";
        }
        
        if (elements[1] is not Operator @operator)
        {
            return $"The 2nd element of a condition [{elements[1]}] must be an operator! " +
                   $"Example: (15 > 7), where 2nd element `>` is an operator.";
        }
        
        if (elements[2] is not Operand secondOperand)
        {
            return $"The 3rd element of a condition [{elements[2]}] must be an operand! " +
                   $"Example: (15 > 7), where 3rd element `7` is an operand.";
        }

        clause = new()
        {
            FirstOperand = firstOperand,
            Operator = @operator,
            SecondOperand = secondOperand,
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
            
            _ => OperatorType.Invalid,
        };
    } 
}