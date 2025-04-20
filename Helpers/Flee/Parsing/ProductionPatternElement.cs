using System.Text;

namespace ScriptedEventsAPI.Helpers.Flee.Parsing;

/**
 * A production pattern element. This class represents a reference to
 * either a token or a production. Each element also contains minimum
 * and maximum occurence counters, controlling the number of
 * repetitions allowed. A production pattern element is always
 * contained within a production pattern rule.
 */
internal class ProductionPatternElement
{
    private readonly bool _token;

    public ProductionPatternElement(bool isToken,
        int id,
        int min,
        int max)
    {
        _token = isToken;
        Id = id;
        if (min < 0) min = 0;
        MinCount = min;
        if (max <= 0)
            max = int.MaxValue;
        else if (max < min) max = min;
        MaxCount = max;
        LookAhead = null;
    }

    public int Id { get; }

    public int MinCount { get; }

    public int MaxCount { get; }

    internal LookAheadSet LookAhead { get; set; }

    public int GetId()
    {
        return Id;
    }

    public int GetMinCount()
    {
        return MinCount;
    }

    public int GetMaxCount()
    {
        return MaxCount;
    }

    public bool IsToken()
    {
        return _token;
    }

    public bool IsProduction()
    {
        return !_token;
    }

    public bool IsMatch(Token token)
    {
        return IsToken() && token != null && token.Id == Id;
    }

    public override bool Equals(object obj)
    {
        if (obj is ProductionPatternElement)
        {
            var elem = (ProductionPatternElement)obj;
            return _token == elem._token
                   && Id == elem.Id
                   && MinCount == elem.MinCount
                   && MaxCount == elem.MaxCount;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Id * 37;
    }

    public override string ToString()
    {
        var buffer = new StringBuilder();

        buffer.Append(Id);
        if (_token)
            buffer.Append("(Token)");
        else
            buffer.Append("(Production)");
        if (MinCount != 1 || MaxCount != 1)
        {
            buffer.Append("{");
            buffer.Append(MinCount);
            buffer.Append(",");
            buffer.Append(MaxCount);
            buffer.Append("}");
        }

        return buffer.ToString();
    }
}