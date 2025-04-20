using System;
using System.Diagnostics;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals.Real;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;

internal abstract class RealLiteralElement : LiteralElement
{
    public static LiteralElement CreateFromInteger(string image, IServiceProvider services)
    {
        var element = default(LiteralElement);

        element = CreateSingle(image, services);

        if (element != null) return element;

        element = CreateDecimal(image, services);

        if (element != null) return element;

        var options = (ExpressionOptions)services.GetService(typeof(ExpressionOptions));

        // Convert to a double if option is set
        if (options.IntegersAsDoubles) return DoubleLiteralElement.Parse(image, services);

        return null;
    }

    public static LiteralElement Create(string image, IServiceProvider services)
    {
        var element = default(LiteralElement);

        element = CreateSingle(image, services);

        if (element != null) return element;

        element = CreateDecimal(image, services);

        if (element != null) return element;

        element = CreateDouble(image, services);

        if (element != null) return element;

        element = CreateImplicitReal(image, services);

        return element;
    }

    private static LiteralElement CreateImplicitReal(string image, IServiceProvider services)
    {
        var options = (ExpressionOptions)services.GetService(typeof(ExpressionOptions));
        var realType = options.RealLiteralDataType;

        switch (realType)
        {
            case RealLiteralDataType.Double:
                return DoubleLiteralElement.Parse(image, services);
            case RealLiteralDataType.Single:
                return SingleLiteralElement.Parse(image, services);
            case RealLiteralDataType.Decimal:
                return DecimalLiteralElement.Parse(image, services);
            default:
                Debug.Fail("Unknown value");
                return null;
        }
    }

    private static DoubleLiteralElement CreateDouble(string image, IServiceProvider services)
    {
        if (image.EndsWith("d", StringComparison.OrdinalIgnoreCase))
        {
            image = image.Remove(image.Length - 1);
            return DoubleLiteralElement.Parse(image, services);
        }

        return null;
    }

    private static SingleLiteralElement CreateSingle(string image, IServiceProvider services)
    {
        if (image.EndsWith("f", StringComparison.OrdinalIgnoreCase))
        {
            image = image.Remove(image.Length - 1);
            return SingleLiteralElement.Parse(image, services);
        }

        return null;
    }

    private static DecimalLiteralElement CreateDecimal(string image, IServiceProvider services)
    {
        if (image.EndsWith("m", StringComparison.OrdinalIgnoreCase))
        {
            image = image.Remove(image.Length - 1);
            return DecimalLiteralElement.Parse(image, services);
        }

        return null;
    }
}