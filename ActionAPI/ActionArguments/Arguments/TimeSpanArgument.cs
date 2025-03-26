using System;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;

public class TimeSpanArgument(string name) : BaseActionArgument(name)
{
    public TimeSpan Value { get; init; }
}