using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals.Integral;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements;

internal class ArithmeticElement : BinaryExpressionElement
{
    private static MethodInfo _ourPowerMethodInfo;
    private static MethodInfo _ourStringConcatMethodInfo;
    private static MethodInfo _ourObjectConcatMethodInfo;
    private BinaryArithmeticOperation _myOperation;

    public ArithmeticElement()
    {
        _ourPowerMethodInfo = typeof(Math).GetMethod("Pow", BindingFlags.Public | BindingFlags.Static);
        _ourStringConcatMethodInfo = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }, null);
        _ourObjectConcatMethodInfo = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) }, null);
    }

    private bool IsOptimizablePower
    {
        get
        {
            if (_myOperation != BinaryArithmeticOperation.Power || !(MyRightChild is Int32LiteralElement)) return false;

            var right = (Int32LiteralElement)MyRightChild;

            return right?.Value >= 0;
        }
    }

    protected override void GetOperation(object operation)
    {
        _myOperation = (BinaryArithmeticOperation)operation;
    }

    protected override Type GetResultType(Type leftType, Type rightType)
    {
        var binaryResultType = ImplicitConverter.GetBinaryResultType(leftType, rightType);
        var overloadedMethod = GetOverloadedArithmeticOperator();

        // Is an overloaded operator defined for our left and right children?
        if (overloadedMethod != null)
            // Yes, so use its return type
            return overloadedMethod.ReturnType;

        if (binaryResultType != null)
        {
            // Operands are primitive types.  Return computed result type unless we are doing a power operation
            if (_myOperation == BinaryArithmeticOperation.Power)
                return GetPowerResultType(leftType, rightType, binaryResultType);

            return binaryResultType;
        }

        if (IsEitherChildOfType(typeof(string)) & (_myOperation == BinaryArithmeticOperation.Add))
            // String concatenation
            return typeof(string);

        // Invalid types
        return null;
    }

    private Type GetPowerResultType(Type leftType, Type rightType, Type binaryResultType)
    {
        if (IsOptimizablePower) return leftType;

        return typeof(double);
    }

    private MethodInfo GetOverloadedArithmeticOperator()
    {
        // Get the name of the operator
        var name = GetOverloadedOperatorFunctionName(_myOperation);
        return GetOverloadedBinaryOperator(name, _myOperation);
    }

    private static string GetOverloadedOperatorFunctionName(BinaryArithmeticOperation op)
    {
        switch (op)
        {
            case BinaryArithmeticOperation.Add:
                return "Addition";
            case BinaryArithmeticOperation.Subtract:
                return "Subtraction";
            case BinaryArithmeticOperation.Multiply:
                return "Multiply";
            case BinaryArithmeticOperation.Divide:
                return "Division";
            case BinaryArithmeticOperation.Mod:
                return "Modulus";
            case BinaryArithmeticOperation.Power:
                return "Exponent";
            default:
                Debug.Assert(false, "unknown operator type");
                return null;
        }
    }

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        var overloadedMethod = GetOverloadedArithmeticOperator();

        if (overloadedMethod != null)
            // Emit a call to an overloaded operator
            EmitOverloadedOperatorCall(overloadedMethod, ilg, services);
        else if (IsEitherChildOfType(typeof(string)))
            // One of our operands is a string so emit a concatenation
            EmitStringConcat(ilg, services);
        else
            // Emit a regular arithmetic operation			
            EmitArithmeticOperation(_myOperation, ilg, services);
    }

    private static bool IsUnsignedForArithmetic(Type t)
    {
        return ReferenceEquals(t, typeof(uint)) | ReferenceEquals(t, typeof(ulong));
    }

    /// <summary>
    ///     Emit an arithmetic operation with handling for unsigned and checked contexts
    /// </summary>
    /// <param name="op"></param>
    /// <param name="ilg"></param>
    /// <param name="services"></param>
    private void EmitArithmeticOperation(BinaryArithmeticOperation op, FleeILGenerator ilg, IServiceProvider services)
    {
        var options = (ExpressionOptions)services.GetService(typeof(ExpressionOptions));
        var unsigned = IsUnsignedForArithmetic(MyLeftChild.ResultType) &
                       IsUnsignedForArithmetic(MyRightChild.ResultType);
        var integral = Utility.IsIntegralType(MyLeftChild.ResultType) & Utility.IsIntegralType(MyRightChild.ResultType);
        var emitOverflow = integral & options.Checked;

        EmitChildWithConvert(MyLeftChild, ResultType, ilg, services);

        if (IsOptimizablePower == false) EmitChildWithConvert(MyRightChild, ResultType, ilg, services);

        switch (op)
        {
            case BinaryArithmeticOperation.Add:
                if (emitOverflow)
                {
                    if (unsigned)
                        ilg.Emit(OpCodes.Add_Ovf_Un);
                    else
                        ilg.Emit(OpCodes.Add_Ovf);
                }
                else
                {
                    ilg.Emit(OpCodes.Add);
                }

                break;
            case BinaryArithmeticOperation.Subtract:
                if (emitOverflow)
                {
                    if (unsigned)
                        ilg.Emit(OpCodes.Sub_Ovf_Un);
                    else
                        ilg.Emit(OpCodes.Sub_Ovf);
                }
                else
                {
                    ilg.Emit(OpCodes.Sub);
                }

                break;
            case BinaryArithmeticOperation.Multiply:
                EmitMultiply(ilg, emitOverflow, unsigned);
                break;
            case BinaryArithmeticOperation.Divide:
                if (unsigned)
                    ilg.Emit(OpCodes.Div_Un);
                else
                    ilg.Emit(OpCodes.Div);
                break;
            case BinaryArithmeticOperation.Mod:
                if (unsigned)
                    ilg.Emit(OpCodes.Rem_Un);
                else
                    ilg.Emit(OpCodes.Rem);
                break;
            case BinaryArithmeticOperation.Power:
                EmitPower(ilg, emitOverflow, unsigned);
                break;
            default:
                Debug.Fail("Unknown op type");
                break;
        }
    }

    private void EmitPower(FleeILGenerator ilg, bool emitOverflow, bool unsigned)
    {
        if (IsOptimizablePower)
            EmitOptimizedPower(ilg, emitOverflow, unsigned);
        else
            ilg.Emit(OpCodes.Call, _ourPowerMethodInfo);
    }

    private void EmitOptimizedPower(FleeILGenerator ilg, bool emitOverflow, bool unsigned)
    {
        var right = (Int32LiteralElement)MyRightChild;

        if (right.Value == 0)
        {
            ilg.Emit(OpCodes.Pop);
            IntegralLiteralElement.EmitLoad(1, ilg);
            ImplicitConverter.EmitImplicitNumericConvert(typeof(int), MyLeftChild.ResultType, ilg);
            return;
        }

        if (right.Value == 1) return;

        // Start at 1 since left operand has already been emited once
        for (var i = 1; i <= right.Value - 1; i++) ilg.Emit(OpCodes.Dup);

        for (var i = 1; i <= right.Value - 1; i++) EmitMultiply(ilg, emitOverflow, unsigned);
    }

    private void EmitMultiply(FleeILGenerator ilg, bool emitOverflow, bool unsigned)
    {
        if (emitOverflow)
        {
            if (unsigned)
                ilg.Emit(OpCodes.Mul_Ovf_Un);
            else
                ilg.Emit(OpCodes.Mul_Ovf);
        }
        else
        {
            ilg.Emit(OpCodes.Mul);
        }
    }

    /// <summary>
    ///     Emit a string concatenation
    /// </summary>
    /// <param name="ilg"></param>
    /// <param name="services"></param>
    private void EmitStringConcat(FleeILGenerator ilg, IServiceProvider services)
    {
        var argType = default(Type);
        var concatMethodInfo = default(MethodInfo);

        // Pick the most specific concat method
        if (AreBothChildrenOfType(typeof(string)))
        {
            concatMethodInfo = _ourStringConcatMethodInfo;
            argType = typeof(string);
        }
        else
        {
            Debug.Assert(IsEitherChildOfType(typeof(string)), "one child must be a string");
            concatMethodInfo = _ourObjectConcatMethodInfo;
            argType = typeof(object);
        }

        // Emit the operands and call the function
        MyLeftChild.Emit(ilg, services);
        ImplicitConverter.EmitImplicitConvert(MyLeftChild.ResultType, argType, ilg);
        MyRightChild.Emit(ilg, services);
        ImplicitConverter.EmitImplicitConvert(MyRightChild.ResultType, argType, ilg);
        ilg.Emit(OpCodes.Call, concatMethodInfo);
    }
}