using ScriptedEventsAPI.ActionAPI.BaseActions;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

public class ActionToken : BaseToken
{
    public BaseAction? Action { get; set; } = null;
}