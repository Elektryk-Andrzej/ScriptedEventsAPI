namespace ScriptedEventsAPI.ActionAPI.BaseActions;

public abstract class TextReturningStandardAction : StandardAction
{
    public string? TextReturn { get; protected set; }
}