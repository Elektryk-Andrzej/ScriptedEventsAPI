using System;
using System.Text.RegularExpressions;
using NCalc;
using ScriptedEventsAPI.ScriptSystem;
using ScriptedEventsAPI.VariableSystem;

namespace ScriptedEventsAPI.Helpers;

public static class Condition
{
    public static TryGet<bool> TryEval(string value, Script scr)
    {
        try
        {
            // flee being weird
            value = value.Replace("!=", "<>");
            value = VariableParser.ReplaceVariablesInContaminatedString(value, scr);
            
            var expression = new Expression(value);
            
            var matches = Regex.Matches(value, @"\w+");
            foreach (Match match in matches)
            {
                if (double.TryParse(match.Value, out _))
                {
                    continue;
                }
                
                expression.Parameters[match.Value] = match.Value;
            }
            
            var result = expression.Evaluate();
            if (result is not bool boolRes)
            {
                return "Result is not a true/false value.";
            }
            
            return boolRes;
        }
        catch (Exception ex)
        {
            return $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}";
        }
    }
}