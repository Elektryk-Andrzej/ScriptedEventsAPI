using System;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodSystem;
using ScriptedEventsAPI.MethodSystem.BaseMethods;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.BaseContexts;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.Contexts;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;

namespace ScriptedEventsAPI.ScriptSystem.TokenSystem.Tokens;

public class MethodToken(Script scr) : BaseContextableToken(scr)
{
    public BaseMethod Method { get; set; } = null!;

    public override TryGet<BaseContext> TryGetResultingContext()
    {
        if (!MethodIndex.NameToMethodIndex.TryGetValue(RawRepresentation, out var type))
            return $"There is no method named '{RawRepresentation}'.";

        if (Activator.CreateInstance(type) is not BaseMethod createdMethod)
            return $"Method '{RawRepresentation}' couldn't be created.";

        Method = createdMethod;
        Method.Script = Script;

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