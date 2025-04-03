using System;
using JetBrains.Annotations;

namespace ScriptedEventsAPI.Helpers;

public class Either<T1, T2>
{
    private readonly Type _type;
    private readonly object _value;

    public Either([NotNull] T1 item)
    {
        _value = item ?? throw new ArgumentNullException(nameof(item));
        _type = typeof(T1);
    }

    public Either([NotNull] T2 item)
    {
        _value = item ?? throw new ArgumentNullException(nameof(item));
        _type = typeof(T2);
    }

    public bool IsType<T>()
    {
        return _value is T;
    }

    public T Cast<T>()
    {
        if (_value is T castedValue)
        {
            return castedValue;
        }

        throw new InvalidCastException($"Cannot cast value of type {_type} to {typeof(T)}.");
    }
    
    public static implicit operator Either<T1, T2>(T1 item) => new(item);
    public static implicit operator Either<T1, T2>(T2 item) => new(item);
    public static implicit operator T1?(Either<T1, T2> either)
    {
        return either.IsType<T1>() 
            ? either.Cast<T1>() 
            : default;
    }
    
    public static implicit operator T2?(Either<T1, T2> either)
    {
        return either.IsType<T2>() 
            ? either.Cast<T2>() 
            : default;
    }
}