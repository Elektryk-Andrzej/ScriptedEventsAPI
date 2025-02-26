using ScriptedEventsAPI.ActionAPI;
using ScriptedEventsAPI.ActionAPI.Actions;
using ScriptedEventsAPI.Other;
using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI.Contexts;

public class ActionContext(ActionToken actionToken) : BaseContext
{
    public readonly BaseAction Action = actionToken.Action;
    public readonly ActionArgumentProcessor Processor = new(actionToken.Action);

    public override bool TryAddToken(BaseToken token)
    {
        if (!Processor.IsValidArgument(token, Action.ArgumentsProvided.Count, out var argument))
        {
            Log.Debug($"{token.Name} is not a valid argument, ending action parsing");
            return false;
        }
        
        Log.Debug($"{token.Name} is valid, adding it to ArgumentsProvided of {Action.Name}");
        argument.SetValueWith(token);
        Action.ArgumentsProvided.Add(argument);
        return true;
    }

    public override void Execute()
    {
        Action.Execute();
    }
}