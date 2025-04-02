using System;
using System.Collections.Generic;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Interfaces;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;
using ScriptedEventsAPI.ActionAPI.BaseActions;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.OtherStructures.ResultStructure;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments;

public class ActionArgumentProcessor(BaseAction action, Script scr)
{
    private readonly Dictionary<Type, Func<BaseToken, BaseActionArgument, IArgEvalRes>> _converters = new()
    {
        { typeof(TextArgument), (token, _) => TextArgument.GetConvertSolution(token, scr) },
        { typeof(DurationArgument), (token, _) => DurationArgument.GetConvertSolution(token, scr) },
        { typeof(PlayerVariableArgument), (token, _) => PlayerVariableArgument.GetConvertSolution(token, scr)},
        { typeof(EnumArgument), (token, arg) => ((EnumArgument)arg).GetConvertSolution(token, scr)}
    };

    public Result IsValidArgument(BaseToken token, int index, out ArgumentSkeleton skeleton)
    {
        var rs = new ResultStacker(
            $"Argument '{token.RawRepresentation}' (index {index}) for action {action.Name} is invalid!");
        
        skeleton = default;
        
        if (index >= action.ExpectedArguments.Length)
        {
            return rs.AddInternal(
                $"Action does not expect more than {action.ExpectedArguments.Length} arguments.");
        }

        var arg = action.ExpectedArguments[index];
        var argType = arg.GetType();
        
        if (!_converters.TryGetValue(argType, out var converter))
        {
            return rs.AddInternal($"No converter for {argType} found.");
        }
        
        IArgEvalRes evaluator = converter(token, arg);
        if (!evaluator.IsStatic)
        {
            Logger.Debug("Argument is dynamic, cannot check if fully valid.");
            
            skeleton = new()
            {
                Evaluator = evaluator,
                ArgumentType = argType,
                Name = arg.Name,
            };
            return true;
        }

        var res = evaluator.GetResult();
        if (res.HasErrored())
        {
            return rs.AddExternal(res);
        }
        
        skeleton = new()
        {
            Evaluator = evaluator,
            ArgumentType = argType,
            Name = arg.Name,
        };

        return true;
    }
}