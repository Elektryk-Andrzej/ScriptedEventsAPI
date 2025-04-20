using System.IO;

namespace ScriptedEventsAPI.Helpers.Flee.Parsing;

/**
 * A regular expression string matcher. This class handles the
 * matching of a specific string with a specific regular
 * expression. It contains state information about the matching
 * process, as for example the position of the latest match, and a
 * number of flags that were set. This class is not thread-safe.
 */
internal class Matcher
{
    private readonly Element _element;
    private readonly bool _ignoreCase;
    private ReaderBuffer _buffer;
    private bool _endOfString;
    private int _length;
    private int _start;

    internal Matcher(Element e, ReaderBuffer buffer, bool ignoreCase)
    {
        _element = e;
        _buffer = buffer;
        _ignoreCase = ignoreCase;
        _start = 0;
        Reset();
    }

    public bool IsCaseInsensitive()
    {
        return _ignoreCase;
    }

    public void Reset()
    {
        _length = -1;
        _endOfString = false;
    }

    public void Reset(string str)
    {
        Reset(new ReaderBuffer(new StringReader(str)));
    }

    public void Reset(ReaderBuffer buffer)
    {
        _buffer = buffer;
        Reset();
    }

    public int Start()
    {
        return _start;
    }

    public int End()
    {
        if (_length > 0) return _start + _length;

        return _start;
    }

    public int Length()
    {
        return _length;
    }

    public bool HasReadEndOfString()
    {
        return _endOfString;
    }

    public bool MatchFromBeginning()
    {
        return MatchFrom(0);
    }

    public bool MatchFrom(int pos)
    {
        Reset();
        _start = pos;
        _length = _element.Match(this, _buffer, _start, 0);
        return _length >= 0;
    }

    public override string ToString()
    {
        if (_length <= 0) return "";

        return _buffer.Substring(_buffer.Position, _length);
    }

    internal void SetReadEndOfString()
    {
        _endOfString = true;
    }
}