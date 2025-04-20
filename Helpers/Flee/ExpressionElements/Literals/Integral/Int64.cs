using System;
using System.Globalization;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals.Integral;

internal class Int64LiteralElement : IntegralLiteralElement
{
    private const string MinValue = "9223372036854775808";

    private readonly bool _myIsMinValue;

    private long _myValue;

    public Int64LiteralElement(long value)
    {
        _myValue = value;
    }

    private Int64LiteralElement()
    {
        _myIsMinValue = true;
    }

    public override Type ResultType => typeof(long);

    public static Int64LiteralElement TryCreate(string image, bool isHex, bool negated)
    {
        if (negated & (image == MinValue)) return new Int64LiteralElement();

        if (isHex)
        {
            var value = default(long);

            if (long.TryParse(image, NumberStyles.AllowHexSpecifier, null, out value) == false) return null;

            if ((value >= 0) & (value <= long.MaxValue)) return new Int64LiteralElement(value);

            return null;
        }
        else
        {
            var value = default(long);

            if (long.TryParse(image, out value)) return new Int64LiteralElement(value);

            return null;
        }
    }

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        EmitLoad(_myValue, ilg);
    }

    public void Negate()
    {
        if (_myIsMinValue)
            _myValue = long.MinValue;
        else
            _myValue = -_myValue;
    }
}