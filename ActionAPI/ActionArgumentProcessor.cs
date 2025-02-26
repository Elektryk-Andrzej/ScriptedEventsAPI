using System.Linq;
using ScriptedEventsAPI.ActionAPI.ActionArguments;
using ScriptedEventsAPI.ActionAPI.Actions;
using ScriptedEventsAPI.Other;
using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.ActionAPI;

public class ActionArgumentProcessor(BaseAction action)
{
    public BaseAction Action { get; set; } = action;

    public bool IsValidArgument(BaseToken value, int index, out BaseActionArgument argument)
    {
        if (index >= Action.RequiredArguments.Length)
        {
            Log.Debug("Provided index is too big.");
            argument = default!;
            return false;
        }
        
        argument = Action.RequiredArguments[index];
        return argument.CanConvert(value);
    }
}