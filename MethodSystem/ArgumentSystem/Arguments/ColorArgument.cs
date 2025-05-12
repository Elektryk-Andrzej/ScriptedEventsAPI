using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using UnityEngine;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public class ColorArgument(string name) : BaseMethodArgument(name)
{
    public override string OperatingValueDescription => "Color in HEX format e.g. #FF2299";
    
    public ArgumentEvaluation<Color> GetConvertSolution(BaseToken token, Script scr)
    {
        return DefaultConvertSolution(token, scr, InternalConvert);
    }

    private ArgumentEvaluation<Color>.EvalRes InternalConvert(string value)
    {
        if (ColorUtility.TryParseHtmlString(value, out var result))
        {
            return result;
        }
        
        return Rs.Add($"Value '{value}' is not a valid color.");
    }
}