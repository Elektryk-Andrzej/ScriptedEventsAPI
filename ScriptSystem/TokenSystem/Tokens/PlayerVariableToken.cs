using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Contexts.VariableDefinition;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.TokenSystem.Tokens;

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
        return new PlayerVariableDefinitionContext(this, Script);
    }
}