namespace ScriptedEventsAPI.Helpers.Flee.Parsing;

/**
 * The token match status. This class contains logic to ensure that
 * only the longest match is considered.
 */
internal class TokenMatch
{
    public int Length { get; private set; } = 0;

    public TokenPattern Pattern { get; private set; } = null;

    public void Clear()
    {
        Length = 0;
        Pattern = null;
    }

    public void Update(int length, TokenPattern pattern)
    {
        if (Length < length)
        {
            Length = length;
            Pattern = pattern;
        }
    }
}