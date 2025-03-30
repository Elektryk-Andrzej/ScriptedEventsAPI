using System.Text;
using System.Text.RegularExpressions;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI;

namespace ScriptedEventsAPI.VariableAPI;

/// <summary>
/// Replaces variables in contaminated strings.
/// </summary>
public static class VariableSubstitution
{
    public static void Process(ref string input, Script scr)
    {
        input = Regex.Replace(input, "({.*?})", match =>
        {
            Logger.Debug($"Changing {match.Value} to its value");
            var name = match.Value.Substring(1, match.Value.Length - 2);
            
            if (scr.TryGetLiteralVariable(name, out var variable))
            {
                Logger.Debug($"success! {name} is a valid var, setting {variable.Value} as");
                return variable.Value;
            }
            else
            {
                Logger.Debug($"error! {name} is not a valid variable");
                return match.Value;
            }
        });
    }
}