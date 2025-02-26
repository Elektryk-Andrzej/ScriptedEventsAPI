using ScriptedEventsAPI.ActionAPI.Actions;

namespace ScriptedEventsAPI.TokenizingAPI.Tokens;

public class ActionToken : BaseToken
{
    // todo: change after testing
    public BaseAction Action { get; set; } = new PrintAction();
    
    public override void OnFinished()
    {
        // todo: change after testing
    }
}