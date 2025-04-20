using System;
using System.Diagnostics;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;
using ScriptedEventsAPI.Helpers.Flee.Resources;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base;

internal abstract class ExpressionElement
{
    /// <summary>
    ///     All expression elements must expose the Type they evaluate to
    /// </summary>
    public abstract Type ResultType { get; }


    protected string Name
    {
        get
        {
            var key = GetType().Name;
            var value = FleeResourceManager.Instance.GetElementNameString(key);
            Debug.Assert(value != null, $"Element name for '{key}' not in resource file");
            return value;
        }
    }

    /// <summary>
    ///     // All expression elements must be able to emit their IL
    /// </summary>
    /// <param name="ilg"></param>
    /// <param name="services"></param>
    public abstract void Emit(FleeILGenerator ilg, IServiceProvider services);

    public override string ToString()
    {
        return Name;
    }

    protected void ThrowCompileException(string messageKey, CompileExceptionReason reason, params object[] arguments)
    {
        var messageTemplate = FleeResourceManager.Instance.GetCompileErrorString(messageKey);
        var message = string.Format(messageTemplate, arguments);
        message = string.Concat(Name, ": ", message);
        throw new ExpressionCompileException(message, reason);
    }

    protected void ThrowAmbiguousCallException(Type leftType, Type rightType, object operation)
    {
        ThrowCompileException(CompileErrorResourceKeys.AmbiguousOverloadedOperator,
            CompileExceptionReason.AmbiguousMatch, leftType.Name, rightType.Name, operation);
    }
}