using System;
using System.Runtime.Serialization;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.Parsing;
using ScriptedEventsAPI.Helpers.Flee.Resources;

namespace ScriptedEventsAPI.Helpers.Flee.PublicTypes;

public enum CompileExceptionReason
{
    SyntaxError,
    ConstantOverflow,
    TypeMismatch,
    UndefinedName,
    FunctionHasNoReturnValue,
    InvalidExplicitCast,
    AmbiguousMatch,
    AccessDenied,
    InvalidFormat
}

/// <summary>
/// </summary>
[Serializable]
public sealed class ExpressionCompileException : Exception
{
    internal ExpressionCompileException(string message, CompileExceptionReason reason) : base(message)
    {
        Reason = reason;
    }

    internal ExpressionCompileException(ParserLogException parseException) : base(string.Empty, parseException)
    {
        Reason = CompileExceptionReason.SyntaxError;
    }

    private ExpressionCompileException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Reason = (CompileExceptionReason)info.GetInt32("Reason");
    }

    public override string Message
    {
        get
        {
            if (Reason == CompileExceptionReason.SyntaxError)
            {
                var innerEx = InnerException;
                var msg = $"{Utility.GetCompileErrorMessage(CompileErrorResourceKeys.SyntaxError)}: {innerEx.Message}";
                return msg;
            }

            return base.Message;
        }
    }

    public CompileExceptionReason Reason { get; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("Reason", Convert.ToInt32(Reason));
    }
}