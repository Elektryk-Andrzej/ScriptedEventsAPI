﻿using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI.Contexting.Structures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;

public abstract class BaseContext
{
    public string Name => GetType().Name;
    public abstract TryAddTokenRes TryAddToken(BaseToken token);
    public override string ToString()
    {
        return Name;
    }

    public abstract Result VerifyCurrentState();
}