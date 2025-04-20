using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals.Integral;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements;

internal class CompareElement : BinaryExpressionElement
{
    private LogicalCompareOperation _myOperation;

    public void Initialize(ExpressionElement leftChild, ExpressionElement rightChild, LogicalCompareOperation op)
    {
        MyLeftChild = leftChild;
        MyRightChild = rightChild;
        _myOperation = op;
    }

    public void Validate()
    {
        ValidateInternal(_myOperation);
    }

    protected override void GetOperation(object operation)
    {
        _myOperation = (LogicalCompareOperation)operation;
    }

    protected override Type GetResultType(Type leftType, Type rightType)
    {
        var binaryResultType = ImplicitConverter.GetBinaryResultType(leftType, rightType);
        var overloadedOperator = GetOverloadedCompareOperator();
        var isEqualityOp = IsOpTypeEqualOrNotEqual(_myOperation);

        // Use our string equality instead of overloaded operator
        if (ReferenceEquals(leftType, typeof(string)) & ReferenceEquals(rightType, typeof(string)) & isEqualityOp)
            // String equality
            return typeof(bool);

        if (overloadedOperator != null) return overloadedOperator.ReturnType;

        if (binaryResultType != null)
            // Comparison of numeric operands
            return typeof(bool);

        if (ReferenceEquals(leftType, typeof(bool)) & ReferenceEquals(rightType, typeof(bool)) & isEqualityOp)
            // Boolean equality
            return typeof(bool);

        if (AreBothChildrenReferenceTypes() & isEqualityOp)
            // Comparison of reference types
            return typeof(bool);

        if (AreBothChildrenSameEnum()) return typeof(bool);

        // Invalid operands
        return null;
    }

    private MethodInfo GetOverloadedCompareOperator()
    {
        var name = GetCompareOperatorName(_myOperation);
        return GetOverloadedBinaryOperator(name, _myOperation);
    }

    private static string GetCompareOperatorName(LogicalCompareOperation op)
    {
        switch (op)
        {
            case LogicalCompareOperation.Equal:
                return "Equality";
            case LogicalCompareOperation.NotEqual:
                return "Inequality";
            case LogicalCompareOperation.GreaterThan:
                return "GreaterThan";
            case LogicalCompareOperation.LessThan:
                return "LessThan";
            case LogicalCompareOperation.GreaterThanOrEqual:
                return "GreaterThanOrEqual";
            case LogicalCompareOperation.LessThanOrEqual:
                return "LessThanOrEqual";
            default:
                Debug.Assert(false, "unknown compare type");
                return null;
        }
    }

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        var binaryResultType = ImplicitConverter.GetBinaryResultType(MyLeftChild.ResultType, MyRightChild.ResultType);
        var overloadedOperator = GetOverloadedCompareOperator();

        if (AreBothChildrenOfType(typeof(string)))
        {
            // String equality
            MyLeftChild.Emit(ilg, services);
            MyRightChild.Emit(ilg, services);
            EmitStringEquality(ilg, _myOperation, services);
        }
        else if (overloadedOperator != null)
        {
            EmitOverloadedOperatorCall(overloadedOperator, ilg, services);
        }
        else if (binaryResultType != null)
        {
            // Emit a compare of numeric operands
            EmitChildWithConvert(MyLeftChild, binaryResultType, ilg, services);
            EmitChildWithConvert(MyRightChild, binaryResultType, ilg, services);
            EmitCompareOperation(ilg, _myOperation);
        }
        else if (AreBothChildrenOfType(typeof(bool)))
        {
            // Boolean equality
            EmitRegular(ilg, services);
        }
        else if (AreBothChildrenReferenceTypes())
        {
            // Reference equality
            EmitRegular(ilg, services);
        }
        else if (MyLeftChild.ResultType.IsEnum & MyRightChild.ResultType.IsEnum)
        {
            EmitRegular(ilg, services);
        }
        else
        {
            Debug.Fail("unknown operand types");
        }
    }

    private void EmitRegular(FleeILGenerator ilg, IServiceProvider services)
    {
        MyLeftChild.Emit(ilg, services);
        MyRightChild.Emit(ilg, services);
        EmitCompareOperation(ilg, _myOperation);
    }

    private static void EmitStringEquality(FleeILGenerator ilg, LogicalCompareOperation op, IServiceProvider services)
    {
        // Get the StringComparison from the options
        var options = (ExpressionOptions)services.GetService(typeof(ExpressionOptions));
        var ic = new Int32LiteralElement((int)options.StringComparison);

        ic.Emit(ilg, services);

        // and emit the method call
        var mi = typeof(string).GetMethod("Equals", new[] { typeof(string), typeof(string), typeof(StringComparison) },
            null);
        ilg.Emit(OpCodes.Call, mi);

        if (op == LogicalCompareOperation.NotEqual)
        {
            ilg.Emit(OpCodes.Ldc_I4_0);
            ilg.Emit(OpCodes.Ceq);
        }
    }

    private static bool IsOpTypeEqualOrNotEqual(LogicalCompareOperation op)
    {
        return (op == LogicalCompareOperation.Equal) | (op == LogicalCompareOperation.NotEqual);
    }

    private bool AreBothChildrenReferenceTypes()
    {
        return (MyLeftChild.ResultType.IsValueType == false) & (MyRightChild.ResultType.IsValueType == false);
    }

    private bool AreBothChildrenSameEnum()
    {
        return MyLeftChild.ResultType.IsEnum && ReferenceEquals(MyLeftChild.ResultType, MyRightChild.ResultType);
    }

    /// <summary>
    ///     Emit the actual compare
    /// </summary>
    /// <param name="ilg"></param>
    /// <param name="op"></param>
    private void EmitCompareOperation(FleeILGenerator ilg, LogicalCompareOperation op)
    {
        var ltOpcode = GetCompareGTLTOpcode(false);
        var gtOpcode = GetCompareGTLTOpcode(true);

        switch (op)
        {
            case LogicalCompareOperation.Equal:
                ilg.Emit(OpCodes.Ceq);
                break;
            case LogicalCompareOperation.LessThan:
                ilg.Emit(ltOpcode);
                break;
            case LogicalCompareOperation.GreaterThan:
                ilg.Emit(gtOpcode);
                break;
            case LogicalCompareOperation.NotEqual:
                ilg.Emit(OpCodes.Ceq);
                ilg.Emit(OpCodes.Ldc_I4_0);
                ilg.Emit(OpCodes.Ceq);
                break;
            case LogicalCompareOperation.LessThanOrEqual:
                ilg.Emit(gtOpcode);
                ilg.Emit(OpCodes.Ldc_I4_0);
                ilg.Emit(OpCodes.Ceq);
                break;
            case LogicalCompareOperation.GreaterThanOrEqual:
                ilg.Emit(ltOpcode);
                ilg.Emit(OpCodes.Ldc_I4_0);
                ilg.Emit(OpCodes.Ceq);
                break;
            default:
                Debug.Fail("Unknown op type");
                break;
        }
    }

    /// <summary>
    ///     Get the correct greater/less than opcode
    /// </summary>
    /// <param name="greaterThan"></param>
    /// <returns></returns>
    private OpCode GetCompareGTLTOpcode(bool greaterThan)
    {
        var leftType = MyLeftChild.ResultType;

        if (ReferenceEquals(leftType, MyRightChild.ResultType))
        {
            if (ReferenceEquals(leftType, typeof(uint)) | ReferenceEquals(leftType, typeof(ulong)))
            {
                if (greaterThan) return OpCodes.Cgt_Un;

                return OpCodes.Clt_Un;
            }

            return GetCompareOpcode(greaterThan);
        }

        return GetCompareOpcode(greaterThan);
    }

    private static OpCode GetCompareOpcode(bool greaterThan)
    {
        if (greaterThan) return OpCodes.Cgt;

        return OpCodes.Clt;
    }
}