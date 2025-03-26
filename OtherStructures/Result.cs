namespace ScriptedEventsAPI.OtherStructures;

public readonly struct Result(bool res, string message)
{
    public readonly bool Value = res;
    public readonly string Message = message;

    public bool HasErrored(out string error)
    {
        error = Message;
        return !Value;
    }
    
    public bool HasErrored()
    {
        return !Value;
    }

    public static implicit operator bool(Result result)
    {
        return result.Value;
    }

    public static implicit operator Result(bool res)
    {
        return new Result(res, string.Empty);
    }
    
    public static implicit operator Result(string msg)
    {
        return new Result(false, msg);
    }
}