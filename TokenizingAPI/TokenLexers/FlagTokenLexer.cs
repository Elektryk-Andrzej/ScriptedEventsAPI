using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI.TokenLexers;

public class FlagTokenLexer : BaseTokenLexer
{
    private readonly char[] _structure = ['!', '-', '-'];
    
    public override BaseToken Token { get; set; } = new FlagToken();

    protected override bool IsValid(char c)
    {
        var len = Token.Representation.Count;
        return len < _structure.Length && _structure[len] == c;
    }
}