using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Contexts.VariableDefinition;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Structures;

namespace SER.ScriptSystem.TokenSystem.Tokens;

public class LiteralVariableToken(Script scr) : BaseContextableToken(scr), IUseBrackets
{
    private int _openBrackets = 0;

    public string NameWithoutBraces =>
        RawRepresentation.Substring(1, RawRepresentation.Length - 2);

    public char OpeningBracket => '{';
    public char ClosingBracket => '}';

    protected override void OnAddingChar(char c)
    {
        if (c == OpeningBracket) _openBrackets++;
        else if (c == ClosingBracket) _openBrackets--;
    }

    public override bool EndParsingOnChar(char c)
    {
        return _openBrackets == 0;
    }

    public override Result IsValidSyntax()
    {
        if (_openBrackets != 0) 
            return $"Variable '{RawRepresentation}' is not fully closed (open {_openBrackets}).";

        if (RawRepresentation.Length <= 2) 
            return $"Variable '{RawRepresentation}' does not have a name.";

        return true;
    }

    public override TryGet<BaseContext> TryGetResultingContext()
    {
        return new LiteralVariableDefinitionContext(this, Script);
    }
}