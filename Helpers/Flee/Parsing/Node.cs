using System.Collections;
using System.IO;

namespace ScriptedEventsAPI.Helpers.Flee.Parsing;

/**
 * An abstract parse tree node. This class is inherited by all
 * nodes in the parse tree, i.e. by the token and production
 * classes.
 */
internal abstract class Node
{
    private ArrayList _values;

    public abstract int Id { get; }

    public abstract string Name { get; }

    public virtual int StartLine
    {
        get
        {
            for (var i = 0; i < Count; i++)
            {
                var line = this[i].StartLine;
                if (line >= 0) return line;
            }

            return -1;
        }
    }

    public virtual int StartColumn
    {
        get
        {
            for (var i = 0; i < Count; i++)
            {
                var col = this[i].StartColumn;
                if (col >= 0) return col;
            }

            return -1;
        }
    }

    public virtual int EndLine
    {
        get
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                var line = this[i].EndLine;
                if (line >= 0) return line;
            }

            return -1;
        }
    }

    public virtual int EndColumn
    {
        get
        {
            int col;

            for (var i = Count - 1; i >= 0; i--)
            {
                col = this[i].EndColumn;
                if (col >= 0) return col;
            }

            return -1;
        }
    }

    public Node Parent { get; private set; }

    public virtual int Count => 0;

    public virtual Node this[int index] => null;

    public ArrayList Values
    {
        get
        {
            if (_values == null) _values = new ArrayList();
            return _values;
        }
        set => _values = value;
    }

    internal virtual bool IsHidden()
    {
        return false;
    }

    public virtual int GetId()
    {
        return Id;
    }

    public virtual string GetName()
    {
        return Name;
    }

    public virtual int GetStartLine()
    {
        return StartLine;
    }

    public virtual int GetStartColumn()
    {
        return StartColumn;
    }

    public virtual int GetEndLine()
    {
        return EndLine;
    }

    public virtual int GetEndColumn()
    {
        return EndColumn;
    }

    public Node GetParent()
    {
        return Parent;
    }

    internal void SetParent(Node parent)
    {
        Parent = parent;
    }

    public virtual int GetChildCount()
    {
        return Count;
    }

    public int GetDescendantCount()
    {
        var count = 0;

        for (var i = 0; i < Count; i++) count += 1 + this[i].GetDescendantCount();
        return count;
    }

    public virtual Node GetChildAt(int index)
    {
        return this[index];
    }

    public int GetValueCount()
    {
        if (_values == null) return 0;

        return _values.Count;
    }

    public object GetValue(int pos)
    {
        return Values[pos];
    }

    public ArrayList GetAllValues()
    {
        return _values;
    }


    public void AddValue(object value)
    {
        if (value != null) Values.Add(value);
    }

    public void AddValues(ArrayList values)
    {
        if (values != null) Values.AddRange(values);
    }

    public void RemoveAllValues()
    {
        _values = null;
    }

    public void PrintTo(TextWriter output)
    {
        PrintTo(output, "");
        output.Flush();
    }

    private void PrintTo(TextWriter output, string indent)
    {
        output.WriteLine(indent + ToString());
        indent = indent + "  ";
        for (var i = 0; i < Count; i++) this[i].PrintTo(output, indent);
    }
}