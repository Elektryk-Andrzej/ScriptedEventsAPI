using System;
using System.Linq;
using ScriptedEventsAPI.ActionAPI.Actions;
using ScriptedEventsAPI.ActionAPI.Actions.BaseActions;
using ScriptedEventsAPI.Other;
using ScriptedEventsAPI.Other.OpRes;
using UnityEngine;

namespace ScriptedEventsAPI.TokenizingAPI.Tokens;

public class ActionToken : BaseToken
{
    public BaseAction? Action { get; set; } = null;
}