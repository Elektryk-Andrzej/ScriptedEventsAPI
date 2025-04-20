using System;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals.Real;

internal class DoubleLiteralElement : RealLiteralElement
{
    private readonly double _myValue;

    private DoubleLiteralElement()
    {
    }

    public DoubleLiteralElement(double value)
    {
        _myValue = value;
    }

    public override Type ResultType => typeof(double);

    public static DoubleLiteralElement Parse(string image, IServiceProvider services)
    {
        var options = (ExpressionParserOptions)services.GetService(typeof(ExpressionParserOptions));
        var element = new DoubleLiteralElement();

        try
        {
            var value = options.ParseDouble(image);
            return new DoubleLiteralElement(value);
        }
        catch (OverflowException ex)
        {
            element.OnParseOverflow(image);
            return null;
        }
    }

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        ilg.Emit(OpCodes.Ldc_R8, _myValue);
    }
}