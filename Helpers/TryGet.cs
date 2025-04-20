using System.Diagnostics.Contracts;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodAPI.Exceptions;

namespace ScriptedEventsAPI.Helpers;

public class TryGet<TValue>(TValue? value, string errorMsg) where TValue : class?
{
    public TValue? Value => value;
    public bool WasSuccess => value is not null;
    public string ErrorMsg => errorMsg;

    [Pure]
    public bool HasErrored(out string error)
    {
        error = ErrorMsg;
        return !WasSuccess;
    }

    [Pure]
    public bool HasErrored(out string error, out TValue? val)
    {
        error = ErrorMsg;
        val = Value;
        return !WasSuccess;
    }

    [Pure]
    public static implicit operator bool(TryGet<TValue> result)
    {
        return result.WasSuccess;
    }

    [Pure]
    public static implicit operator string(TryGet<TValue> result)
    {
        return result.ErrorMsg;
    }

    [Pure]
    public static implicit operator TryGet<TValue>(TValue value)
    {
        return new TryGet<TValue>(value, string.Empty);
    }

    [Pure]
    public static implicit operator TryGet<TValue>(Result res)
    {
        if (res.HasErrored(out var msg)) return new TryGet<TValue>(null, msg);

        throw new DeveloperFuckupException("implicit operator TryGet<TValue>(Result res) called when not errored");
    }

    [Pure]
    public static implicit operator TryGet<TValue>(string msg)
    {
        return new TryGet<TValue>(null, msg);
    }
}