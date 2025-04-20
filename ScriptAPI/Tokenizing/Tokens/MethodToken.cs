using System;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodAPI;
using ScriptedEventsAPI.MethodAPI.BaseMethods;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Contexts;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Structures;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

public class MethodToken(Script scr) : BaseContextableToken(scr)
{
    private readonly Script _scr = scr;
    public BaseMethod Method { get; set; } = null!;

    public override TryGet<BaseContext> TryGetResultingContext()
    {
        if (!MethodIndex.NameToMethodIndex.TryGetValue(RawRepresentation, out var type))
            return $"There is no method named '{RawRepresentation}'.";

        if (Activator.CreateInstance(type) is not BaseMethod createdMethod)
            return $"Method '{RawRepresentation}' couldn't be created.";

        Method = createdMethod;
        Method.Script = _scr;

        return new MethodContext(this);
    }

    public override bool EndParsingOnChar(char c)
    {
        return char.IsWhiteSpace(c);
    }

    public override Result IsValidSyntax()
    {
        return true;
    }
}