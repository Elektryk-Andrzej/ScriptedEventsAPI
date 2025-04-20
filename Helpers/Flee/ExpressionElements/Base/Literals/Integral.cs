using System;
using System.Diagnostics;
using System.Globalization;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals.Integral;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;

internal abstract class IntegralLiteralElement : LiteralElement
{
    /// <summary>
    ///     Attempt to find the first type of integer that a number can fit into
    /// </summary>
    /// <param name="image"></param>
    /// <param name="isHex"></param>
    /// <param name="negated"></param>
    /// <param name="services"></param>
    /// <returns></returns>
    public static LiteralElement Create(string image, bool isHex, bool negated, IServiceProvider services)
    {
        var comparison = StringComparison.OrdinalIgnoreCase;

        if (isHex == false)
        {
            // Create a real element if required
            var realElement = RealLiteralElement.CreateFromInteger(image, services);

            if (realElement != null) return realElement;
        }

        var hasUSuffix = image.EndsWith("u", comparison) & !image.EndsWith("lu", comparison);
        var hasLSuffix = image.EndsWith("l", comparison) & !image.EndsWith("ul", comparison);
        var hasUlSuffix = image.EndsWith("ul", comparison) | image.EndsWith("lu", comparison);
        var hasSuffix = hasUSuffix | hasLSuffix | hasUlSuffix;

        var constant = default(LiteralElement);
        var numStyles = NumberStyles.Integer;

        if (isHex)
        {
            numStyles = NumberStyles.AllowHexSpecifier;
            image = image.Remove(0, 2);
        }

        if (hasSuffix == false)
        {
            // If the literal has no suffix, it has the first of these types in which its value can be represented: int, uint, long, ulong.
            constant = Int32LiteralElement.TryCreate(image, isHex, negated);

            if (constant != null) return constant;

            constant = UInt32LiteralElement.TryCreate(image, numStyles);

            if (constant != null) return constant;

            constant = Int64LiteralElement.TryCreate(image, isHex, negated);

            if (constant != null) return constant;

            return new UInt64LiteralElement(image, numStyles);
        }

        if (hasUSuffix)
        {
            image = image.Remove(image.Length - 1);
            // If the literal is suffixed by U or u, it has the first of these types in which its value can be represented: uint, ulong.

            constant = UInt32LiteralElement.TryCreate(image, numStyles);

            if (constant != null) return constant;

            return new UInt64LiteralElement(image, numStyles);
        }

        if (hasLSuffix)
        {
            // If the literal is suffixed by L or l, it has the first of these types in which its value can be represented: long, ulong.
            image = image.Remove(image.Length - 1);

            constant = Int64LiteralElement.TryCreate(image, isHex, negated);

            if (constant != null) return constant;

            return new UInt64LiteralElement(image, numStyles);
        }

        // If the literal is suffixed by UL, Ul, uL, ul, LU, Lu, lU, or lu, it is of type ulong.
        Debug.Assert(hasUlSuffix, "expecting ul suffix");
        image = image.Remove(image.Length - 2);
        return new UInt64LiteralElement(image, numStyles);
    }
}