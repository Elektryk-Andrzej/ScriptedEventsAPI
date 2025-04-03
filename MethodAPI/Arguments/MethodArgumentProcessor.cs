using System;
using System.Collections.Generic;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.Arguments.Interfaces;
using ScriptedEventsAPI.MethodAPI.Arguments.Structures;
using ScriptedEventsAPI.MethodAPI.BaseMethods;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.MethodAPI.Arguments;

public class MethodArgumentProcessor(BaseMethod method, Script scr)
{
    private readonly Dictionary<Type, Func<BaseToken, BaseMethodArgument, IArgEvalRes>> _converters = new()
    {
        { typeof(TextArgument), (token, _) => TextArgument.GetConvertSolution(token, scr) },
        { typeof(DurationArgument), (token, _) => DurationArgument.GetConvertSolution(token, scr) },
        { typeof(PlayerVariableArgument), (token, _) => PlayerVariableArgument.GetConvertSolution(token, scr) },
        { typeof(EnumArgument), (token, arg) => ((EnumArgument)arg).GetConvertSolution(token, scr) },
        { typeof(SinglePlayerArgument), (token, _) => SinglePlayerArgument.GetConvertSolution(token, scr) },
        { typeof(OptionsArgument), (token, arg) => ((OptionsArgument)arg).GetConvertSolution(token, scr) },
        { typeof(ConditionArgument), (token, arg) => ((ConditionArgument)arg).GetConvertSolution(token, scr) },
    };

    public Result IsValidArgument(BaseToken token, int index, out ArgumentSkeleton skeleton)
    {
        var rs = new ResultStacker(
            $"Argument '{token.RawRepresentation}' (index {index}) for method {method.Name} is invalid!");
        
        skeleton = default;
        
        if (index >= method.ExpectedArguments.Length)
        {
            return rs.AddInternal(
                $"Method does not expect more than {method.ExpectedArguments.Length} arguments.");
        }

        var arg = method.ExpectedArguments[index];
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
            Logger.Debug($"value of resMsg: {res.ErrorMsg}");
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