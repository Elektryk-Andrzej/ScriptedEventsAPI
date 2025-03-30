using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;
using ScriptedEventsAPI.ActionAPI.BaseActions;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;
using ScriptedEventsAPI.VariableAPI.Structures;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments;

public class ActionArgumentProcessor(BaseAction action, Script scr)
{
    private Dictionary<Type, Func<BaseToken, BaseActionArgument, (Result, object)>> Converters => new()
    {
        { typeof(TextArgument), (token, _) => (TextArgument.TryConvert(token, scr, out var value), value) },
        { typeof(DurationArgument), (token, _) => (DurationArgument.TryConvert(token, out var value), value) },
        { typeof(PlayerVariableArgument), (token, _) => (PlayerVariableArgument.TryConvert(token, scr, out var value), value)},
        { typeof(EnumArgument), (token, arg) => (((EnumArgument)arg).TryConvert(token, out var value), value)}
    };

    public Result IsValidArgument(BaseToken token, int index, out ArgumentSkeleton skeleton) 
    {            
        skeleton = default;
        
        if (index >= action.ExpectedArguments.Length)
        {
            return $"Action '{action.Name}' does not expect more arguments " +
                   $"than [{action.ExpectedArguments.Length}], but tried to index [{index}].";
        }

        var arg = action.ExpectedArguments[index];
        var argType = arg.GetType();
        if (TryConvertGeneral(token, arg, out var argValue).HasErrored(out var error))
        {
            return error;
        }
        
        skeleton = new()
        {
            Value = argValue,
            Type = argType,
            Name = arg.Name,
        };

        return true;
    }

    public Result TryConvertGeneral(BaseToken token, BaseActionArgument arg, out object argValue)
    {
        argValue = null!;
        var argType = arg.GetType();
        
        if (!Converters.TryGetValue(argType, out var converter))
        {
            return $"No converter for {argType} found.";
        }
            
        var (result, resValue) = converter(token, arg);
        if (result.HasErrored())
        {
            return result;
        }

        argValue = resValue;
        return true;
    }
}