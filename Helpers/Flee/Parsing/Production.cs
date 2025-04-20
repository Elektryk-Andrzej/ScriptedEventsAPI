using System.Collections;

namespace ScriptedEventsAPI.Helpers.Flee.Parsing;

/**
 * A production node. This class represents a grammar production
 * (i.e. a list of child nodes) in a parse tree. The productions
 * are created by a parser, that adds children a according to a
 * set of production patterns (i.e. grammar rules).
 */
internal class Production : Node
{
    private readonly ArrayList _children;

    public Production(ProductionPattern pattern)
    {
        Pattern = pattern;
        _children = new ArrayList();
    }

    public override int Id => Pattern.Id;

    public override string Name => Pattern.Name;

    public override int Count => _children.Count;

    public override Node this[int index]
    {
        get
        {
            if (index < 0 || index >= _children.Count) return null;

            return (Node)_children[index];
        }
    }

    public ProductionPattern Pattern { get; }

    public void AddChild(Node child)
    {
        if (child != null)
        {
            child.SetParent(this);
            _children.Add(child);
        }
    }

    public ProductionPattern GetPattern()
    {
        return Pattern;
    }

    internal override bool IsHidden()
    {
        return Pattern.Synthetic;
    }

    public override string ToString()
    {
        return Pattern.Name + '(' + Pattern.Id + ')';
    }
}