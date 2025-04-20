using System;
using System.Reflection;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals.Real;

internal class DecimalLiteralElement : RealLiteralElement
{
    private static readonly ConstructorInfo OurConstructorInfo = GetConstructor();
    private readonly decimal _myValue;

    private DecimalLiteralElement()
    {
    }

    public DecimalLiteralElement(decimal value)
    {
        _myValue = value;
    }

    public override Type ResultType => typeof(decimal);

    private static ConstructorInfo GetConstructor()
    {
        Type[] types =
        {
            typeof(int),
            typeof(int),
            typeof(int),
            typeof(bool),
            typeof(byte)
        };
        return typeof(decimal).GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any,
            types, null);
    }

    public static DecimalLiteralElement Parse(string image, IServiceProvider services)
    {
        var options = (ExpressionParserOptions)services.GetService(typeof(ExpressionParserOptions));
        var element = new DecimalLiteralElement();

        try
        {
            var value = options.ParseDecimal(image);
            return new DecimalLiteralElement(value);
        }
        catch (OverflowException ex)
        {
            element.OnParseOverflow(image);
            return null;
        }
    }

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        var index = ilg.GetTempLocalIndex(typeof(decimal));
        Utility.EmitLoadLocalAddress(ilg, index);

        var bits = decimal.GetBits(_myValue);
        EmitLoad(bits[0], ilg);
        EmitLoad(bits[1], ilg);
        EmitLoad(bits[2], ilg);

        var flags = bits[3];

        EmitLoad(flags >> 31 == -1, ilg);

        EmitLoad(flags >> 16, ilg);

        ilg.Emit(OpCodes.Call, OurConstructorInfo);

        Utility.EmitLoadLocal(ilg, index);
    }
}