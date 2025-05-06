using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.BaseContexts;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.Contexts.VariableDefinition;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;

namespace ScriptedEventsAPI.ScriptSystem.TokenSystem.Tokens;

public class PlayerVariableToken(Script scr) : BaseContextableToken(scr)
{
    public string NameWithoutPrefix => RawRepresentation.Substring(1);

    public override bool EndParsingOnChar(char c)
    {
        return !char.IsLetter(c);
    }

    public override Result IsValidSyntax()
    {
        return Result.Assert(RawRepresentation.Length > 1,
            $"Player variable must have have the the prefix '@' and a name, " +
            $"but '{RawRepresentation}' does not satisfy that.");
    }

    public override TryGet<BaseContext> TryGetResultingContext()
    {
        return new PlayerVariableDefinitionContext(this, scr);
    }
}