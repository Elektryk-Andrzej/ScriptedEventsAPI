using ScriptedEventsAPI.ActionAPI.BaseActions;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Contexts;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

public class ActionToken : BaseContextableToken
{
    public BaseAction? Action { get; set; } = null;
    public override BaseContext? GetResultingContext()
    {
        return Action is not null
            ? new ActionContext(this, Action.Script) 
            : null;
    }
}