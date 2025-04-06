namespace ScriptedEventsAPI.Helpers.ResultStructure;

public readonly struct Result(bool wasSuccess, string errorMsg)
{
    public readonly bool WasSuccess = wasSuccess;
    public readonly string ErrorMsg = errorMsg;

    public bool HasErrored(out string error)
    {
        error = ErrorMsg;
        return !WasSuccess;
    }
    
    public bool HasErrored()
    {
        return !WasSuccess;
    }

    public static implicit operator bool(Result result)
    {
        return result.WasSuccess;
    }
    
    public static implicit operator string(Result result)
    {
        return result.ErrorMsg;
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