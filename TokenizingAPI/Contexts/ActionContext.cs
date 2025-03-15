using System.Collections.Generic;
using MEC;
using ScriptedEventsAPI.ActionAPI;
using ScriptedEventsAPI.ActionAPI.Actions;
using ScriptedEventsAPI.ActionAPI.Actions.BaseActions;
using ScriptedEventsAPI.Other;
using ScriptedEventsAPI.TokenizingAPI.Contexts.Structures;
using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI.Contexts;

public class ActionContext(ActionToken actionToken) : BaseContext
{
    public readonly BaseAction Action = actionToken.Action!;
    public readonly ActionArgumentProcessor Processor = new(actionToken.Action!);

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        if (!Processor.IsValidArgument(token, Action.ArgsProvided.Count, out var argument))
        {
            return TryAddTokenRes.Error($"{token.Name} is not a valid argument, ending action parsing");
        }
        
        Log.Debug($"{argument.Name} is valid, adding it to ArgsProvided of {Action.Name}");
        Action.ArgsProvided.Add(argument);
        return TryAddTokenRes.Continue();
    }

    public override IEnumerator<float> Execute()
    {
        Log.Debug($"{Action.Name} context is executing.");

        switch (Action)
        {
            case StandardAction stdAct:
                stdAct.Execute();
                yield break;
            case YieldingAction yieldAct:
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(yieldAct.Execute()));
                yield break;
        }
    }
}