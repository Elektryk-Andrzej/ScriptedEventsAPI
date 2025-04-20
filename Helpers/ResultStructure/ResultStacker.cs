using System.Diagnostics.Contracts;

namespace ScriptedEventsAPI.Helpers.ResultStructure;

[Pure]
public class ResultStacker(string initMsg)
{
    [Pure]
    public Result AddExt(Result other)
    {
        return !other.HasErrored()
            ? other
            : $"'{other.ErrorMsg}' has caused another error:\n-> '{initMsg}'";
    }

    [Pure]
    public Result AddInt(Result other)
    {
        return other.HasErrored()
            ? $"'{initMsg}'; {other.ErrorMsg}"
            : other;
    }
}