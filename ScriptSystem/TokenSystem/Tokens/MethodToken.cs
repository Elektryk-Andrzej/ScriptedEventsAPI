using System;
using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.MethodSystem;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Contexts;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.TokenSystem.Tokens;

public class MethodToken(Script scr) : BaseContextableToken(scr)
{
    public BaseMethod Method { get; set; } = null!;

    public override TryGet<BaseContext> TryGetResultingContext()
    {
        if (!MethodIndex.NameToMethodIndex.TryGetValue(RawRepresentation, out var method))
            return $"There is no method named '{RawRepresentation}'.";

        if (Activator.CreateInstance(method.GetType()) is not BaseMethod createdMethod)
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