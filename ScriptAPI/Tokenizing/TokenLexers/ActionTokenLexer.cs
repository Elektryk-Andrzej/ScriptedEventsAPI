using System;
using ScriptedEventsAPI.ActionAPI;
using ScriptedEventsAPI.ActionAPI.BaseActions;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.TokenLexers;

public class ActionTokenLexer(char initChar, Script scr) : BaseTokenLexer(initChar)
{
    public override BaseToken Token { get; set; } = new ActionToken();

    protected override bool IsNotCompleted(char c)
    {
        return char.IsLetter(c);
    }

    public override Result IsFinalStateValid()
    {
        if (!ActionIndex.NameToActionIndex.TryGetValue(Token.RawRepresentation, out var actType))
        {
            return $"There is no action named '{Token.RawRepresentation}'.";
        }

        if (Activator.CreateInstance(actType) is not BaseAction createdAction)
        {
            return $"Action '{Token.RawRepresentation}' couldn't be created.";
        }

        createdAction.Script = scr;
        ((ActionToken)Token).Action = createdAction;
        return true;
    }
}