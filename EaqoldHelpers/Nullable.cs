using System;

namespace ScriptedEventsAPI.EaqoldHelpers;

public struct Nullable<T> where T : struct
{
    public T? InternalValue { get; set; } = null;
    
    public Nullable(T? value)
    {
        InternalValue = value;
    }

    public bool HasValue() => HasValue(out _);

    public bool HasValue(out T c)
    {
        c = InternalValue ?? default;
        return InternalValue is not null;
    }
    
    public T Value => InternalValue ?? throw new NullReferenceException($"Requested value of {nameof(T)}, but it's null.");

    public static implicit operator bool(Nullable<T> nullable)
    {
        return nullable.InternalValue is null;
    }
    
    public static implicit operator Nullable<T>(T? value)
    {
        return new(value);
    }
    
    public static implicit operator T?(Nullable<T> nullable)
    {
        return nullable.InternalValue;
    }
}
