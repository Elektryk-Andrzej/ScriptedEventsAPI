using System.Text;

namespace ScriptedEventsAPI.Helpers.Flee.Parsing;

/**
 * A token node. This class represents a token (i.e. a set of adjacent
 * characters) in a parse tree. The tokens are created by a tokenizer,
 * that groups characters together into tokens according to a set of
 * token patterns.
 */
internal class Token : Node
{
    private readonly int _endColumn;
    private readonly int _endLine;
    private readonly int _startColumn;
    private readonly int _startLine;
    private Token _next = null;
    private Token _previous = null;

    public Token(TokenPattern pattern, string image, int line, int col)
    {
        Pattern = pattern;
        Image = image;
        _startLine = line;
        _startColumn = col;
        _endLine = line;
        _endColumn = col + image.Length - 1;
        for (var pos = 0; image.IndexOf('\n', pos) >= 0;)
        {
            pos = image.IndexOf('\n', pos) + 1;
            _endLine++;
            _endColumn = image.Length - pos;
        }
    }

    public override int Id => Pattern.Id;

    public override string Name => Pattern.Name;

    public override int StartLine => _startLine;

    public override int StartColumn => _startColumn;

    public override int EndLine => _endLine;

    public override int EndColumn => _endColumn;

    public string Image { get; }

    internal TokenPattern Pattern { get; }

    public Token Previous
    {
        get => _previous;
        set
        {
            if (_previous != null) _previous._next = null;
            _previous = value;
            if (_previous != null) _previous._next = this;
        }
    }

    public Token Next
    {
        get => _next;
        set
        {
            if (_next != null) _next._previous = null;
            _next = value;
            if (_next != null) _next._previous = this;
        }
    }

    public string GetImage()
    {
        return Image;
    }

    public Token GetPreviousToken()
    {
        return Previous;
    }

    public Token GetNextToken()
    {
        return Next;
    }

    public override string ToString()
    {
        var buffer = new StringBuilder();
        var newline = Image.IndexOf('\n');

        buffer.Append(Pattern.Name);
        buffer.Append("(");
        buffer.Append(Pattern.Id);
        buffer.Append("): \"");
        if (newline >= 0)
        {
            if (newline > 0 && Image[newline - 1] == '\r') newline--;
            buffer.Append(Image.Substring(0, newline));
            buffer.Append("(...)");
        }
        else
        {
            buffer.Append(Image);
        }

        buffer.Append("\", line: ");
        buffer.Append(_startLine);
        buffer.Append(", col: ");
        buffer.Append(_startColumn);

        return buffer.ToString();
    }

    public string ToShortString()
    {
        var buffer = new StringBuilder();
        var newline = Image.IndexOf('\n');

        buffer.Append('"');
        if (newline >= 0)
        {
            if (newline > 0 && Image[newline - 1] == '\r') newline--;
            buffer.Append(Image.Substring(0, newline));
            buffer.Append("(...)");
        }
        else
        {
            buffer.Append(Image);
        }

        buffer.Append('"');
        if (Pattern.Type == TokenPattern.PatternType.REGEXP)
        {
            buffer.Append(" <");
            buffer.Append(Pattern.Name);
            buffer.Append(">");
        }

        return buffer.ToString();
    }
}