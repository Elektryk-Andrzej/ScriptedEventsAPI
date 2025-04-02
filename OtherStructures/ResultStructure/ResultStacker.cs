namespace ScriptedEventsAPI.OtherStructures.ResultStructure;

public readonly struct ResultStacker(string initMsg)
{
    public Result AddExternal(Result other)
    {
        return !other.HasErrored() 
            ? other 
            : $"Error '{other.ErrorMsg}' has caused another error:\n-> '{initMsg}'";
    }

    public Result AddInternal(Result other)
    {
        return !other.HasErrored()
            ? other
            : $"'{initMsg}'; {other.ErrorMsg}";
    }
}