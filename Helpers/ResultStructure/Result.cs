using SER.MethodSystem.Exceptions;

namespace SER.Helpers.ResultStructure;

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
        if (res == false)
            throw new DeveloperFuckupException("Result cannot be returned as false without an error message.");

        return new Result(res, string.Empty);
    }

    public static implicit operator Result(string msg)
    {
        if (string.IsNullOrEmpty(msg))
            throw new DeveloperFuckupException("Result error message cannot be null or empty.");

        return new Result(false, msg);
    }

    public static Result Assert(bool successWhen, string errorMsg)
    {
        if (successWhen) return true;

        return errorMsg;
    }
}