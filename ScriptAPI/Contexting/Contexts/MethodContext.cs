using System.Collections.Generic;
using System.Linq;
using MEC;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodAPI.Arguments;
using ScriptedEventsAPI.MethodAPI.BaseMethods;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Structures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Contexting.Contexts;

public class MethodContext(MethodToken methodToken, Script scr) : YieldingContext
{
    public readonly BaseMethod Method = methodToken.Method!;
    public readonly MethodArgumentProcessor Processor = new(methodToken.Method!, scr);

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        if (token is EndLineToken)
        {
            return TryAddTokenRes.End();
        }
        
        Logger.Debug($"Checking if {token} is a valid argument for method {Method.Name}.");
        if (Processor.IsValidArgument(token, Method.Args.Count, out var skeleton).HasErrored(out var error))
        {
            return TryAddTokenRes.Error($"{token.TokenName} ({token.RawRepresentation}) is not a valid argument, reason: {error}");
        }
        
        Logger.Debug($"Adding argument '{skeleton.Name}' (index {Method.Args.Count}) (type {skeleton.ArgumentType.Name}) to method '{Method.Name}'.");
        Method.Args.Add(skeleton);
        return TryAddTokenRes.Continue();
    }

    public override Result VerifyCurrentState()
    {
        var requiredArgs = Method.ExpectedArguments.Count(arg => arg.Required);
        var providedArgs = Method.Args.Count;

        return providedArgs >= requiredArgs
            ? true
            : $"Method '{Method.Name}' is missing required arguments! " +
              $"Provided arguments: {providedArgs}, required arguments: {requiredArgs}";
    }

    public override IEnumerator<float> Execute()
    {
        Logger.Debug($"'{Method.Name}' method is now running..");

        switch (Method)
        {
            case StandardMethod stdAct:
                stdAct.Execute();
                yield break;
            case YieldingMethod yieldAct:
                yield return Timing.WaitUntilDone(yieldAct.Execute().Run());
                yield break;
        }
    }
}