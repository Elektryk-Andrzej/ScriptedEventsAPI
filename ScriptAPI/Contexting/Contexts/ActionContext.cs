using System.Collections.Generic;
using System.Linq;
using MEC;
using ScriptedEventsAPI.ActionAPI.ActionArguments;
using ScriptedEventsAPI.ActionAPI.BaseActions;
using ScriptedEventsAPI.EaqoldHelpers;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Structures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Contexting.Contexts;

public class ActionContext(ActionToken actionToken, Script scr) : YieldingContext
{
    public readonly BaseAction Action = actionToken.Action!;
    public readonly ActionArgumentProcessor Processor = new(actionToken.Action!, scr);

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        if (token is EndLineToken)
        {
            return TryAddTokenRes.End();
        }
        
        Logger.Debug($"Checking if {token} is a valid argument for action {Action.Name}.");
        if (Processor.IsValidArgument(token, Action.Args.Count, out var skeleton).HasErrored(out var error))
        {
            return TryAddTokenRes.Error($"{token.TokenName} ({token.RawRepresentation}) is not a valid argument, reason: {error}");
        }
        
        Logger.Debug($"Adding argument '{skeleton.Name}' ({Action.Args.Count}) of value [{skeleton.Value}] (type {skeleton.Type}) to action '{Action.Name}'.");
        Action.Args.Add(skeleton);
        return TryAddTokenRes.Continue();
    }

    public override Result VerifyCurrentState()
    {
        var requiredArgs = Action.ExpectedArguments.Count(arg => arg.Required);
        var providedArgs = Action.Args.Count;

        return new(
            requiredArgs >= providedArgs, 
            $"Action '{Action.Name}' is missing required arguments! " +
            $"Provided arguments: {providedArgs}, required arguments: {requiredArgs}");
    }

    public override IEnumerator<float> Execute()
    {
        Logger.Debug($"'{Action.Name}' action is now running..");

        switch (Action)
        {
            case StandardAction stdAct:
                stdAct.Execute();
                yield break;
            case YieldingAction yieldAct:
                yield return Timing.WaitUntilDone(yieldAct.Execute().Run());
                yield break;
        }
    }
}