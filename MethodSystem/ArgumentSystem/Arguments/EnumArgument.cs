using System;
using System.Linq;
using Exiled.API.Extensions;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public class EnumArgument<TEnum>(string name) : BaseMethodArgument(name) where TEnum : Enum
{
    public override string OperatingValueDescription => 
        $"{typeof(TEnum).Name} enum value " +
        $"e.g. {Enum.GetValues(typeof(TEnum)).Cast<TEnum>().GetRandomValue().ToString()}";
    
    public ArgumentEvaluation<object> GetConvertSolution(BaseToken token, Script scr)
    {
        return DefaultConvertSolution(token, scr, InternalConvert);
    }

    private static ArgumentEvaluation<object>.EvalRes InternalConvert(string value)
    {
        if (!Enum.IsDefined(typeof(TEnum), value))
            return new()
            {
                Result = $"Enum {typeof(TEnum).Name} does not include '{value}' as a valid value.",
                Value = null!
            };

        var enumValue = Enum.Parse(typeof(TEnum), value, true);
        return new()
        {
            Result = true,
            Value = enumValue
        };
    }
}