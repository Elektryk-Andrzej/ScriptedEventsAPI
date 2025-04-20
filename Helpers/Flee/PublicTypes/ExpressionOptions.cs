using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;

namespace ScriptedEventsAPI.Helpers.Flee.PublicTypes;

public sealed class ExpressionOptions
{
    private readonly ExpressionContext _myOwner;

    private PropertyDictionary _myProperties;

    internal ExpressionOptions(ExpressionContext owner)
    {
        _myOwner = owner;
        _myProperties = new PropertyDictionary();

        InitializeProperties();
    }

    internal event EventHandler CaseSensitiveChanged;

    #region "Methods - Private"

    private void InitializeProperties()
    {
        StringComparison = StringComparison.Ordinal;
        OwnerMemberAccess = BindingFlags.Public;

        _myProperties.SetToDefault<bool>("CaseSensitive");
        _myProperties.SetToDefault<bool>("Checked");
        _myProperties.SetToDefault<bool>("EmitToAssembly");
        _myProperties.SetToDefault<Type>("ResultType");
        _myProperties.SetToDefault<bool>("IsGeneric");
        _myProperties.SetToDefault<bool>("IntegersAsDoubles");
        _myProperties.SetValue("ParseCulture", CultureInfo.CurrentCulture);
        SetParseCulture(ParseCulture);
        _myProperties.SetValue("RealLiteralDataType", RealLiteralDataType.Double);
    }

    private void SetParseCulture(CultureInfo ci)
    {
        var po = _myOwner.ParserOptions;
        po.DecimalSeparator = Convert.ToChar(ci.NumberFormat.NumberDecimalSeparator);
        po.FunctionArgumentSeparator = Convert.ToChar(ci.TextInfo.ListSeparator);
        po.DateTimeFormat = ci.DateTimeFormat.ShortDatePattern;
    }

    #endregion

    #region "Methods - Internal"

    internal ExpressionOptions Clone()
    {
        var clonedOptions = (ExpressionOptions)MemberwiseClone();
        clonedOptions._myProperties = _myProperties.Clone();
        return clonedOptions;
    }

    internal bool IsOwnerType(Type t)
    {
        return OwnerType.IsAssignableFrom(t);
    }

    internal void SetOwnerType(Type ownerType)
    {
        OwnerType = ownerType;
    }

    #endregion

    #region "Properties - Public"

    public Type ResultType
    {
        get => _myProperties.GetValue<Type>("ResultType");
        set
        {
            Utility.AssertNotNull(value, "value");
            _myProperties.SetValue("ResultType", value);
        }
    }

    public bool Checked
    {
        get => _myProperties.GetValue<bool>("Checked");
        set => _myProperties.SetValue("Checked", value);
    }

    public StringComparison StringComparison
    {
        get => _myProperties.GetValue<StringComparison>("StringComparison");
        set => _myProperties.SetValue("StringComparison", value);
    }

    public bool EmitToAssembly
    {
        get => _myProperties.GetValue<bool>("EmitToAssembly");
        set => _myProperties.SetValue("EmitToAssembly", value);
    }

    public BindingFlags OwnerMemberAccess
    {
        get => _myProperties.GetValue<BindingFlags>("OwnerMemberAccess");
        set => _myProperties.SetValue("OwnerMemberAccess", value);
    }

    public bool CaseSensitive
    {
        get => _myProperties.GetValue<bool>("CaseSensitive");
        set
        {
            if (CaseSensitive != value)
            {
                _myProperties.SetValue("CaseSensitive", value);
                if (CaseSensitiveChanged != null) CaseSensitiveChanged(this, EventArgs.Empty);
            }
        }
    }

    public bool IntegersAsDoubles
    {
        get => _myProperties.GetValue<bool>("IntegersAsDoubles");
        set => _myProperties.SetValue("IntegersAsDoubles", value);
    }

    public CultureInfo ParseCulture
    {
        get => _myProperties.GetValue<CultureInfo>("ParseCulture");
        set
        {
            Utility.AssertNotNull(value, "ParseCulture");
            if (value.LCID != ParseCulture.LCID)
            {
                _myProperties.SetValue("ParseCulture", value);
                SetParseCulture(value);
                _myOwner.ParserOptions.RecreateParser();
            }
        }
    }

    public RealLiteralDataType RealLiteralDataType
    {
        get => _myProperties.GetValue<RealLiteralDataType>("RealLiteralDataType");
        set => _myProperties.SetValue("RealLiteralDataType", value);
    }

    #endregion

    #region "Properties - Non Public"

    internal IEqualityComparer<string> StringComparer
    {
        get
        {
            if (CaseSensitive) return System.StringComparer.Ordinal;

            return System.StringComparer.OrdinalIgnoreCase;
        }
    }

    internal MemberFilter MemberFilter
    {
        get
        {
            if (CaseSensitive) return Type.FilterName;

            return Type.FilterNameIgnoreCase;
        }
    }

    internal StringComparison MemberStringComparison
    {
        get
        {
            if (CaseSensitive) return StringComparison.Ordinal;

            return StringComparison.OrdinalIgnoreCase;
        }
    }

    internal Type OwnerType { get; private set; }

    internal bool IsGeneric
    {
        get => _myProperties.GetValue<bool>("IsGeneric");
        set => _myProperties.SetValue("IsGeneric", value);
    }

    #endregion
}