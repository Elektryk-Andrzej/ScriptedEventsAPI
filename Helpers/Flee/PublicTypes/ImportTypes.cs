using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.Resources;

namespace ScriptedEventsAPI.Helpers.Flee.PublicTypes;

public abstract class ImportBase : IEnumerable<ImportBase>, IEquatable<ImportBase>
{
    internal ImportBase()
    {
    }

    #region "Properties - Protected"

    protected ExpressionContext Context { get; private set; }

    #endregion

    #region "Methods - Public"

    public MemberInfo[] GetMembers(MemberTypes memberType)
    {
        List<MemberInfo> found = new List<MemberInfo>();
        AddMembers(memberType, found);
        return found.ToArray();
    }

    #endregion

    #region "Methods - Non Public"

    internal virtual void SetContext(ExpressionContext context)
    {
        Context = context;
        Validate();
    }

    internal abstract void Validate();

    protected abstract void AddMembers(string memberName, MemberTypes memberType, ICollection<MemberInfo> dest);
    protected abstract void AddMembers(MemberTypes memberType, ICollection<MemberInfo> dest);

    internal ImportBase Clone()
    {
        return (ImportBase)MemberwiseClone();
    }

    protected static void AddImportMembers(ImportBase import, string memberName, MemberTypes memberType,
        ICollection<MemberInfo> dest)
    {
        import.AddMembers(memberName, memberType, dest);
    }

    protected static void AddImportMembers(ImportBase import, MemberTypes memberType, ICollection<MemberInfo> dest)
    {
        import.AddMembers(memberType, dest);
    }

    protected static void AddMemberRange(ICollection<MemberInfo> members, ICollection<MemberInfo> dest)
    {
        foreach (var mi in members) dest.Add(mi);
    }

    protected bool AlwaysMemberFilter(MemberInfo member, object criteria)
    {
        return true;
    }

    internal abstract bool IsMatch(string name);
    internal abstract Type FindType(string typename);

    internal virtual ImportBase FindImport(string name)
    {
        return null;
    }

    internal MemberInfo[] FindMembers(string memberName, MemberTypes memberType)
    {
        List<MemberInfo> found = new List<MemberInfo>();
        AddMembers(memberName, memberType, found);
        return found.ToArray();
    }

    #endregion

    #region "IEnumerable Implementation"

    public virtual IEnumerator<ImportBase> GetEnumerator()
    {
        List<ImportBase> coll = new List<ImportBase>();
        return coll.GetEnumerator();
    }

    private IEnumerator GetEnumerator1()
    {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator1();
    }

    #endregion

    #region "IEquatable Implementation"

    public bool Equals(ImportBase other)
    {
        return EqualsInternal(other);
    }

    protected abstract bool EqualsInternal(ImportBase import);

    #endregion

    #region "Properties - Public"

    public abstract string Name { get; }

    public virtual bool IsContainer => false;

    #endregion
}

public sealed class TypeImport : ImportBase
{
    private readonly BindingFlags _myBindFlags;

    public TypeImport(Type importType) : this(importType, false)
    {
    }

    public TypeImport(Type importType, bool useTypeNameAsNamespace) : this(importType,
        BindingFlags.Public | BindingFlags.Static, useTypeNameAsNamespace)
    {
    }

    #region "Methods - Public"

    public override IEnumerator<ImportBase> GetEnumerator()
    {
        if (IsContainer)
        {
            List<ImportBase> coll = new List<ImportBase>();
            coll.Add(new TypeImport(Target, false));
            return coll.GetEnumerator();
        }

        return base.GetEnumerator();
    }

    #endregion

    #region "Methods - Non Public"

    internal TypeImport(Type t, BindingFlags flags, bool useTypeNameAsNamespace)
    {
        Utility.AssertNotNull(t, "t");
        Target = t;
        _myBindFlags = flags;
        IsContainer = useTypeNameAsNamespace;
    }

    internal override void Validate()
    {
        Context.AssertTypeIsAccessible(Target);
    }

    protected override void AddMembers(string memberName, MemberTypes memberType, ICollection<MemberInfo> dest)
    {
        MemberInfo[] members = Target.FindMembers(memberType, _myBindFlags, Context.Options.MemberFilter, memberName);
        AddMemberRange(members, dest);
    }

    protected override void AddMembers(MemberTypes memberType, ICollection<MemberInfo> dest)
    {
        if (IsContainer == false)
        {
            MemberInfo[] members = Target.FindMembers(memberType, _myBindFlags, AlwaysMemberFilter, null);
            AddMemberRange(members, dest);
        }
    }

    internal override bool IsMatch(string name)
    {
        if (IsContainer) return string.Equals(Target.Name, name, Context.Options.MemberStringComparison);

        return false;
    }

    internal override Type FindType(string typeName)
    {
        if (string.Equals(typeName, Target.Name, Context.Options.MemberStringComparison)) return Target;

        return null;
    }

    protected override bool EqualsInternal(ImportBase import)
    {
        var otherSameType = import as TypeImport;
        return otherSameType != null && ReferenceEquals(Target, otherSameType.Target);
    }

    #endregion

    #region "Properties - Public"

    public override bool IsContainer { get; }

    public override string Name => Target.Name;

    public Type Target { get; }

    #endregion
}

public sealed class MethodImport : ImportBase
{
    public MethodImport(MethodInfo importMethod)
    {
        Utility.AssertNotNull(importMethod, "importMethod");
        Target = importMethod;
    }

    public override string Name => Target.Name;

    public MethodInfo Target { get; }

    internal override void Validate()
    {
        Context.AssertTypeIsAccessible(Target.ReflectedType);
    }

    protected override void AddMembers(string memberName, MemberTypes memberType, ICollection<MemberInfo> dest)
    {
        if (string.Equals(memberName, Target.Name, Context.Options.MemberStringComparison) &&
            (memberType & MemberTypes.Method) != 0) dest.Add(Target);
    }

    protected override void AddMembers(MemberTypes memberType, ICollection<MemberInfo> dest)
    {
        if ((memberType & MemberTypes.Method) != 0) dest.Add(Target);
    }

    internal override bool IsMatch(string name)
    {
        return string.Equals(Target.Name, name, Context.Options.MemberStringComparison);
    }

    internal override Type FindType(string typeName)
    {
        return null;
    }

    protected override bool EqualsInternal(ImportBase import)
    {
        var otherSameType = import as MethodImport;
        return otherSameType != null && Target.MethodHandle.Equals(otherSameType.Target.MethodHandle);
    }
}

public sealed class NamespaceImport : ImportBase, ICollection<ImportBase>
{
    private readonly List<ImportBase> _myImports;

    public NamespaceImport(string importNamespace)
    {
        Utility.AssertNotNull(importNamespace, "importNamespace");
        if (importNamespace.Length == 0)
        {
            var msg = Utility.GetGeneralErrorMessage(GeneralErrorResourceKeys.InvalidNamespaceName);
            throw new ArgumentException(msg);
        }

        Name = importNamespace;
        _myImports = new List<ImportBase>();
    }

    private ICollection<ImportBase> NonContainerImports
    {
        get
        {
            List<ImportBase> found = new List<ImportBase>();

            foreach (var import in _myImports)
                if (import.IsContainer == false)
                    found.Add(import);

            return found;
        }
    }

    public override bool IsContainer => true;

    public override string Name { get; }

    internal override void SetContext(ExpressionContext context)
    {
        base.SetContext(context);

        foreach (var import in _myImports) import.SetContext(context);
    }

    internal override void Validate()
    {
    }

    protected override void AddMembers(string memberName, MemberTypes memberType, ICollection<MemberInfo> dest)
    {
        foreach (var import in NonContainerImports) AddImportMembers(import, memberName, memberType, dest);
    }

    protected override void AddMembers(MemberTypes memberType, ICollection<MemberInfo> dest)
    {
    }

    internal override Type FindType(string typeName)
    {
        foreach (var import in NonContainerImports)
        {
            var t = import.FindType(typeName);

            if (t != null) return t;
        }

        return null;
    }

    internal override ImportBase FindImport(string name)
    {
        foreach (var import in _myImports)
            if (import.IsMatch(name))
                return import;

        return null;
    }

    internal override bool IsMatch(string name)
    {
        return string.Equals(Name, name, Context.Options.MemberStringComparison);
    }

    protected override bool EqualsInternal(ImportBase import)
    {
        var otherSameType = import as NamespaceImport;
        return otherSameType != null && Name.Equals(otherSameType.Name, Context.Options.MemberStringComparison);
    }

    #region "ICollection implementation"

    public void Add(ImportBase item)
    {
        Utility.AssertNotNull(item, "item");

        if (Context != null) item.SetContext(Context);

        _myImports.Add(item);
    }

    public void Clear()
    {
        _myImports.Clear();
    }

    public bool Contains(ImportBase item)
    {
        return _myImports.Contains(item);
    }

    public void CopyTo(ImportBase[] array, int arrayIndex)
    {
        _myImports.CopyTo(array, arrayIndex);
    }

    public bool Remove(ImportBase item)
    {
        return _myImports.Remove(item);
    }

    public override IEnumerator<ImportBase> GetEnumerator()
    {
        return _myImports.GetEnumerator();
    }

    public int Count => _myImports.Count;

    public bool IsReadOnly => false;

    #endregion
}