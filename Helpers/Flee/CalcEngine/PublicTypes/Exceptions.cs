using System;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;

namespace ScriptedEventsAPI.Helpers.Flee.CalcEngine.PublicTypes;

public class CircularReferenceException : Exception
{
    private readonly string _myCircularReferenceSource;

    internal CircularReferenceException()
    {
    }

    internal CircularReferenceException(string circularReferenceSource)
    {
        _myCircularReferenceSource = circularReferenceSource;
    }

    public override string Message
    {
        get
        {
            if (_myCircularReferenceSource == null) return "Circular reference detected in calculation engine";

            return $"Circular reference detected in calculation engine at '{_myCircularReferenceSource}'";
        }
    }
}

public class BatchLoadCompileException : Exception
{
    internal BatchLoadCompileException(string atomName, string expressionText,
        ExpressionCompileException innerException) : base(
        $"Batch Load: The expression for atom '${atomName}' could not be compiled", innerException)
    {
        AtomName = atomName;
        ExpressionText = expressionText;
    }

    public string AtomName { get; }

    public string ExpressionText { get; }
}