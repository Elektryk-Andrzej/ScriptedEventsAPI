﻿using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments;

public abstract class BaseActionArgument(string name)
{
    public string Name { get; private set; } = name;
}