using System.IO;

namespace ScriptedEventsAPI.Helpers.Flee.Parsing;

internal class CombineElement : Element
{
    private readonly Element _elem1;
    private readonly Element _elem2;

    public CombineElement(Element first, Element second)
    {
        _elem1 = first;
        _elem2 = second;
    }

    public override object Clone()
    {
        return new CombineElement(_elem1, _elem2);
    }

    public override int Match(Matcher m,
        ReaderBuffer buffer,
        int start,
        int skip)
    {
        var length1 = -1;
        var length2 = 0;
        var skip1 = 0;
        var skip2 = 0;

        while (skip >= 0)
        {
            length1 = _elem1.Match(m, buffer, start, skip1);
            if (length1 < 0) return -1;
            length2 = _elem2.Match(m, buffer, start + length1, skip2);
            if (length2 < 0)
            {
                skip1++;
                skip2 = 0;
            }
            else
            {
                skip2++;
                skip--;
            }
        }

        return length1 + length2;
    }

    public override void PrintTo(TextWriter output, string indent)
    {
        _elem1.PrintTo(output, indent);
        _elem2.PrintTo(output, indent);
    }
}