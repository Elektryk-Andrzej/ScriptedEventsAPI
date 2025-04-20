using System;
using System.Globalization;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals.Integral;

internal class Int32LiteralElement : IntegralLiteralElement
{
    private const string MinValue = "2147483648";
    private readonly bool _myIsMinValue;

    public Int32LiteralElement(int value)
    {
        Value = value;
    }

    private Int32LiteralElement()
    {
        _myIsMinValue = true;
    }

    public override Type ResultType => typeof(int);

    public int Value { get; private set; }

    public static Int32LiteralElement TryCreate(string image, bool isHex, bool negated)
    {
        if (negated & (image == MinValue)) return new Int32LiteralElement();

        if (isHex)
        {
            var value = default(int);

            // Since Int32.TryParse will succeed for a string like 0xFFFFFFFF we have to do some special handling
            if (int.TryParse(image, NumberStyles.AllowHexSpecifier, null, out value) == false) return null;

            if ((value >= 0) & (value <= int.MaxValue)) return new Int32LiteralElement(value);

            return null;
        }
        else
        {
            var value = default(int);

            if (int.TryParse(image, out value)) return new Int32LiteralElement(value);

            return null;
        }
    }

    public void Negate()
    {
        if (_myIsMinValue)
            Value = int.MinValue;
        else
            Value = -Value;
    }

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        EmitLoad(Value, ilg);
    }
}