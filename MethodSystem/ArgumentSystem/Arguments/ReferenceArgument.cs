using System;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Structures;
using ScriptedEventsAPI.ScriptSystem;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;

public class ReferenceArgument<TValue>(string name) : BaseMethodArgument(name)
{
    public readonly Type ValueType = typeof(TValue);
    
    public static ArgEvalRes<TValue> GetConvertSolution(BaseToken token, Script scr)
    {
        return VariableParser.IsVariableUsedInString(token.RawRepresentation, scr,
            out var getProcessedVariableValueFunc)
            ? new(() => InternalConvert(getProcessedVariableValueFunc()))
            : new(InternalConvert(token.RawRepresentation));
    }

    private static ArgEvalRes<TValue>.ResInfo InternalConvert(string value)
    {
        if (!ObjectReferenceSystem.TryRetreiveObject(value, out var obj))
        {
            return $"Value '{value}' is not a valid object reference.";
        }
        
        return obj switch
        {
            TValue valueObj => valueObj,
            _ => $"Value '{value}' is not a valid object reference."
        };
    }
}