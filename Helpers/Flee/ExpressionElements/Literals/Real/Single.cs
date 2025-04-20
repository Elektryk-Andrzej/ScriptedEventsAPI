using System;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals.Real;

internal class SingleLiteralElement : RealLiteralElement
{
    private readonly float _myValue;

    private SingleLiteralElement()
    {
    }

    public SingleLiteralElement(float value)
    {
        _myValue = value;
    }

    public override Type ResultType => typeof(float);

    public static SingleLiteralElement Parse(string image, IServiceProvider services)
    {
        var options = (ExpressionParserOptions)services.GetService(typeof(ExpressionParserOptions));
        var element = new SingleLiteralElement();

        try
        {
            var value = options.ParseSingle(image);
            return new SingleLiteralElement(value);
        }
        catch (OverflowException ex)
        {
            element.OnParseOverflow(image);
            return null;
        }
    }

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        ilg.Emit(OpCodes.Ldc_R4, _myValue);
    }
}