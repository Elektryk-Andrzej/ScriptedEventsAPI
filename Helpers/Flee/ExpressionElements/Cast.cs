using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;
using ScriptedEventsAPI.Helpers.Flee.Resources;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements;

internal class CastElement : ExpressionElement
{
    private readonly ExpressionElement _myCastExpression;
    private readonly Type _myDestType;

    public CastElement(ExpressionElement castExpression, string[] destTypeParts, bool isArray,
        IServiceProvider services)
    {
        _myCastExpression = castExpression;

        _myDestType = GetDestType(destTypeParts, services);

        if (_myDestType == null)
            ThrowCompileException(CompileErrorResourceKeys.CouldNotResolveType, CompileExceptionReason.UndefinedName,
                GetDestTypeString(destTypeParts, isArray));

        if (isArray) _myDestType = _myDestType.MakeArrayType();

        if (IsValidCast(_myCastExpression.ResultType, _myDestType) == false) ThrowInvalidCastException();
    }

    public override Type ResultType => _myDestType;

    private static string GetDestTypeString(string[] parts, bool isArray)
    {
        var s = string.Join(".", parts);

        if (isArray) s = s + "[]";

        return s;
    }

    /// <summary>
    ///     Resolve the type we are casting to
    /// </summary>
    /// <param name="destTypeParts"></param>
    /// <param name="services"></param>
    /// <returns></returns>
    private static Type GetDestType(string[] destTypeParts, IServiceProvider services)
    {
        var context = (ExpressionContext)services.GetService(typeof(ExpressionContext));

        Type t = null;

        // Try to find a builtin type with the name
        if (destTypeParts.Length == 1) t = ExpressionImports.GetBuiltinType(destTypeParts[0]);

        if (t != null) return t;

        // Try to find the type in an import
        t = context.Imports.FindType(destTypeParts);

        if (t != null) return t;

        return null;
    }

    private bool IsValidCast(Type sourceType, Type destType)
    {
        if (ReferenceEquals(sourceType, destType))
            // Identity cast always succeeds
            return true;

        if (destType.IsAssignableFrom(sourceType))
            // Cast is already implicitly valid
            return true;

        if (ImplicitConverter.EmitImplicitConvert(sourceType, destType, null))
            // Cast is already implicitly valid
            return true;

        if (IsCastableNumericType(sourceType) & IsCastableNumericType(destType))
            // Explicit cast of numeric types always succeeds
            return true;

        if (sourceType.IsEnum | destType.IsEnum) return IsValidExplicitEnumCast(sourceType, destType);

        if (GetExplictOverloadedOperator(sourceType, destType) != null)
            // Overloaded explict cast exists
            return true;

        if (sourceType.IsValueType)
            // If we get here then the cast always fails since we are either casting one value type to another
            // or a value type to an invalid reference type
            return false;

        if (destType.IsValueType)
        {
            // Reference type to value type
            // Can only succeed if the reference type is a base of the value type or
            // it is one of the interfaces the value type implements
            Type[] interfaces = destType.GetInterfaces();
            return IsBaseType(destType, sourceType) | (Array.IndexOf(interfaces, sourceType) != -1);
        }

        // Reference type to reference type
        return IsValidExplicitReferenceCast(sourceType, destType);
    }

    private MethodInfo GetExplictOverloadedOperator(Type sourceType, Type destType)
    {
        var binder = new ExplicitOperatorMethodBinder(destType, sourceType);

        // Look for an operator on the source type and dest types
        var miSource = Utility.GetOverloadedOperator("Explicit", sourceType, binder, sourceType);
        var miDest = Utility.GetOverloadedOperator("Explicit", destType, binder, sourceType);

        if ((miSource == null) & (miDest == null)) return null;

        if (miSource == null) return miDest;

        if (miDest == null) return miSource;

        ThrowAmbiguousCallException(sourceType, destType, "Explicit");
        return null;
    }

    private bool IsValidExplicitEnumCast(Type sourceType, Type destType)
    {
        sourceType = GetUnderlyingEnumType(sourceType);
        destType = GetUnderlyingEnumType(destType);
        return IsValidCast(sourceType, destType);
    }

    private bool IsValidExplicitReferenceCast(Type sourceType, Type destType)
    {
        Debug.Assert((sourceType.IsValueType == false) & (destType.IsValueType == false), "expecting reference types");

        if (ReferenceEquals(sourceType, typeof(object)))
            // From object to any other reference-type
            return true;

        if (sourceType.IsArray & destType.IsArray)
        {
            // From an array-type S with an element type SE to an array-type T with an element type TE,
            // provided all of the following are true:

            // S and T have the same number of dimensions
            if (sourceType.GetArrayRank() != destType.GetArrayRank()) return false;

            var SE = sourceType.GetElementType();
            var TE = destType.GetElementType();

            // Both SE and TE are reference-types
            if (SE.IsValueType | TE.IsValueType) return false;

            // An explicit reference conversion exists from SE to TE
            return IsValidExplicitReferenceCast(SE, TE);
        }

        if (sourceType.IsClass & destType.IsClass)
            // From any class-type S to any class-type T, provided S is a base class of T
            return IsBaseType(destType, sourceType);

        if (sourceType.IsClass & destType.IsInterface)
            // From any class-type S to any interface-type T, provided S is not sealed and provided S does not implement T
            return (sourceType.IsSealed == false) & (ImplementsInterface(sourceType, destType) == false);

        if (sourceType.IsInterface & destType.IsClass)
            // From any interface-type S to any class-type T, provided T is not sealed or provided T implements S.
            return (destType.IsSealed == false) | ImplementsInterface(destType, sourceType);

        if (sourceType.IsInterface & destType.IsInterface)
            // From any interface-type S to any interface-type T, provided S is not derived from T
            return ImplementsInterface(sourceType, destType) == false;

        Debug.Assert(false, "unknown explicit cast");

        return false;
    }

    private static bool IsBaseType(Type target, Type potentialBase)
    {
        var current = target;
        while (current != null)
        {
            if (ReferenceEquals(current, potentialBase)) return true;
            current = current.BaseType;
        }

        return false;
    }

    private static bool ImplementsInterface(Type target, Type interfaceType)
    {
        Type[] interfaces = target.GetInterfaces();
        return Array.IndexOf(interfaces, interfaceType) != -1;
    }

    private void ThrowInvalidCastException()
    {
        ThrowCompileException(CompileErrorResourceKeys.CannotConvertType, CompileExceptionReason.InvalidExplicitCast,
            _myCastExpression.ResultType.Name, _myDestType.Name);
    }

    private static bool IsCastableNumericType(Type t)
    {
        return t.IsPrimitive & !ReferenceEquals(t, typeof(bool));
    }

    private static Type GetUnderlyingEnumType(Type t)
    {
        if (t.IsEnum) return Enum.GetUnderlyingType(t);

        return t;
    }

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        _myCastExpression.Emit(ilg, services);

        var sourceType = _myCastExpression.ResultType;
        var destType = _myDestType;

        EmitCast(ilg, sourceType, destType, services);
    }

    private void EmitCast(FleeILGenerator ilg, Type sourceType, Type destType, IServiceProvider services)
    {
        var explicitOperator = GetExplictOverloadedOperator(sourceType, destType);

        if (ReferenceEquals(sourceType, destType))
        {
            // Identity cast; do nothing
        }
        else if (explicitOperator != null)
        {
            ilg.Emit(OpCodes.Call, explicitOperator);
        }
        else if (sourceType.IsEnum | destType.IsEnum)
        {
            EmitEnumCast(ilg, sourceType, destType, services);
        }
        else if (ImplicitConverter.EmitImplicitConvert(sourceType, destType, ilg))
        {
            // Implicit numeric cast; do nothing
        }
        else if (IsCastableNumericType(sourceType) & IsCastableNumericType(destType))
        {
            // Explicit numeric cast
            EmitExplicitNumericCast(ilg, sourceType, destType, services);
        }
        else if (sourceType.IsValueType)
        {
            Debug.Assert(destType.IsValueType == false, "expecting reference type");
            ilg.Emit(OpCodes.Box, sourceType);
        }
        else
        {
            if (destType.IsValueType)
            {
                // Reference type to value type
                ilg.Emit(OpCodes.Unbox_Any, destType);
            }
            else
            {
                // Reference type to reference type
                if (destType.IsAssignableFrom(sourceType) == false)
                    // Only emit cast if it is an explicit cast
                    ilg.Emit(OpCodes.Castclass, destType);
            }
        }
    }

    private void EmitEnumCast(FleeILGenerator ilg, Type sourceType, Type destType, IServiceProvider services)
    {
        if (destType.IsValueType == false)
        {
            ilg.Emit(OpCodes.Box, sourceType);
        }
        else if (sourceType.IsValueType == false)
        {
            ilg.Emit(OpCodes.Unbox_Any, destType);
        }
        else
        {
            sourceType = GetUnderlyingEnumType(sourceType);
            destType = GetUnderlyingEnumType(destType);
            EmitCast(ilg, sourceType, destType, services);
        }
    }

    private static void EmitExplicitNumericCast(FleeILGenerator ilg, Type sourceType, Type destType,
        IServiceProvider services)
    {
        var desttc = Type.GetTypeCode(destType);
        var sourcetc = Type.GetTypeCode(sourceType);
        var unsigned = IsUnsignedType(sourceType);
        var options = (ExpressionOptions)services.GetService(typeof(ExpressionOptions));
        var @checked = options.Checked;
        var op = OpCodes.Nop;

        switch (desttc)
        {
            case TypeCode.SByte:
                if (unsigned & @checked)
                    op = OpCodes.Conv_Ovf_I1_Un;
                else if (@checked)
                    op = OpCodes.Conv_Ovf_I1;
                else
                    op = OpCodes.Conv_I1;
                break;
            case TypeCode.Byte:
                if (unsigned & @checked)
                    op = OpCodes.Conv_Ovf_U1_Un;
                else if (@checked)
                    op = OpCodes.Conv_Ovf_U1;
                else
                    op = OpCodes.Conv_U1;
                break;
            case TypeCode.Int16:
                if (unsigned & @checked)
                    op = OpCodes.Conv_Ovf_I2_Un;
                else if (@checked)
                    op = OpCodes.Conv_Ovf_I2;
                else
                    op = OpCodes.Conv_I2;
                break;
            case TypeCode.UInt16:
                if (unsigned & @checked)
                    op = OpCodes.Conv_Ovf_U2_Un;
                else if (@checked)
                    op = OpCodes.Conv_Ovf_U2;
                else
                    op = OpCodes.Conv_U2;
                break;
            case TypeCode.Int32:
                if (unsigned & @checked)
                    op = OpCodes.Conv_Ovf_I4_Un;
                else if (@checked)
                    op = OpCodes.Conv_Ovf_I4;
                else if (sourcetc != TypeCode.UInt32)
                    // Don't need to emit a convert for this case since, to the CLR, it is the same data type
                    op = OpCodes.Conv_I4;
                break;
            case TypeCode.UInt32:
                if (unsigned & @checked)
                    op = OpCodes.Conv_Ovf_U4_Un;
                else if (@checked)
                    op = OpCodes.Conv_Ovf_U4;
                else if (sourcetc != TypeCode.Int32) op = OpCodes.Conv_U4;
                break;
            case TypeCode.Int64:
                if (unsigned & @checked)
                    op = OpCodes.Conv_Ovf_I8_Un;
                else if (@checked)
                    op = OpCodes.Conv_Ovf_I8;
                else if (sourcetc != TypeCode.UInt64) op = OpCodes.Conv_I8;
                break;
            case TypeCode.UInt64:
                if (unsigned & @checked)
                    op = OpCodes.Conv_Ovf_U8_Un;
                else if (@checked)
                    op = OpCodes.Conv_Ovf_U8;
                else if (sourcetc != TypeCode.Int64) op = OpCodes.Conv_U8;
                break;
            case TypeCode.Single:
                op = OpCodes.Conv_R4;
                break;
            default:
                Debug.Assert(false, "Unknown cast dest type");
                break;
        }

        if (op.Equals(OpCodes.Nop) == false) ilg.Emit(op);
    }

    private static bool IsUnsignedType(Type t)
    {
        var tc = Type.GetTypeCode(t);
        switch (tc)
        {
            case TypeCode.Byte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                return true;
            default:
                return false;
        }
    }
}