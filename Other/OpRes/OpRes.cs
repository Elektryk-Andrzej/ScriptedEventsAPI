namespace ScriptedEventsAPI.Other.OpRes;

public struct OpRes(bool res, string message)
{
    public readonly bool Result = res;
    public readonly string Message = message;

    public bool HasErrored(out string error)
    {
        error = Message;
        return !Result;
    }

    public static implicit operator bool(OpRes opRes)
    {
        return opRes.Result;
    }

    public static implicit operator OpRes(bool res)
    {
        return new OpRes(res, string.Empty);
    }
}