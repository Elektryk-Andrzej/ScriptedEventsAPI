using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals.Integral;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals.Real;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;
using ScriptedEventsAPI.Helpers.Flee.Resources;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.MemberElements;

[Obsolete("Represents an identifier")]
internal class IdentifierElement : MemberElement
{
    private Type _myCalcEngineReferenceType;
    private FieldInfo _myField;
    private PropertyInfo _myProperty;
    private PropertyDescriptor _myPropertyDescriptor;
    private Type _myVariableType;

    public IdentifierElement(string name)
    {
        MyName = name;
    }

    private Type MemberOwnerType
    {
        get
        {
            if (_myField != null) return _myField.ReflectedType;

            if (_myPropertyDescriptor != null) return _myPropertyDescriptor.ComponentType;

            if (_myProperty != null) return _myProperty.ReflectedType;

            return null;
        }
    }

    public override Type ResultType
    {
        get
        {
            if (_myCalcEngineReferenceType != null) return _myCalcEngineReferenceType;

            if (_myVariableType != null) return _myVariableType;

            if (_myPropertyDescriptor != null) return _myPropertyDescriptor.PropertyType;

            if (_myField != null) return _myField.FieldType;

            var mi = _myProperty.GetGetMethod(true);
            return mi.ReturnType;
        }
    }

    protected override bool RequiresAddress => _myPropertyDescriptor == null;

    protected override bool IsPublic
    {
        get
        {
            if ((_myVariableType != null) | (_myCalcEngineReferenceType != null)) return true;

            if (_myVariableType != null) return true;

            if (_myPropertyDescriptor != null) return true;

            if (_myField != null) return _myField.IsPublic;

            var mi = _myProperty.GetGetMethod(true);
            return mi.IsPublic;
        }
    }

    protected override bool SupportsStatic
    {
        get
        {
            if (_myVariableType != null)
                // Variables never support static
                return false;

            if (_myPropertyDescriptor != null)
                // Neither do virtual properties
                return false;

            if (MyOptions.IsOwnerType(MemberOwnerType) && MyPrevious == null)
                // Owner members support static if we are the first element
                return true;

            // Support static if we are the first (ie: we are a static import)
            return MyPrevious == null;
        }
    }

    protected override bool SupportsInstance
    {
        get
        {
            if (_myVariableType != null)
                // Variables always support instance
                return true;

            if (_myPropertyDescriptor != null)
                // So do virtual properties
                return true;

            if (MyOptions.IsOwnerType(MemberOwnerType) && MyPrevious == null)
                // Owner members support instance if we are the first element
                return true;

            // We always support instance if we are not the first element
            return MyPrevious != null;
        }
    }

    public override bool IsStatic
    {
        get
        {
            if ((_myVariableType != null) | (_myCalcEngineReferenceType != null)) return false;

            if (_myVariableType != null) return false;

            if (_myField != null) return _myField.IsStatic;

            if (_myPropertyDescriptor != null) return false;

            var mi = _myProperty.GetGetMethod(true);
            return mi.IsStatic;
        }
    }

    public override bool IsExtensionMethod => false;

    protected override void ResolveInternal()
    {
        // Try to bind to a field or property
        if (ResolveFieldProperty(MyPrevious))
        {
            AddReferencedVariable(MyPrevious);
            return;
        }

        // Try to find a variable with our name
        _myVariableType = MyContext.Variables.GetVariableTypeInternal(MyName);

        // Variables are only usable as the first element
        if (MyPrevious == null && _myVariableType != null)
        {
            AddReferencedVariable(MyPrevious);
            return;
        }

        var ce = MyContext.CalculationEngine;

        if (ce != null)
        {
            ce.AddDependency(MyName, MyContext);
            _myCalcEngineReferenceType = ce.ResolveTailType(MyName);
            return;
        }

        if (MyPrevious == null)
            ThrowCompileException(CompileErrorResourceKeys.NoIdentifierWithName, CompileExceptionReason.UndefinedName,
                MyName);
        else
            ThrowCompileException(CompileErrorResourceKeys.NoIdentifierWithNameOnType,
                CompileExceptionReason.UndefinedName, MyName, MyPrevious.TargetType.Name);
    }

    private bool ResolveFieldProperty(MemberElement previous)
    {
        MemberInfo[] members = GetMembers(MemberTypes.Field | MemberTypes.Property);

        // Keep only the ones which are accessible
        members = GetAccessibleMembers(members);

        if (members.Length == 0)
            // No accessible members; try to resolve a virtual property
            return ResolveVirtualProperty(previous);

        if (members.Length > 1)
        {
            // More than one accessible member
            if (previous == null)
                ThrowCompileException(CompileErrorResourceKeys.IdentifierIsAmbiguous,
                    CompileExceptionReason.AmbiguousMatch, MyName);
            else
                ThrowCompileException(CompileErrorResourceKeys.IdentifierIsAmbiguousOnType,
                    CompileExceptionReason.AmbiguousMatch, MyName, previous.TargetType.Name);
        }
        else
        {
            // Only one member; bind to it
            _myField = members[0] as FieldInfo;
            if (_myField != null) return true;

            // Assume it must be a property
            _myProperty = (PropertyInfo)members[0];
            return true;
        }

        return false;
    }

    private bool ResolveVirtualProperty(MemberElement previous)
    {
        if (previous == null)
            // We can't use virtual properties if we are the first element
            return false;

        var coll = TypeDescriptor.GetProperties(previous.ResultType);
        _myPropertyDescriptor = coll.Find(MyName, true);
        return _myPropertyDescriptor != null;
    }

    private void AddReferencedVariable(MemberElement previous)
    {
        if (previous != null) return;

        if (_myVariableType != null || MyOptions.IsOwnerType(MemberOwnerType))
        {
            var info = (ExpressionInfo)MyServices.GetService(typeof(ExpressionInfo));
            info.AddReferencedVariable(MyName);
        }
    }

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        base.Emit(ilg, services);

        EmitFirst(ilg);

        if (_myCalcEngineReferenceType != null)
            EmitReferenceLoad(ilg);
        else if (_myVariableType != null)
            EmitVariableLoad(ilg);
        else if (_myField != null)
            EmitFieldLoad(_myField, ilg, services);
        else if (_myPropertyDescriptor != null)
            EmitVirtualPropertyLoad(ilg);
        else
            EmitPropertyLoad(_myProperty, ilg);
    }

    private void EmitReferenceLoad(FleeILGenerator ilg)
    {
        ilg.Emit(OpCodes.Ldarg_1);
        MyContext.CalculationEngine.EmitLoad(MyName, ilg);
    }

    private void EmitFirst(FleeILGenerator ilg)
    {
        if (MyPrevious != null) return;

        var isVariable = _myVariableType != null;

        if (isVariable)
            // Load variables
            EmitLoadVariables(ilg);
        else if (MyOptions.IsOwnerType(MemberOwnerType) & (IsStatic == false)) EmitLoadOwner(ilg);
    }

    private void EmitVariableLoad(FleeILGenerator ilg)
    {
        var mi = VariableCollection.GetVariableLoadMethod(_myVariableType);
        ilg.Emit(OpCodes.Ldstr, MyName);
        EmitMethodCall(mi, ilg);
    }

    private void EmitFieldLoad(FieldInfo fi, FleeILGenerator ilg, IServiceProvider services)
    {
        if (fi.IsLiteral)
            EmitLiteral(fi, ilg, services);
        else if (ResultType.IsValueType & NextRequiresAddress)
            EmitLdfld(fi, true, ilg);
        else
            EmitLdfld(fi, false, ilg);
    }

    private static void EmitLdfld(FieldInfo fi, bool indirect, FleeILGenerator ilg)
    {
        if (fi.IsStatic)
        {
            if (indirect)
                ilg.Emit(OpCodes.Ldsflda, fi);
            else
                ilg.Emit(OpCodes.Ldsfld, fi);
        }
        else
        {
            if (indirect)
                ilg.Emit(OpCodes.Ldflda, fi);
            else
                ilg.Emit(OpCodes.Ldfld, fi);
        }
    }

    /// <summary>
    ///     Emit the load of a constant field.  We can't emit a ldsfld/ldfld of a constant so we have to get its value
    ///     and then emit a ldc.
    /// </summary>
    /// <param name="fi"></param>
    /// <param name="ilg"></param>
    /// <param name="services"></param>
    private static void EmitLiteral(FieldInfo fi, FleeILGenerator ilg, IServiceProvider services)
    {
        var value = fi.GetValue(null);
        var t = value.GetType();
        var code = Type.GetTypeCode(t);
        var elem = default(LiteralElement);

        switch (code)
        {
            case TypeCode.Char:
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
                elem = new Int32LiteralElement(Convert.ToInt32(value));
                break;
            case TypeCode.UInt32:
                elem = new UInt32LiteralElement((uint)value);
                break;
            case TypeCode.Int64:
                elem = new Int64LiteralElement((long)value);
                break;
            case TypeCode.UInt64:
                elem = new UInt64LiteralElement((ulong)value);
                break;
            case TypeCode.Double:
                elem = new DoubleLiteralElement((double)value);
                break;
            case TypeCode.Single:
                elem = new SingleLiteralElement((float)value);
                break;
            case TypeCode.Boolean:
                elem = new BooleanLiteralElement((bool)value);
                break;
            case TypeCode.String:
                elem = new StringLiteralElement((string)value);
                break;
            default:
                elem = null;
                Debug.Fail("Unsupported constant type");
                break;
        }

        elem.Emit(ilg, services);
    }

    private void EmitPropertyLoad(PropertyInfo pi, FleeILGenerator ilg)
    {
        var getter = pi.GetGetMethod(true);
        EmitMethodCall(getter, ilg);
    }

    /// <summary>
    ///     Load a PropertyDescriptor based property
    /// </summary>
    /// <param name="ilg"></param>
    private void EmitVirtualPropertyLoad(FleeILGenerator ilg)
    {
        // The previous value is already on the top of the stack but we need it at the bottom

        // Get a temporary local index
        var index = ilg.GetTempLocalIndex(MyPrevious.ResultType);

        // Store the previous value there
        Utility.EmitStoreLocal(ilg, index);

        // Load the variable collection
        EmitLoadVariables(ilg);
        // Load the property name
        ilg.Emit(OpCodes.Ldstr, MyName);

        // Load the previous value and convert it to object
        Utility.EmitLoadLocal(ilg, index);
        ImplicitConverter.EmitImplicitConvert(MyPrevious.ResultType, typeof(object), ilg);

        // Call the method to get the actual value
        var mi = VariableCollection.GetVirtualPropertyLoadMethod(ResultType);
        EmitMethodCall(mi, ilg);
    }
}