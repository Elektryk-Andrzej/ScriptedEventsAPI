using System;
using System.Diagnostics;
using System.Reflection.Emit;

namespace ScriptedEventsAPI.Helpers.Flee.InternalTypes;

internal class ImplicitConverter
{
    /// <summary>
    ///     Table of results for binary operations using primitives
    /// </summary>
    private static readonly Type[,] OurBinaryResultTable;

    /// <summary>
    ///     Primitive types we support
    /// </summary>
    private static readonly Type[] OurBinaryTypes;

    static ImplicitConverter()
    {
        // Create a table with all the primitive types
        Type[] types =
        {
            typeof(char),
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double)
        };
        OurBinaryTypes = types;
        Type[,] table = new Type[types.Length, types.Length];
        OurBinaryResultTable = table;
        FillIdentities(types, table);

        // Fill the table
        AddEntry(typeof(uint), typeof(ulong), typeof(ulong));
        AddEntry(typeof(int), typeof(long), typeof(long));
        AddEntry(typeof(uint), typeof(long), typeof(long));
        AddEntry(typeof(int), typeof(uint), typeof(long));
        AddEntry(typeof(uint), typeof(float), typeof(float));
        AddEntry(typeof(uint), typeof(double), typeof(double));
        AddEntry(typeof(int), typeof(float), typeof(float));
        AddEntry(typeof(int), typeof(double), typeof(double));
        AddEntry(typeof(long), typeof(float), typeof(float));
        AddEntry(typeof(long), typeof(double), typeof(double));
        AddEntry(typeof(ulong), typeof(float), typeof(float));
        AddEntry(typeof(ulong), typeof(double), typeof(double));
        AddEntry(typeof(float), typeof(double), typeof(double));

        // Byte
        AddEntry(typeof(byte), typeof(byte), typeof(int));
        AddEntry(typeof(byte), typeof(sbyte), typeof(int));
        AddEntry(typeof(byte), typeof(short), typeof(int));
        AddEntry(typeof(byte), typeof(ushort), typeof(int));
        AddEntry(typeof(byte), typeof(int), typeof(int));
        AddEntry(typeof(byte), typeof(uint), typeof(uint));
        AddEntry(typeof(byte), typeof(long), typeof(long));
        AddEntry(typeof(byte), typeof(ulong), typeof(ulong));
        AddEntry(typeof(byte), typeof(float), typeof(float));
        AddEntry(typeof(byte), typeof(double), typeof(double));

        // SByte
        AddEntry(typeof(sbyte), typeof(sbyte), typeof(int));
        AddEntry(typeof(sbyte), typeof(short), typeof(int));
        AddEntry(typeof(sbyte), typeof(ushort), typeof(int));
        AddEntry(typeof(sbyte), typeof(int), typeof(int));
        AddEntry(typeof(sbyte), typeof(uint), typeof(long));
        AddEntry(typeof(sbyte), typeof(long), typeof(long));
        //invalid -- AddEntry(GetType(SByte), GetType(UInt64), GetType(UInt64))
        AddEntry(typeof(sbyte), typeof(float), typeof(float));
        AddEntry(typeof(sbyte), typeof(double), typeof(double));

        // int16
        AddEntry(typeof(short), typeof(short), typeof(int));
        AddEntry(typeof(short), typeof(ushort), typeof(int));
        AddEntry(typeof(short), typeof(int), typeof(int));
        AddEntry(typeof(short), typeof(uint), typeof(long));
        AddEntry(typeof(short), typeof(long), typeof(long));
        //invalid -- AddEntry(GetType(Int16), GetType(UInt64), GetType(UInt64))
        AddEntry(typeof(short), typeof(float), typeof(float));
        AddEntry(typeof(short), typeof(double), typeof(double));

        // Uint16
        AddEntry(typeof(ushort), typeof(ushort), typeof(int));
        AddEntry(typeof(ushort), typeof(short), typeof(int));
        AddEntry(typeof(ushort), typeof(int), typeof(int));
        AddEntry(typeof(ushort), typeof(uint), typeof(uint));
        AddEntry(typeof(ushort), typeof(long), typeof(long));
        AddEntry(typeof(ushort), typeof(ulong), typeof(ulong));
        AddEntry(typeof(ushort), typeof(float), typeof(float));
        AddEntry(typeof(ushort), typeof(double), typeof(double));

        // Char
        AddEntry(typeof(char), typeof(char), typeof(int));
        AddEntry(typeof(char), typeof(ushort), typeof(ushort));
        AddEntry(typeof(char), typeof(int), typeof(int));
        AddEntry(typeof(char), typeof(uint), typeof(uint));
        AddEntry(typeof(char), typeof(long), typeof(long));
        AddEntry(typeof(char), typeof(ulong), typeof(ulong));
        AddEntry(typeof(char), typeof(float), typeof(float));
        AddEntry(typeof(char), typeof(double), typeof(double));
    }

    private ImplicitConverter()
    {
    }

    private static void FillIdentities(Type[] typeList, Type[,] table)
    {
        for (var i = 0; i <= typeList.Length - 1; i++)
        {
            var t = typeList[i];
            table[i, i] = t;
        }
    }

    private static void AddEntry(Type t1, Type t2, Type result)
    {
        var index1 = GetTypeIndex(t1);
        var index2 = GetTypeIndex(t2);
        OurBinaryResultTable[index1, index2] = result;
        OurBinaryResultTable[index2, index1] = result;
    }

    private static int GetTypeIndex(Type t)
    {
        return Array.IndexOf(OurBinaryTypes, t);
    }

    public static bool EmitImplicitConvert(Type sourceType, Type destType, FleeILGenerator ilg)
    {
        if (ReferenceEquals(sourceType, destType)) return true;

        if (EmitOverloadedImplicitConvert(sourceType, destType, ilg)) return true;

        if (ImplicitConvertToReferenceType(sourceType, destType, ilg)) return true;

        return ImplicitConvertToValueType(sourceType, destType, ilg);
    }

    private static bool EmitOverloadedImplicitConvert(Type sourceType, Type destType, FleeILGenerator ilg)
    {
        // Look for an implicit operator on the destination type
        var mi = Utility.GetSimpleOverloadedOperator("Implicit", sourceType, destType);

        if (mi == null)
            // No match
            return false;

        if (ilg != null) ilg.Emit(OpCodes.Call, mi);

        return true;
    }

    private static bool ImplicitConvertToReferenceType(Type sourceType, Type destType, FleeILGenerator ilg)
    {
        if (destType.IsValueType) return false;

        if (ReferenceEquals(sourceType, typeof(Null)))
            // Null is always convertible to a reference type
            return true;

        if (destType.IsAssignableFrom(sourceType) == false) return false;

        if (sourceType.IsValueType)
            if (ilg != null)
                ilg.Emit(OpCodes.Box, sourceType);

        return true;
    }

    private static bool ImplicitConvertToValueType(Type sourceType, Type destType, FleeILGenerator ilg)
    {
        // We only handle value types
        if ((sourceType.IsValueType == false) & (destType.IsValueType == false)) return false;

        // No implicit conversion to enum.  Have to do this check here since calling GetTypeCode on an enum will return the typecode
        // of the underlying type which screws us up.
        if (sourceType.IsEnum | destType.IsEnum) return false;

        return EmitImplicitNumericConvert(sourceType, destType, ilg);
    }

    /// <summary>
    ///     Emit an implicit conversion (if the ilg is not null) and returns a value that determines whether the implicit
    ///     conversion
    ///     succeeded
    /// </summary>
    /// <param name="sourceType"></param>
    /// <param name="destType"></param>
    /// <param name="ilg"></param>
    /// <returns></returns>
    public static bool EmitImplicitNumericConvert(Type sourceType, Type destType, FleeILGenerator ilg)
    {
        var sourceTypeCode = Type.GetTypeCode(sourceType);
        var destTypeCode = Type.GetTypeCode(destType);

        switch (destTypeCode)
        {
            case TypeCode.Int16:
                return ImplicitConvertToInt16(sourceTypeCode, ilg);
            case TypeCode.UInt16:
                return ImplicitConvertToUInt16(sourceTypeCode, ilg);
            case TypeCode.Int32:
                return ImplicitConvertToInt32(sourceTypeCode, ilg);
            case TypeCode.UInt32:
                return ImplicitConvertToUInt32(sourceTypeCode, ilg);
            case TypeCode.Double:
                return ImplicitConvertToDouble(sourceTypeCode, ilg);
            case TypeCode.Single:
                return ImplicitConvertToSingle(sourceTypeCode, ilg);
            case TypeCode.Int64:
                return ImplicitConvertToInt64(sourceTypeCode, ilg);
            case TypeCode.UInt64:
                return ImplicitConvertToUInt64(sourceTypeCode, ilg);
            default:
                return false;
        }
    }


    private static bool ImplicitConvertToInt16(TypeCode sourceTypeCode, FleeILGenerator ilg)
    {
        switch (sourceTypeCode)
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.Int16:
                return true;
            default:
                return false;
        }
    }

    private static bool ImplicitConvertToUInt16(TypeCode sourceTypeCode, FleeILGenerator ilg)
    {
        switch (sourceTypeCode)
        {
            case TypeCode.Char:
            case TypeCode.Byte:
            case TypeCode.UInt16:
                return true;
            default:
                return false;
        }
    }

    private static bool ImplicitConvertToInt32(TypeCode sourceTypeCode, FleeILGenerator ilg)
    {
        switch (sourceTypeCode)
        {
            case TypeCode.Char:
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
                return true;
            default:
                return false;
        }
    }

    private static bool ImplicitConvertToUInt32(TypeCode sourceTypeCode, FleeILGenerator ilg)
    {
        switch (sourceTypeCode)
        {
            case TypeCode.Char:
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
                return true;
            default:
                return false;
        }
    }

    private static bool ImplicitConvertToDouble(TypeCode sourceTypeCode, FleeILGenerator ilg)
    {
        switch (sourceTypeCode)
        {
            case TypeCode.Char:
            case TypeCode.SByte:
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.Single:
            case TypeCode.Int64:
                EmitConvert(ilg, OpCodes.Conv_R8);
                break;
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                EmitConvert(ilg, OpCodes.Conv_R_Un);
                EmitConvert(ilg, OpCodes.Conv_R8);
                break;
            case TypeCode.Double:
                break;
            default:
                return false;
        }

        return true;
    }

    private static bool ImplicitConvertToSingle(TypeCode sourceTypeCode, FleeILGenerator ilg)
    {
        switch (sourceTypeCode)
        {
            case TypeCode.Char:
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.Int64:
                EmitConvert(ilg, OpCodes.Conv_R4);
                break;
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                EmitConvert(ilg, OpCodes.Conv_R_Un);
                EmitConvert(ilg, OpCodes.Conv_R4);
                break;
            case TypeCode.Single:
                break;
            default:
                return false;
        }

        return true;
    }

    private static bool ImplicitConvertToInt64(TypeCode sourceTypeCode, FleeILGenerator ilg)
    {
        switch (sourceTypeCode)
        {
            case TypeCode.SByte:
            case TypeCode.Int16:
            case TypeCode.Int32:
                EmitConvert(ilg, OpCodes.Conv_I8);
                break;
            case TypeCode.Char:
            case TypeCode.Byte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
                EmitConvert(ilg, OpCodes.Conv_U8);
                break;
            case TypeCode.Int64:
                break;
            default:
                return false;
        }

        return true;
    }

    private static bool ImplicitConvertToUInt64(TypeCode sourceTypeCode, FleeILGenerator ilg)
    {
        switch (sourceTypeCode)
        {
            case TypeCode.Char:
            case TypeCode.Byte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
                EmitConvert(ilg, OpCodes.Conv_U8);
                break;
            case TypeCode.UInt64:
                break;
            default:
                return false;
        }

        return true;
    }

    private static void EmitConvert(FleeILGenerator ilg, OpCode convertOpcode)
    {
        if (ilg != null) ilg.Emit(convertOpcode);
    }

    /// <summary>
    ///     Get the result type for a binary operation
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <returns></returns>
    public static Type GetBinaryResultType(Type t1, Type t2)
    {
        var index1 = GetTypeIndex(t1);
        var index2 = GetTypeIndex(t2);

        if ((index1 == -1) | (index2 == -1)) return null;

        return OurBinaryResultTable[index1, index2];
    }

    public static int GetImplicitConvertScore(Type sourceType, Type destType)
    {
        if (ReferenceEquals(sourceType, destType)) return 0;

        if (ReferenceEquals(sourceType, typeof(Null))) return GetInverseDistanceToObject(destType);

        if (Utility.GetSimpleOverloadedOperator("Implicit", sourceType, destType) != null)
            // Implicit operator conversion, score it at 1 so it's just above the minimum
            return 1;

        if (sourceType.IsValueType)
        {
            if (destType.IsValueType)
            {
                // Value type -> value type
                var sourceScore = GetValueTypeImplicitConvertScore(sourceType);
                var destScore = GetValueTypeImplicitConvertScore(destType);

                return destScore - sourceScore;
            }

            // Value type -> reference type
            return GetReferenceTypeImplicitConvertScore(sourceType, destType);
        }

        if (destType.IsValueType)
            // Reference type -> value type
            // Reference types can never be implicitly converted to value types
            Debug.Fail("No implicit conversion from reference type to value type");
        else
            // Reference type -> reference type
            return GetReferenceTypeImplicitConvertScore(sourceType, destType);
        return 0;
    }

    private static int GetValueTypeImplicitConvertScore(Type t)
    {
        var tc = Type.GetTypeCode(t);

        switch (tc)
        {
            case TypeCode.Byte:
                return 1;
            case TypeCode.SByte:
                return 2;
            case TypeCode.Char:
                return 3;
            case TypeCode.Int16:
                return 4;
            case TypeCode.UInt16:
                return 5;
            case TypeCode.Int32:
                return 6;
            case TypeCode.UInt32:
                return 7;
            case TypeCode.Int64:
                return 8;
            case TypeCode.UInt64:
                return 9;
            case TypeCode.Single:
                return 10;
            case TypeCode.Double:
                return 11;
            case TypeCode.Decimal:
                return 11;
            case TypeCode.Boolean:
                return 12;
            case TypeCode.DateTime:
                return 13;
            default:
                Debug.Assert(false, "unknown value type");
                return -1;
        }
    }

    private static int GetReferenceTypeImplicitConvertScore(Type sourceType, Type destType)
    {
        if (destType.IsInterface) return 100;

        return GetInheritanceDistance(sourceType, destType);
    }

    private static int GetInheritanceDistance(Type sourceType, Type destType)
    {
        var count = 0;
        var current = sourceType;

        while (!ReferenceEquals(current, destType))
        {
            count += 1;
            current = current.BaseType;
        }

        return count * 1000;
    }

    private static int GetInverseDistanceToObject(Type t)
    {
        var score = 1000;
        var current = t.BaseType;

        while (current != null)
        {
            score -= 100;
            current = current.BaseType;
        }

        return score;
    }
}