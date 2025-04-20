using System;
using System.Collections;
using System.IO;
using System.Text;

namespace ScriptedEventsAPI.Helpers.Flee.Parsing;

[Obsolete(" A base parser class. This class provides the standard parser interface, as well as token handling.")]
internal abstract class Parser
{
    private readonly Hashtable _patternIds = new();
    private readonly ArrayList _patterns = new();
    private readonly ArrayList _tokens = new();
    private ParserLogException _errorLog = new();
    private int _errorRecovery = -1;
    private bool _initialized;

    /// <summary>
    ///     Creates a new parser.
    /// </summary>
    /// <param name="input"></param>
    internal Parser(TextReader input) : this(input, null)
    {
    }

    /// <summary>
    ///     Creates a new parser.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="analyzer"></param>
    internal Parser(TextReader input, Analyzer analyzer)
    {
        Tokenizer = NewTokenizer(input);
        Analyzer = analyzer ?? NewAnalyzer();
    }

    /**
     * Creates a new parser.
     * 
     * @param tokenizer       the tokenizer to use
     */
    internal Parser(Tokenizer tokenizer) : this(tokenizer, null)
    {
    }

    internal Parser(Tokenizer tokenizer, Analyzer analyzer)
    {
        Tokenizer = tokenizer;
        Analyzer = analyzer ?? NewAnalyzer();
    }

    public Tokenizer Tokenizer { get; }

    public Analyzer Analyzer { get; private set; }

    protected virtual Tokenizer NewTokenizer(TextReader input)
    {
        // TODO: This method should really be abstract, but it isn't in this
        //       version due to backwards compatibility requirements.
        return new Tokenizer(input);
    }

    protected virtual Analyzer NewAnalyzer()
    {
        // TODO: This method should really be abstract, but it isn't in this
        //       version due to backwards compatibility requirements.
        return new Analyzer();
    }

    public Tokenizer GetTokenizer()
    {
        return Tokenizer;
    }

    public Analyzer GetAnalyzer()
    {
        return Analyzer;
    }

    internal void SetInitialized(bool initialized)
    {
        _initialized = initialized;
    }

    public virtual void AddPattern(ProductionPattern pattern)
    {
        if (pattern.Count <= 0)
            throw new ParserCreationException(
                ParserCreationException.ErrorType.INVALID_PRODUCTION,
                pattern.Name,
                "no production alternatives are present (must have at " +
                "least one)");
        if (_patternIds.ContainsKey(pattern.Id))
            throw new ParserCreationException(
                ParserCreationException.ErrorType.INVALID_PRODUCTION,
                pattern.Name,
                "another pattern with the same id (" + pattern.Id +
                ") has already been added");
        _patterns.Add(pattern);
        _patternIds.Add(pattern.Id, pattern);
        SetInitialized(false);
    }

    public virtual void Prepare()
    {
        if (_patterns.Count <= 0)
            throw new ParserCreationException(
                ParserCreationException.ErrorType.INVALID_PARSER,
                "no production patterns have been added");
        for (var i = 0; i < _patterns.Count; i++) CheckPattern((ProductionPattern)_patterns[i]);
        SetInitialized(true);
    }

    private void CheckPattern(ProductionPattern pattern)
    {
        for (var i = 0; i < pattern.Count; i++) CheckAlternative(pattern.Name, pattern[i]);
    }

    private void CheckAlternative(string name,
        ProductionPatternAlternative alt)
    {
        for (var i = 0; i < alt.Count; i++) CheckElement(name, alt[i]);
    }


    private void CheckElement(string name,
        ProductionPatternElement elem)
    {
        if (elem.IsProduction() && GetPattern(elem.Id) == null)
            throw new ParserCreationException(
                ParserCreationException.ErrorType.INVALID_PRODUCTION,
                name,
                "an undefined production pattern id (" + elem.Id +
                ") is referenced");
    }

    public void Reset(TextReader input)
    {
        Tokenizer.Reset(input);
        Analyzer.Reset();
    }

    public void Reset(TextReader input, Analyzer analyzer)
    {
        Tokenizer.Reset(input);
        Analyzer = analyzer;
    }

    public Node Parse()
    {
        Node root = null;

        // Initialize parser
        if (!_initialized) Prepare();
        _tokens.Clear();
        _errorLog = new ParserLogException();
        _errorRecovery = -1;

        // Parse input
        try
        {
            root = ParseStart();
        }
        catch (ParseException e)
        {
            AddError(e, true);
        }

        // Check for errors
        if (_errorLog.Count > 0) throw _errorLog;

        return root;
    }

    protected abstract Node ParseStart();

    protected virtual Production NewProduction(ProductionPattern pattern)
    {
        return Analyzer.NewProduction(pattern);
    }

    internal void AddError(ParseException e, bool recovery)
    {
        if (_errorRecovery <= 0) _errorLog.AddError(e);
        if (recovery) _errorRecovery = 3;
    }

    internal ProductionPattern GetPattern(int id)
    {
        return (ProductionPattern)_patternIds[id];
    }

    internal ProductionPattern GetStartPattern()
    {
        if (_patterns.Count <= 0) return null;

        return (ProductionPattern)_patterns[0];
    }

    internal ICollection GetPatterns()
    {
        return _patterns;
    }

    internal void EnterNode(Node node)
    {
        if (!node.IsHidden() && _errorRecovery < 0)
            try
            {
                Analyzer.Enter(node);
            }
            catch (ParseException e)
            {
                AddError(e, false);
            }
    }

    internal Node ExitNode(Node node)
    {
        if (!node.IsHidden() && _errorRecovery < 0)
            try
            {
                return Analyzer.Exit(node);
            }
            catch (ParseException e)
            {
                AddError(e, false);
            }

        return node;
    }

    internal void AddNode(Production node, Node child)
    {
        if (_errorRecovery >= 0)
        {
            // Do nothing
        }
        else if (node.IsHidden())
        {
            node.AddChild(child);
        }
        else if (child != null && child.IsHidden())
        {
            for (var i = 0; i < child.Count; i++) AddNode(node, child[i]);
        }
        else
        {
            try
            {
                Analyzer.Child(node, child);
            }
            catch (ParseException e)
            {
                AddError(e, false);
            }
        }
    }

    internal Token NextToken()
    {
        var token = PeekToken(0);

        if (token != null)
        {
            _tokens.RemoveAt(0);
            return token;
        }

        throw new ParseException(
            ParseException.ErrorType.UNEXPECTED_EOF,
            null,
            Tokenizer.GetCurrentLine(),
            Tokenizer.GetCurrentColumn());
    }

    internal Token NextToken(int id)
    {
        var token = NextToken();

        if (token.Id == id)
        {
            if (_errorRecovery > 0) _errorRecovery--;
            return token;
        }

        var list = new ArrayList(1) { Tokenizer.GetPatternDescription(id) };
        throw new ParseException(
            ParseException.ErrorType.UNEXPECTED_TOKEN,
            token.ToShortString(),
            list,
            token.StartLine,
            token.StartColumn);
    }

    internal Token PeekToken(int steps)
    {
        while (steps >= _tokens.Count)
            try
            {
                var token = Tokenizer.Next();
                if (token == null) return null;

                _tokens.Add(token);
            }
            catch (ParseException e)
            {
                AddError(e, true);
            }

        return (Token)_tokens[steps];
    }

    public override string ToString()
    {
        var buffer = new StringBuilder();

        for (var i = 0; i < _patterns.Count; i++)
        {
            buffer.Append(ToString((ProductionPattern)_patterns[i]));
            buffer.Append("\n");
        }

        return buffer.ToString();
    }

    private string ToString(ProductionPattern prod)
    {
        var buffer = new StringBuilder();
        var indent = new StringBuilder();
        int i;

        buffer.Append(prod.Name);
        buffer.Append(" (");
        buffer.Append(prod.Id);
        buffer.Append(") ");
        for (i = 0; i < buffer.Length; i++) indent.Append(" ");
        buffer.Append("= ");
        indent.Append("| ");
        for (i = 0; i < prod.Count; i++)
        {
            if (i > 0) buffer.Append(indent);
            buffer.Append(ToString(prod[i]));
            buffer.Append("\n");
        }

        for (i = 0; i < prod.Count; i++)
        {
            var set = prod[i].LookAhead;
            if (set.GetMaxLength() > 1)
            {
                buffer.Append("Using ");
                buffer.Append(set.GetMaxLength());
                buffer.Append(" token look-ahead for alternative ");
                buffer.Append(i + 1);
                buffer.Append(": ");
                buffer.Append(set.ToString(Tokenizer));
                buffer.Append("\n");
            }
        }

        return buffer.ToString();
    }

    private string ToString(ProductionPatternAlternative alt)
    {
        var buffer = new StringBuilder();

        for (var i = 0; i < alt.Count; i++)
        {
            if (i > 0) buffer.Append(" ");
            buffer.Append(ToString(alt[i]));
        }

        return buffer.ToString();
    }

    private string ToString(ProductionPatternElement elem)
    {
        var buffer = new StringBuilder();
        var min = elem.MinCount;
        var max = elem.MaxCount;

        if (min == 0 && max == 1) buffer.Append("[");
        if (elem.IsToken())
            buffer.Append(GetTokenDescription(elem.Id));
        else
            buffer.Append(GetPattern(elem.Id).Name);
        if (min == 0 && max == 1)
        {
            buffer.Append("]");
        }
        else if (min == 0 && max == int.MaxValue)
        {
            buffer.Append("*");
        }
        else if (min == 1 && max == int.MaxValue)
        {
            buffer.Append("+");
        }
        else if (min != 1 || max != 1)
        {
            buffer.Append("{");
            buffer.Append(min);
            buffer.Append(",");
            buffer.Append(max);
            buffer.Append("}");
        }

        return buffer.ToString();
    }

    internal string GetTokenDescription(int token)
    {
        if (Tokenizer == null) return "";

        return Tokenizer.GetPatternDescription(token);
    }
}