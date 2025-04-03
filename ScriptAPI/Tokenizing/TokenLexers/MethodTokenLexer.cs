using System;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodAPI;
using ScriptedEventsAPI.MethodAPI.BaseMethods;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.TokenLexers;

public class MethodTokenLexer(char initChar, Script scr, BaseToken? previousToken) : BaseTokenLexer(initChar)
{
    public override BaseToken Token { get; protected set; } = new MethodToken();

    protected override bool IsNotCompleted(char c)
    {
        return char.IsLetter(c);
    }

    public override Result IsValid()
    {
        // if the method is not the first token and is not following an "=", it means that its not an method
        if (previousToken is not null and not { RawRepresentation: "=" })
        {
            Token = new UnclassifiedValueToken()
            {
                RawCharRepresentation = Token.RawCharRepresentation,
            };
            
            return true;
        }
        
        if (!MethodIndex.NameToMethodIndex.TryGetValue(Token.RawRepresentation, out var type))
        {
            return $"There is no method named '{Token.RawRepresentation}'.";
        }

        if (Activator.CreateInstance(type) is not BaseMethod createdMethod)
        {
            return $"Method '{Token.RawRepresentation}' couldn't be created.";
        }

        createdMethod.Script = scr;
        ((MethodToken)Token).Method = createdMethod;
        return true;
    }
}