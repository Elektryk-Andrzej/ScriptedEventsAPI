using Exiled.API.Features;
using ScriptedEventsAPI.OtherStructures;

namespace ScriptedEventsAPI.ActionAPI.BaseActions;

public abstract class StringReturningStandardAction : StandardAction
{
    public string? Result { get; set; }
}