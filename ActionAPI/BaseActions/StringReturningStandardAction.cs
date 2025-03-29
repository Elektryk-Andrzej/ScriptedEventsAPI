namespace ScriptedEventsAPI.ActionAPI.BaseActions;

public abstract class StringReturningStandardAction : StandardAction
{
    public string? Result { get; set; }
}