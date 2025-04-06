using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.MethodAPI.BaseMethods;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Contexting;
using ScriptedEventsAPI.ScriptAPI.Contexting.Contexts;
using ScriptedEventsAPI.ScriptAPI.Tokenizing;

namespace ScriptedEventsAPI.VariableAPI;

/// <summary>
/// Replaces variables in contaminated strings.
/// </summary>
public static class VariableParser
{
    public static void ReplaceVariablesInContaminatedString(ref string input, Script scr)
    {
        input = ReplaceVariablesInContaminatedString(input, scr);
    }

    public static string ReplaceVariablesInContaminatedString(string input, Script scr)
    {
        var result = new StringBuilder();
        int i = 0;

        while (i < input.Length)
        {
            if (input[i] != '{')
            {
                result.Append(input[i]);
                i++;
                continue;
            }
            
            int start = i;
            int depth = 1;
            i++;

            while (i < input.Length && depth > 0)
            {
                switch (input[i])
                {
                    case '{':
                        depth++;
                        break;
                    case '}':
                        depth--;
                        break;
                }

                i++;
            }

            if (depth > 0)
            {
                result.Append(input.Substring(start));
                break;
            }
            
            string inner = input.Substring(start + 1, i - start - 2);
            Logger.Debug($"Changing {{{inner}}} to its value");

            if (string.IsNullOrEmpty(inner))
            {
                result.Append($"{{{inner}}}");
                continue;
            }
            
            if (char.IsUpper(inner[0]))
            {
                var tokens = new Tokenizer(scr).GetTokensFromLine(inner.ToCharArray());
                var contexts = new Contexter(scr).LinkAllTokens(tokens);
                if (contexts.Count != 1)
                {
                    Logger.Debug($"{{{inner}}} should be a single context, but fetched {contexts.Count}");
                    result.Append($"{{{inner}}}");
                    continue;
                }

                if (contexts.First() is not MethodContext methodContext)
                {
                    Logger.Debug($"{{{inner}}} should be method, but is a {contexts.First().GetType().Name}");
                    result.Append($"{{{inner}}}");
                    continue;
                }

                if (methodContext.Method is not TextReturningStandardMethod textMethod)
                {
                    Logger.Debug($"{{{inner}}} method does not return a value!");
                    result.Append($"{{{inner}}}");
                    continue;
                }

                textMethod.Execute();
                result.Append(textMethod.TextReturn);
                continue;
            }
            
            if (scr.TryGetLiteralVariable(inner, out var variable))
            {
                Logger.Debug($"success! {inner} is a valid var, setting {variable.Value()} as");
                result.Append(variable.Value());
                continue;
            }
            
            Logger.Debug($"error! {inner} is not a valid variable");
            result.Append($"{{{inner}}}");
        }

        return result.ToString();
    }
    
    public static bool IsVariableUsedInString(string value, Script scr, out Func<string> processedValueFunc)
    {
        processedValueFunc = null!;

        if (Regex.Matches(value, "({.*?})").Count == 0)
        {
            return false;
        }

        processedValueFunc = () => ReplaceVariablesInContaminatedString(value, scr);
        return true;
    }
    
    public static string[] SplitWithCharIgnoringVariables(string input, Func<char, bool> splitChar)
    {
        List<string> parts = [];
        var current = new StringBuilder();
        int depth = 0;

        foreach (char c in input)
        {
            switch (c)
            {
                case '{':
                    depth++;
                    current.Append(c);
                    continue;
                case '}':
                    depth = Math.Max(0, depth - 1);
                    current.Append(c);
                    continue;
            }

            if (splitChar(c) && depth == 0)
            {
                if (current.Length > 0)
                {
                    parts.Add(current.ToString());
                    current.Clear();
                }
                continue;
            }

            current.Append(c);
        }

        if (current.Length > 0)
            parts.Add(current.ToString());

        return parts.ToArray();
    }

    public static bool IsValidVariableSyntax(string input)
    {
        return input.Length > 2 && input.StartsWith("{") && input.EndsWith("}");
    }
}