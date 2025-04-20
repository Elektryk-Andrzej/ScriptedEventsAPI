using System;
using System.Text.RegularExpressions;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.Helpers;

public static class Condition
{
    public static TryGet<bool> TryEval(string expr, Script scr)
    {
        try
        {
            // flee being weird
            expr = expr.Replace("!=", "<>");
            expr = VariableParser.ReplaceVariablesInContaminatedString(expr, scr);
            
            ExpressionContext context = new();
            
            var matches = Regex.Matches(expr, @"\w+");
            foreach (Match match in matches)
            {
                if (double.TryParse(match.Value, out _))
                {
                    continue;
                }
                
                context.Variables[match.Value] = match.Value;
            }
            
            IDynamicExpression e = context.CompileDynamic(expr);
            return (bool)e.Evaluate();
        }
        catch (ExpressionCompileException ex)
        {
            return $"Provided condition '{expr}' is invalid: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}";
        }
    }
}