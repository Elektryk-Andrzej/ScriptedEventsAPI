using System.Diagnostics.Contracts;
using System.Linq;

namespace ScriptedEventsAPI.Helpers.ResultStructure;

[Pure]
public class ResultStacker(string initMsg)
{
    private static string Process(string value)
    {
        if (value.Length < 2) return value;
        
        if (char.IsLower(value.First()))
        {
            value = value.First().ToString().ToUpper() + value.Substring(1);
        }

        if (!char.IsPunctuation(value.Last()))
        {
            value += ".";
        }
        
        return value;
    }
    
    [Pure]
    public Result Add(Result other)
    {
        return !other.HasErrored()
            ? Process(other.ErrorMsg)
            : $"{other.ErrorMsg}\n-> {Process(initMsg)}";
    }
}