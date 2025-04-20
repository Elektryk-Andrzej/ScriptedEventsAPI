using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals.Integral;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.LogicalBitwise;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.MemberElements;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;

namespace ScriptedEventsAPI.Helpers.Flee.Parsing;

internal class FleeExpressionAnalyzer : ExpressionAnalyzer
{
    private readonly Regex _myRegularEscapeRegex;
    private readonly Regex _myUnicodeEscapeRegex;

    private bool _myInUnaryNegate;

    private IServiceProvider _myServices;

    internal FleeExpressionAnalyzer()
    {
        _myUnicodeEscapeRegex = new Regex("\\\\u[0-9a-f]{4}", RegexOptions.IgnoreCase);
        _myRegularEscapeRegex = new Regex("\\\\[\\\\\"'trn]", RegexOptions.IgnoreCase);
    }

    public void SetServices(IServiceProvider services)
    {
        _myServices = services;
    }

    public override void Reset()
    {
        _myServices = null;
    }

    public override Node ExitExpression(Production node)
    {
        AddFirstChildValue(node);
        return node;
    }

    public override Node ExitExpressionGroup(Production node)
    {
        node.AddValues(GetChildValues(node));
        return node;
    }

    public override Node ExitXorExpression(Production node)
    {
        AddBinaryOp(node, typeof(XorElement));
        return node;
    }

    public override Node ExitOrExpression(Production node)
    {
        AddBinaryOp(node, typeof(AndOrElement));
        return node;
    }

    public override Node ExitAndExpression(Production node)
    {
        AddBinaryOp(node, typeof(AndOrElement));
        return node;
    }

    public override Node ExitNotExpression(Production node)
    {
        AddUnaryOp(node, typeof(NotElement));
        return node;
    }

    public override Node ExitCompareExpression(Production node)
    {
        AddBinaryOp(node, typeof(CompareElement));
        return node;
    }

    public override Node ExitShiftExpression(Production node)
    {
        AddBinaryOp(node, typeof(ShiftElement));
        return node;
    }

    public override Node ExitAdditiveExpression(Production node)
    {
        AddBinaryOp(node, typeof(ArithmeticElement));
        return node;
    }

    public override Node ExitMultiplicativeExpression(Production node)
    {
        AddBinaryOp(node, typeof(ArithmeticElement));
        return node;
    }

    public override Node ExitPowerExpression(Production node)
    {
        AddBinaryOp(node, typeof(ArithmeticElement));
        return node;
    }

    // Try to fold a negated constant int32.  We have to do this so that parsing int32.MinValue will work
    public override Node ExitNegateExpression(Production node)
    {
        IList childValues = GetChildValues(node);

        // Get last child
        var childElement = (ExpressionElement)childValues[childValues.Count - 1];

        // Is it an signed integer constant?
        if (ReferenceEquals(childElement.GetType(), typeof(Int32LiteralElement)) & (childValues.Count == 2))
        {
            ((Int32LiteralElement)childElement).Negate();
            // Add it directly instead of the negate element since it will already be negated
            node.AddValue(childElement);
        }
        else if (ReferenceEquals(childElement.GetType(), typeof(Int64LiteralElement)) & (childValues.Count == 2))
        {
            ((Int64LiteralElement)childElement).Negate();
            // Add it directly instead of the negate element since it will already be negated
            node.AddValue(childElement);
        }
        else
        {
            // No so just add a regular negate
            AddUnaryOp(node, typeof(NegateElement));
        }

        return node;
    }

    public override Node ExitMemberExpression(Production node)
    {
        IList childValues = GetChildValues(node);
        var first = childValues[0];

        if (childValues.Count == 1 && !(first is MemberElement))
        {
            node.AddValue(first);
        }
        else
        {
            var list = new InvocationListElement(childValues, _myServices);
            node.AddValue(list);
        }

        return node;
    }

    public override Node ExitIndexExpression(Production node)
    {
        IList childValues = GetChildValues(node);
        var args = new ArgumentList(childValues);
        var e = new IndexerElement(args);
        node.AddValue(e);
        return node;
    }

    public override Node ExitMemberAccessExpression(Production node)
    {
        node.AddValue(node.GetChildAt(1).GetValue(0));
        return node;
    }

    public override Node ExitSpecialFunctionExpression(Production node)
    {
        AddFirstChildValue(node);
        return node;
    }

    public override Node ExitIfExpression(Production node)
    {
        IList childValues = GetChildValues(node);
        var op = new ConditionalElement((ExpressionElement)childValues[0], (ExpressionElement)childValues[1],
            (ExpressionElement)childValues[2]);
        node.AddValue(op);
        return node;
    }

    public override Node ExitInExpression(Production node)
    {
        IList childValues = GetChildValues(node);

        if (childValues.Count == 1)
        {
            AddFirstChildValue(node);
            return node;
        }

        var operand = (ExpressionElement)childValues[0];
        childValues.RemoveAt(0);

        var second = childValues[0];
        var op = default(InElement);

        if (second is IList)
        {
            op = new InElement(operand, (IList)second);
        }
        else
        {
            var il = new InvocationListElement(childValues, _myServices);
            op = new InElement(operand, il);
        }

        node.AddValue(op);
        return node;
    }

    public override Node ExitInTargetExpression(Production node)
    {
        AddFirstChildValue(node);
        return node;
    }

    public override Node ExitInListTargetExpression(Production node)
    {
        IList childValues = GetChildValues(node);
        node.AddValue(childValues);
        return node;
    }

    public override Node ExitCastExpression(Production node)
    {
        IList childValues = GetChildValues(node);
        string[] destTypeParts = (string[])childValues[1];
        var isArray = (bool)childValues[2];
        var op = new CastElement((ExpressionElement)childValues[0], destTypeParts, isArray, _myServices);
        node.AddValue(op);
        return node;
    }

    public override Node ExitCastTypeExpression(Production node)
    {
        IList childValues = GetChildValues(node);
        List<string> parts = new List<string>();

        foreach (string part in childValues) parts.Add(part);

        var isArray = false;

        if (parts[parts.Count - 1] == "[]")
        {
            isArray = true;
            parts.RemoveAt(parts.Count - 1);
        }

        node.AddValue(parts.ToArray());
        node.AddValue(isArray);
        return node;
    }

    public override Node ExitMemberFunctionExpression(Production node)
    {
        AddFirstChildValue(node);
        return node;
    }

    public override Node ExitFieldPropertyExpression(Production node)
    {
        //string name = ((Token)node.GetChildAt(0))?.Image;
        var name = node.GetChildAt(0).GetValue(0).ToString();
        var elem = new IdentifierElement(name);
        node.AddValue(elem);
        return node;
    }

    public override Node ExitFunctionCallExpression(Production node)
    {
        IList childValues = GetChildValues(node);
        var name = (string)childValues[0];
        childValues.RemoveAt(0);
        var args = new ArgumentList(childValues);
        var funcCall = new FunctionCallElement(name, args);
        node.AddValue(funcCall);
        return node;
    }

    public override Node ExitArgumentList(Production node)
    {
        IList childValues = GetChildValues(node);
        node.AddValues((ArrayList)childValues);
        return node;
    }

    public override Node ExitBasicExpression(Production node)
    {
        AddFirstChildValue(node);
        return node;
    }

    public override Node ExitLiteralExpression(Production node)
    {
        AddFirstChildValue(node);
        return node;
    }

    private void AddFirstChildValue(Production node)
    {
        node.AddValue(GetChildAt(node, 0).Values[0]);
    }

    private void AddUnaryOp(Production node, Type elementType)
    {
        IList childValues = GetChildValues(node);

        if (childValues.Count == 2)
        {
            var element = (UnaryElement)Activator.CreateInstance(elementType);
            element.SetChild((ExpressionElement)childValues[1]);
            node.AddValue(element);
        }
        else
        {
            node.AddValue(childValues[0]);
        }
    }

    private void AddBinaryOp(Production node, Type elementType)
    {
        IList childValues = GetChildValues(node);

        if (childValues.Count > 1)
        {
            var e = BinaryExpressionElement.CreateElement(childValues, elementType);
            node.AddValue(e);
        }
        else if (childValues.Count == 1)
        {
            node.AddValue(childValues[0]);
        }
        else
        {
            Debug.Assert(false, "wrong number of chilren");
        }
    }

    public override Node ExitReal(Token node)
    {
        var image = node.Image;
        var element = RealLiteralElement.Create(image, _myServices);

        node.AddValue(element);
        return node;
    }

    public override Node ExitInteger(Token node)
    {
        var element = IntegralLiteralElement.Create(node.Image, false, _myInUnaryNegate, _myServices);
        node.AddValue(element);
        return node;
    }

    public override Node ExitHexliteral(Token node)
    {
        var element = IntegralLiteralElement.Create(node.Image, true, _myInUnaryNegate, _myServices);
        node.AddValue(element);
        return node;
    }

    public override Node ExitBooleanLiteralExpression(Production node)
    {
        AddFirstChildValue(node);
        return node;
    }

    public override Node ExitTrue(Token node)
    {
        node.AddValue(new BooleanLiteralElement(true));
        return node;
    }

    public override Node ExitFalse(Token node)
    {
        node.AddValue(new BooleanLiteralElement(false));
        return node;
    }

    public override Node ExitStringLiteral(Token node)
    {
        var s = DoEscapes(node.Image);
        var element = new StringLiteralElement(s);
        node.AddValue(element);
        return node;
    }

    public override Node ExitCharLiteral(Token node)
    {
        var s = DoEscapes(node.Image);
        node.AddValue(new CharLiteralElement(s[0]));
        return node;
    }

    public override Node ExitDatetime(Token node)
    {
        var context = (ExpressionContext)_myServices.GetService(typeof(ExpressionContext));
        var image = node.Image.Substring(1, node.Image.Length - 2);
        var element = new DateTimeLiteralElement(image, context);
        node.AddValue(element);
        return node;
    }

    public override Node ExitTimespan(Token node)
    {
        var image = node.Image.Substring(2, node.Image.Length - 3);
        var element = new TimeSpanLiteralElement(image);
        node.AddValue(element);
        return node;
    }

    private string DoEscapes(string image)
    {
        // Remove outer quotes
        image = image.Substring(1, image.Length - 2);
        image = _myUnicodeEscapeRegex.Replace(image, UnicodeEscapeMatcher);
        image = _myRegularEscapeRegex.Replace(image, RegularEscapeMatcher);
        return image;
    }

    private string RegularEscapeMatcher(Match m)
    {
        var s = m.Value;
        // Remove leading \
        s = s.Remove(0, 1);

        switch (s)
        {
            case "\\":
            case "\"":
            case "'":
                return s;
            case "t":
            case "T":
                return Convert.ToChar(9).ToString();
            case "n":
            case "N":
                return Convert.ToChar(10).ToString();
            case "r":
            case "R":
                return Convert.ToChar(13).ToString();
            default:
                Debug.Assert(false, "Unrecognized escape sequence");
                return null;
        }
    }

    private string UnicodeEscapeMatcher(Match m)
    {
        var s = m.Value;
        // Remove \u
        s = s.Remove(0, 2);
        var code = int.Parse(s, NumberStyles.AllowHexSpecifier);
        var c = Convert.ToChar(code);
        return c.ToString();
    }

    public override Node ExitIdentifier(Token node)
    {
        node.AddValue(node.Image);
        return node;
    }

    public override Node ExitNullLiteral(Token node)
    {
        node.AddValue(new NullLiteralElement());
        return node;
    }

    public override Node ExitArrayBraces(Token node)
    {
        node.AddValue("[]");
        return node;
    }

    public override Node ExitAdd(Token node)
    {
        node.AddValue(BinaryArithmeticOperation.Add);
        return node;
    }

    public override Node ExitSub(Token node)
    {
        node.AddValue(BinaryArithmeticOperation.Subtract);
        return node;
    }

    public override Node ExitMul(Token node)
    {
        node.AddValue(BinaryArithmeticOperation.Multiply);
        return node;
    }

    public override Node ExitDiv(Token node)
    {
        node.AddValue(BinaryArithmeticOperation.Divide);
        return node;
    }

    public override Node ExitMod(Token node)
    {
        node.AddValue(BinaryArithmeticOperation.Mod);
        return node;
    }

    public override Node ExitPower(Token node)
    {
        node.AddValue(BinaryArithmeticOperation.Power);
        return node;
    }

    public override Node ExitEq(Token node)
    {
        node.AddValue(LogicalCompareOperation.Equal);
        return node;
    }

    public override Node ExitNe(Token node)
    {
        node.AddValue(LogicalCompareOperation.NotEqual);
        return node;
    }

    public override Node ExitLt(Token node)
    {
        node.AddValue(LogicalCompareOperation.LessThan);
        return node;
    }

    public override Node ExitGt(Token node)
    {
        node.AddValue(LogicalCompareOperation.GreaterThan);
        return node;
    }

    public override Node ExitLte(Token node)
    {
        node.AddValue(LogicalCompareOperation.LessThanOrEqual);
        return node;
    }

    public override Node ExitGte(Token node)
    {
        node.AddValue(LogicalCompareOperation.GreaterThanOrEqual);
        return node;
    }

    public override Node ExitAnd(Token node)
    {
        node.AddValue(AndOrOperation.And);
        return node;
    }

    public override Node ExitOr(Token node)
    {
        node.AddValue(AndOrOperation.Or);
        return node;
    }

    public override Node ExitXor(Token node)
    {
        node.AddValue("Xor");
        return node;
    }

    public override Node ExitNot(Token node)
    {
        node.AddValue(string.Empty);
        return node;
    }

    public override Node ExitLeftShift(Token node)
    {
        node.AddValue(ShiftOperation.LeftShift);
        return node;
    }

    public override Node ExitRightShift(Token node)
    {
        node.AddValue(ShiftOperation.RightShift);
        return node;
    }

    public override void Child(Production node, Node child)
    {
        base.Child(node, child);
        _myInUnaryNegate = (node.Id == (int)ExpressionConstants.NEGATE_EXPRESSION) &
                           (child.Id == (int)ExpressionConstants.SUB);
    }
}