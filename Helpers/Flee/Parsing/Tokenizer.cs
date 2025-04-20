using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ScriptedEventsAPI.Helpers.Flee.Parsing;

/**
 * A character stream tokenizer. This class groups the characters read
 * from the stream together into tokens ("words"). The grouping is
 * controlled by token patterns that contain either a fixed string to
 * search for, or a regular expression. If the stream of characters
 * don't match any of the token patterns, a parse exception is thrown.
 */
internal class Tokenizer
{
    private readonly TokenMatch _lastMatch = new();
    private readonly NFAMatcher _nfaMatcher;
    private readonly RegExpMatcher _regExpMatcher;
    private readonly StringDFAMatcher _stringDfaMatcher;
    private ReaderBuffer _buffer = null;
    private Token _previousToken = null;

    public Tokenizer(TextReader input)
        : this(input, false)
    {
    }

    public Tokenizer(TextReader input, bool ignoreCase)
    {
        _stringDfaMatcher = new StringDFAMatcher(ignoreCase);
        _nfaMatcher = new NFAMatcher(ignoreCase);
        _regExpMatcher = new RegExpMatcher(ignoreCase);
        _buffer = new ReaderBuffer(input);
    }

    public bool UseTokenList { get; set; } = false;

    public bool GetUseTokenList()
    {
        return UseTokenList;
    }

    public void SetUseTokenList(bool useTokenList)
    {
        UseTokenList = useTokenList;
    }

    public string GetPatternDescription(int id)
    {
        var pattern = _stringDfaMatcher.GetPattern(id);
        if (pattern == null) pattern = _nfaMatcher.GetPattern(id);
        if (pattern == null) pattern = _regExpMatcher.GetPattern(id);
        return pattern?.ToShortString();
    }

    public int GetCurrentLine()
    {
        return _buffer.LineNumber;
    }

    public int GetCurrentColumn()
    {
        return _buffer.ColumnNumber;
    }

    /**
     * nfa - true to attempt as an nfa pattern for regexp. This handles most things except the complex repeates, ie {1,4}
     */
    public void AddPattern(TokenPattern pattern, bool nfa = true)
    {
        switch (pattern.Type)
        {
            case TokenPattern.PatternType.STRING:
                try
                {
                    _stringDfaMatcher.AddPattern(pattern);
                }
                catch (Exception e)
                {
                    throw new ParserCreationException(
                        ParserCreationException.ErrorType.INVALID_TOKEN,
                        pattern.Name,
                        "error adding string token: " +
                        e.Message);
                }

                break;
            case TokenPattern.PatternType.REGEXP:
                if (nfa)
                    try
                    {
                        _nfaMatcher.AddPattern(pattern);
                    }
                    catch (Exception)
                    {
                        nfa = false;
                    }

                if (!nfa)
                    try
                    {
                        _regExpMatcher.AddPattern(pattern);
                    }
                    catch (Exception e)
                    {
                        throw new ParserCreationException(
                            ParserCreationException.ErrorType.INVALID_TOKEN,
                            pattern.Name,
                            "regular expression contains error(s): " +
                            e.Message);
                    }

                break;
            default:
                throw new ParserCreationException(
                    ParserCreationException.ErrorType.INVALID_TOKEN,
                    pattern.Name,
                    "pattern type " + pattern.Type +
                    " is undefined");
        }
    }

    public void Reset(TextReader input)
    {
        //this.buffer.Dispose();
        _buffer = new ReaderBuffer(input);
        _previousToken = null;
        _lastMatch.Clear();
    }

    public Token Next()
    {
        Token token = null;

        do
        {
            token = NextToken();
            if (token == null)
            {
                _previousToken = null;
                return null;
            }

            if (UseTokenList)
            {
                token.Previous = _previousToken;
                _previousToken = token;
            }

            if (token.Pattern.Ignore)
                token = null;
            else if (token.Pattern.Error)
                throw new ParseException(
                    ParseException.ErrorType.INVALID_TOKEN,
                    token.Pattern.ErrorMessage,
                    token.StartLine,
                    token.StartColumn);
        } while (token == null);

        return token;
    }

    private Token NextToken()
    {
        try
        {
            _lastMatch.Clear();
            _stringDfaMatcher.Match(_buffer, _lastMatch);
            _nfaMatcher.Match(_buffer, _lastMatch);
            _regExpMatcher.Match(_buffer, _lastMatch);
            int line;
            int column;
            if (_lastMatch.Length > 0)
            {
                line = _buffer.LineNumber;
                column = _buffer.ColumnNumber;
                var str = _buffer.Read(_lastMatch.Length);
                return NewToken(_lastMatch.Pattern, str, line, column);
            }

            if (_buffer.Peek(0) < 0) return null;

            line = _buffer.LineNumber;
            column = _buffer.ColumnNumber;
            throw new ParseException(
                ParseException.ErrorType.UNEXPECTED_CHAR,
                _buffer.Read(1),
                line,
                column);
        }
        catch (IOException e)
        {
            throw new ParseException(ParseException.ErrorType.IO,
                e.Message,
                -1,
                -1);
        }
    }

    protected virtual Token NewToken(TokenPattern pattern,
        string image,
        int line,
        int column)
    {
        return new Token(pattern, image, line, column);
    }

    public override string ToString()
    {
        var buffer = new StringBuilder();
        buffer.Append(_stringDfaMatcher);
        buffer.Append(_nfaMatcher);
        buffer.Append(_regExpMatcher);
        return buffer.ToString();
    }
}

internal abstract class TokenMatcher
{
    protected bool IgnoreCase = false;
    protected TokenPattern[] Patterns = new TokenPattern[0];

    protected TokenMatcher(bool ignoreCase)
    {
        IgnoreCase = ignoreCase;
    }

    public abstract void Match(ReaderBuffer buffer, TokenMatch match);

    public TokenPattern GetPattern(int id)
    {
        for (var i = 0; i < Patterns.Length; i++)
            if (Patterns[i].Id == id)
                return Patterns[i];

        return null;
    }

    public virtual void AddPattern(TokenPattern pattern)
    {
        Array.Resize(ref Patterns, Patterns.Length + 1);
        Patterns[Patterns.Length - 1] = pattern;
    }

    public override string ToString()
    {
        var buffer = new StringBuilder();

        for (var i = 0; i < Patterns.Length; i++)
        {
            buffer.Append(Patterns[i]);
            buffer.Append("\n\n");
        }

        return buffer.ToString();
    }
}

internal class StringDFAMatcher : TokenMatcher
{
    private readonly TokenStringDFA _automaton = new();

    public StringDFAMatcher(bool ignoreCase) : base(ignoreCase)
    {
    }

    public override void AddPattern(TokenPattern pattern)
    {
        _automaton.AddMatch(pattern.Pattern, IgnoreCase, pattern);
        base.AddPattern(pattern);
    }

    public override void Match(ReaderBuffer buffer, TokenMatch match)
    {
        var res = _automaton.Match(buffer, IgnoreCase);

        if (res != null) match.Update(res.Pattern.Length, res);
    }
}

internal class NFAMatcher : TokenMatcher
{
    private readonly TokenNFA _automaton = new();

    public NFAMatcher(bool ignoreCase) : base(ignoreCase)
    {
    }

    public override void AddPattern(TokenPattern pattern)
    {
        if (pattern.Type == TokenPattern.PatternType.STRING)
            _automaton.AddTextMatch(pattern.Pattern, IgnoreCase, pattern);
        else
            _automaton.AddRegExpMatch(pattern.Pattern, IgnoreCase, pattern);
        base.AddPattern(pattern);
    }

    public override void Match(ReaderBuffer buffer, TokenMatch match)
    {
        _automaton.Match(buffer, match);
    }
}

internal class RegExpMatcher : TokenMatcher
{
    private REHandler[] _regExps = new REHandler[0];

    public RegExpMatcher(bool ignoreCase) : base(ignoreCase)
    {
    }

    public override void AddPattern(TokenPattern pattern)
    {
        REHandler re;
        try
        {
            re = new GrammaticaRE(pattern.Pattern, IgnoreCase);
            pattern.DebugInfo = "Grammatica regexp\n" + re;
        }
        catch (Exception)
        {
            re = new SystemRE(pattern.Pattern, IgnoreCase);
            pattern.DebugInfo = "native .NET regexp";
        }

        Array.Resize(ref _regExps, _regExps.Length + 1);
        _regExps[_regExps.Length - 1] = re;
        base.AddPattern(pattern);
    }

    public override void Match(ReaderBuffer buffer, TokenMatch match)
    {
        for (var i = 0; i < _regExps.Length; i++)
        {
            var length = _regExps[i].Match(buffer);
            if (length > 0) match.Update(length, Patterns[i]);
        }
    }
}

internal abstract class REHandler
{
    public abstract int Match(ReaderBuffer buffer);
}

internal class GrammaticaRE : REHandler
{
    private readonly RegExp _regExp;
    private Matcher _matcher = null;

    public GrammaticaRE(string regex, bool ignoreCase)
    {
        _regExp = new RegExp(regex, ignoreCase);
    }

    public override int Match(ReaderBuffer buffer)
    {
        if (_matcher == null)
            _matcher = _regExp.Matcher(buffer);
        else
            _matcher.Reset(buffer);
        return _matcher.MatchFromBeginning() ? _matcher.Length() : 0;
    }
}

internal class SystemRE : REHandler
{
    private readonly Regex _reg;

    public SystemRE(string regex, bool ignoreCase)
    {
        if (ignoreCase)
            _reg = new Regex(regex, RegexOptions.IgnoreCase);
        else
            _reg = new Regex(regex);
    }

    public override int Match(ReaderBuffer buffer)
    {
        Match m;

        // Ugly hack since .NET doesn't have a flag for when the
        // end of the input string was encountered...
        buffer.Peek(1024 * 16);
        // Also, there is no API to limit the search to the specified
        // position, so we double-check the index afterwards instead.
        m = _reg.Match(buffer.ToString(), buffer.Position);
        if (m.Success && m.Index == buffer.Position) return m.Length;

        return 0;
    }
}