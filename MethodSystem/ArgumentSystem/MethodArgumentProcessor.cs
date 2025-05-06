using System;
using System.Collections.Generic;
using System.Reflection;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Interfaces;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Structures;
using ScriptedEventsAPI.MethodSystem.BaseMethods;
using ScriptedEventsAPI.ScriptSystem;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;

namespace ScriptedEventsAPI.MethodSystem.ArgumentSystem;

public class MethodArgumentProcessor(BaseMethod method, Script scr)
{
    private class ConverterInfo(MethodInfo method, bool isStatic)
    {
        private MethodInfo Method { get; } = method;
        private bool IsStatic { get; } = isStatic;
        
        public IArgEvalRes Invoke(BaseToken token, Script script, BaseMethodArgument arg)
        {
            object[] parameters = [token, script];
            object? target = IsStatic ? null : arg;
            
            return (IArgEvalRes)Method.Invoke(target, parameters);
        }
    }
    
    private static readonly Dictionary<Type, ConverterInfo> ConverterCache = new();
    
    private static ConverterInfo GetConverterInfo(Type argType)
    {
        if (ConverterCache.TryGetValue(argType, out var cachedInfo))
        {
            return cachedInfo;
        }
        
        var instanceMethod = argType.GetMethod("GetConvertSolution", 
            BindingFlags.Public | BindingFlags.Instance,
            null, [typeof(BaseToken), typeof(Script)], null);
            
        if (instanceMethod != null)
        {
            var converterInfo = new ConverterInfo(instanceMethod, false);
            ConverterCache[argType] = converterInfo;
            return converterInfo;
        }
        
        var staticMethod = argType.GetMethod("GetConvertSolution", 
            BindingFlags.Public | BindingFlags.Static,
            null, [typeof(BaseToken), typeof(Script)], null);

        if (staticMethod == null) 
            throw new InvalidOperationException($"No suitable GetConvertSolution method found for {argType.Name}.");
        
        // cursed
        {
            var converterInfo = new ConverterInfo(staticMethod, true);
            ConverterCache[argType] = converterInfo;
            return converterInfo;
        }

    }

    public Result IsValidArgument(BaseToken token, int index, out ArgumentSkeleton skeleton)
    {
        var rs = new ResultStacker(
            $"Argument '{token.RawRepresentation}' (index {index}) for method {method.Name} is invalid.");

        skeleton = default;

        if (index >= method.ExpectedArguments.Length)
            return rs.Add(
                $"Method does not expect more than {method.ExpectedArguments.Length} arguments.");

        var arg = method.ExpectedArguments[index];
        var argType = arg.GetType();
        
        var evaluator = GetConverterInfo(argType).Invoke(token, scr, arg);
        
        if (!evaluator.IsStatic)
        {
            Logger.Debug("Argument is dynamic, cannot check if fully valid.");

            skeleton = new()
            {
                Evaluator = evaluator,
                ArgumentType = argType,
                Name = arg.Name
            };
            
            return true;
        }

        var res = evaluator.GetResult();
        if (res.HasErrored())
        {
            Logger.Debug($"value of resMsg: {res.ErrorMsg}");
            return rs.Add(res);
        }

        skeleton = new()
        {
            Evaluator = evaluator,
            ArgumentType = argType,
            Name = arg.Name
        };

        return true;
    }
}