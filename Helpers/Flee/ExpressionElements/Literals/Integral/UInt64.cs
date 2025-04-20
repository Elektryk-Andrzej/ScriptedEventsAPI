using System;
using System.Globalization;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals.Integral;

internal class UInt64LiteralElement : IntegralLiteralElement
{
    private readonly ulong _myValue;

    public UInt64LiteralElement(string image, NumberStyles ns)
    {
        try
        {
            _myValue = ulong.Parse(image, ns);
        }
        catch (OverflowException ex)
        {
            OnParseOverflow(image);
        }
    }

    public UInt64LiteralElement(ulong value)
    {
        _myValue = value;
    }

    public override Type ResultType => typeof(ulong);

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        EmitLoad(Convert.ToInt64(_myValue), ilg);
    }
}