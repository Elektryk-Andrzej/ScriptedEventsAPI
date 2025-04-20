using System.Collections;
using System.Text;

namespace ScriptedEventsAPI.Helpers.Flee.Parsing;

/**
 * A production pattern. This class represents a set of production
 * alternatives that together forms a single production. A
 * production pattern is identified by an integer id and a name,
 * both provided upon creation. The pattern id is used for
 * referencing the production pattern from production pattern
 * elements.
 */
internal class ProductionPattern
{
    private readonly ArrayList _alternatives;

    private int _defaultAlt;

    public ProductionPattern(int id, string name)
    {
        Id = id;
        Name = name;
        Synthetic = false;
        _alternatives = new ArrayList();
        _defaultAlt = -1;
        LookAhead = null;
    }

    public int Id { get; }

    public string Name { get; }

    public bool Synthetic { get; set; }

    internal LookAheadSet LookAhead { get; set; }

    internal ProductionPatternAlternative DefaultAlternative
    {
        get
        {
            if (_defaultAlt >= 0)
            {
                var obj = _alternatives[_defaultAlt];
                return (ProductionPatternAlternative)obj;
            }

            return null;
        }
        set
        {
            _defaultAlt = 0;
            for (var i = 0; i < _alternatives.Count; i++)
                if (_alternatives[i] == value)
                    _defaultAlt = i;
        }
    }

    public int Count => _alternatives.Count;

    public ProductionPatternAlternative this[int index] => (ProductionPatternAlternative)_alternatives[index];

    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return Name;
    }

    public bool IsSyntetic()
    {
        return Synthetic;
    }

    public void SetSyntetic(bool synthetic)
    {
        Synthetic = synthetic;
    }

    public int GetAlternativeCount()
    {
        return Count;
    }

    public ProductionPatternAlternative GetAlternative(int pos)
    {
        return this[pos];
    }

    public bool IsLeftRecursive()
    {
        ProductionPatternAlternative alt;

        for (var i = 0; i < _alternatives.Count; i++)
        {
            alt = (ProductionPatternAlternative)_alternatives[i];
            if (alt.IsLeftRecursive()) return true;
        }

        return false;
    }

    public bool IsRightRecursive()
    {
        ProductionPatternAlternative alt;

        for (var i = 0; i < _alternatives.Count; i++)
        {
            alt = (ProductionPatternAlternative)_alternatives[i];
            if (alt.IsRightRecursive()) return true;
        }

        return false;
    }

    public bool IsMatchingEmpty()
    {
        ProductionPatternAlternative alt;

        for (var i = 0; i < _alternatives.Count; i++)
        {
            alt = (ProductionPatternAlternative)_alternatives[i];
            if (alt.IsMatchingEmpty()) return true;
        }

        return false;
    }

    public void AddAlternative(ProductionPatternAlternative alt)
    {
        if (_alternatives.Contains(alt))
            throw new ParserCreationException(
                ParserCreationException.ErrorType.INVALID_PRODUCTION,
                Name,
                "two identical alternatives exist");
        alt.SetPattern(this);
        _alternatives.Add(alt);
    }

    public override string ToString()
    {
        var buffer = new StringBuilder();
        var indent = new StringBuilder();
        int i;

        buffer.Append(Name);
        buffer.Append("(");
        buffer.Append(Id);
        buffer.Append(") ");
        for (i = 0; i < buffer.Length; i++) indent.Append(" ");
        for (i = 0; i < _alternatives.Count; i++)
        {
            if (i == 0)
            {
                buffer.Append("= ");
            }
            else
            {
                buffer.Append("\n");
                buffer.Append(indent);
                buffer.Append("| ");
            }

            buffer.Append(_alternatives[i]);
        }

        return buffer.ToString();
    }
}