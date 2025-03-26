using System;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;

public class DurationArgument(string name) : BaseActionArgument(name)
{
    public TimeSpan Value { get; set; } = TimeSpan.Zero;
}