using System;
using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments;

public class DurationArgument(string name) : BaseActionArgument(name)
{
    public TimeSpan Value { get; set; } = TimeSpan.Zero;
}