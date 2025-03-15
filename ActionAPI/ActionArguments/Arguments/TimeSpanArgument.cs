using System;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments;

public class TimeSpanArgument(string name) : BaseActionArgument(name)
{
    public TimeSpan Value { get; set; }
}