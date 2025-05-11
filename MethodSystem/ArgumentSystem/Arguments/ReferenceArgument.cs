using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Structures;
using ScriptedEventsAPI.ScriptSystem;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;
using ScriptedEventsAPI.VariableSystem;

namespace ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;

public class ReferenceArgument<TValue>(string name) : BaseMethodArgument(name)
{
    public override string OperatingValueDescription => $"A reference to {typeof(TValue).Name}";
    
    public ArgumentEvaluation<TValue> GetConvertSolution(BaseToken token, Script scr)
    {
        return DefaultConvertSolution(token, scr, InternalConvert);
    }

    private static ArgumentEvaluation<TValue>.EvalRes InternalConvert(string value)
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