using System;
using System.Globalization;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals.Integral;

internal class UInt32LiteralElement : IntegralLiteralElement
{
    private readonly uint _myValue;

    public UInt32LiteralElement(uint value)
    {
        _myValue = value;
    }

    public override Type ResultType => typeof(uint);

    public static UInt32LiteralElement TryCreate(string image, NumberStyles ns)
    {
        var value = default(uint);
        if (uint.TryParse(image, ns, null, out value)) return new UInt32LiteralElement(value);

        return null;
    }

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        EmitLoad(Convert.ToInt32(_myValue), ilg);
    }
}