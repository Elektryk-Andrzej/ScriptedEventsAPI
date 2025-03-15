using System;
using System.Linq;
using Exiled.API.Features;
using ScriptedEventsAPI.ActionAPI;
using ScriptedEventsAPI.ActionAPI.Actions;
using ScriptedEventsAPI.ActionAPI.Actions.BaseActions;
using ScriptedEventsAPI.Other.OpRes;
using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI.TokenLexers;

public class ActionTokenLexer(char initChar) : BaseTokenLexer(initChar)
{
    public override BaseToken Token { get; set; } = new ActionToken();

    protected override bool IsValid(char c)
    {
        return char.IsLetter(c);
    }

    public override OpRes IsFinalStateValid()
    {
        if (ActionIndex.NameToActionIndex.TryGetValue(Token.AsString, out var act))
        {
            ((ActionToken)Token).Action = Activator.CreateInstance(act) as BaseAction;
            return ((ActionToken)Token).Action is not null;
        }

        return new(false, $"There is no action named {Token.AsString}!");
    }
}