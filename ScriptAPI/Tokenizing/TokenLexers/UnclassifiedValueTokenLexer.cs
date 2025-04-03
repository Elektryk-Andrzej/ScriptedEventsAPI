using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.TokenLexers;

public class UnclassifiedValueTokenLexer(char initChar) : BaseTokenLexer(initChar)
{
    public override BaseToken Token { get; protected set; } = new UnclassifiedValueToken();
    protected override bool IsNotCompleted(char c) => !char.IsWhiteSpace(c);

    public override Result IsValid()
    {
        if (Token.RawRepresentation == "=")
        {
            Token = new VariableDefinitionToken
            {
                RawCharRepresentation = ['=']
            };
        }

        return true;
    }
}