using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.TokenLexers;

public class FlagTokenLexer : BaseTokenLexer
{
    private readonly char[] _structure = ['!', '-', '-'];
    
    public override BaseToken Token { get; set; } = new FlagDefinitionToken();

    protected override bool IsNotCompleted(char c)
    {
        var len = Token.RawCharRepresentation.Count;
        return len < _structure.Length && _structure[len] == c;
    }
}