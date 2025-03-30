﻿using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;

public class PlayerVariableArgument(string name, bool required = true) : BaseActionArgument(name, required)
{
    public static Result TryConvert(BaseToken token, Script scr, out Func<List<Player>> value)
    {
        value = null!;
        
        if (token is not PlayerVariableToken playerVariableToken)
        {
            return $"Player variable argument requires it's value to be of type {nameof(PlayerVariableToken)}, " +
                   $"but got provided with type {token.GetType().Name}.";
        }

        var res = scr.TryGetPlayerVariable(playerVariableToken.NameWithoutPrefix, out var variable);
        value = variable.Players;
        return res;
    }
}