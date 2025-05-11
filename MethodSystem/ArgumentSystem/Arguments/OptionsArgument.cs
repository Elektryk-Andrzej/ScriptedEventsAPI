using System;
using System.Linq;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Structures;
using ScriptedEventsAPI.ScriptSystem;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;

namespace ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;

public class OptionsArgument(string name, params Option[] options) : BaseMethodArgument(name)
{
    public readonly Option[] Options = options;
    public override string OperatingValueDescription => "Valid option from listed";
    
    public ArgumentEvaluation<string> GetConvertSolution(BaseToken token, Script scr)
    {
        return DefaultConvertSolution(token, scr, InternalConvert);
    }

    private ArgumentEvaluation<string>.EvalRes InternalConvert(string value)
    {
        var option = Options.FirstOrDefault(opt => opt.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));
        if (option == null)
            return new()
            {
                Result = $"Value '{value}' does not match any of the following options: " +
                         $"{string.Join(", ", Options.Select(o => o.Value))}",
                Value = null!
            };

        return new()
        {
            Result = true,
            Value = option.Value
        };
    }
}