using System.Text;

namespace ScriptedEventsAPI.Helpers.Flee.Parsing;

/**
 * * A token pattern. This class contains the definition of a token
 * * (i.e. it's pattern), and allows testing a string against this
 * * pattern. A token pattern is uniquely identified by an integer id,
 * * that must be provided upon creation.
 * *
 */
internal class TokenPattern
{
    public enum PatternType
    {
        /**
         * The string pattern type is used for tokens that only
         * match an exact string.
         */
        STRING,

        /**
         * The regular expression pattern type is used for tokens
         * that match a regular expression.
         */
        REGEXP
    }

    private bool _error;
    private string _errorMessage;

    private string _ignoreMessage;

    public TokenPattern(int id,
        string name,
        PatternType type,
        string pattern)
    {
        Id = id;
        Name = name;
        Type = type;
        Pattern = pattern;
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public PatternType Type { get; set; }

    public string Pattern { get; set; }

    public bool Error
    {
        get => _error;
        set
        {
            _error = value;
            if (_error && _errorMessage == null) _errorMessage = "unrecognized token found";
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _error = true;
            _errorMessage = value;
        }
    }

    public bool Ignore { get; set; }

    public string IgnoreMessage
    {
        get => _ignoreMessage;
        set
        {
            Ignore = true;
            _ignoreMessage = value;
        }
    }

    public string DebugInfo { get; set; }

    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return Name;
    }

    public PatternType GetPatternType()
    {
        return Type;
    }

    public string GetPattern()
    {
        return Pattern;
    }

    public bool IsError()
    {
        return Error;
    }

    public string GetErrorMessage()
    {
        return ErrorMessage;
    }

    public void SetError()
    {
        Error = true;
    }

    public void SetError(string message)
    {
        ErrorMessage = message;
    }

    public bool IsIgnore()
    {
        return Ignore;
    }

    public string GetIgnoreMessage()
    {
        return IgnoreMessage;
    }


    public void SetIgnore()
    {
        Ignore = true;
    }


    public void SetIgnore(string message)
    {
        IgnoreMessage = message;
    }

    public override string ToString()
    {
        var buffer = new StringBuilder();

        buffer.Append(Name);
        buffer.Append(" (");
        buffer.Append(Id);
        buffer.Append("): ");
        switch (Type)
        {
            case PatternType.STRING:
                buffer.Append("\"");
                buffer.Append(Pattern);
                buffer.Append("\"");
                break;
            case PatternType.REGEXP:
                buffer.Append("<<");
                buffer.Append(Pattern);
                buffer.Append(">>");
                break;
        }

        if (_error)
        {
            buffer.Append(" ERROR: \"");
            buffer.Append(_errorMessage);
            buffer.Append("\"");
        }

        if (Ignore)
        {
            buffer.Append(" IGNORE");
            if (_ignoreMessage != null)
            {
                buffer.Append(": \"");
                buffer.Append(_ignoreMessage);
                buffer.Append("\"");
            }
        }

        if (DebugInfo != null)
        {
            buffer.Append("\n  ");
            buffer.Append(DebugInfo);
        }

        return buffer.ToString();
    }

    public string ToShortString()
    {
        var buffer = new StringBuilder();
        var newline = Pattern.IndexOf('\n');

        if (Type == PatternType.STRING)
        {
            buffer.Append("\"");
            if (newline >= 0)
            {
                if (newline > 0 && Pattern[newline - 1] == '\r') newline--;
                buffer.Append(Pattern.Substring(0, newline));
                buffer.Append("(...)");
            }
            else
            {
                buffer.Append(Pattern);
            }

            buffer.Append("\"");
        }
        else
        {
            buffer.Append("<");
            buffer.Append(Name);
            buffer.Append(">");
        }

        return buffer.ToString();
    }

    public void SetData(int id, string name, PatternType type, string pattern)
    {
        Id = id;
        Name = name;
        Type = type;
        Pattern = pattern;
    }
}