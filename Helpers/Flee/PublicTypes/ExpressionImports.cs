using System;
using System.Collections.Generic;
using System.Reflection;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.Resources;

namespace ScriptedEventsAPI.Helpers.Flee.PublicTypes;

public sealed class ExpressionImports
{
    private static readonly Dictionary<string, Type> OurBuiltinTypeMap = CreateBuiltinTypeMap();

    private ExpressionContext MyContext;
    private TypeImport MyOwnerImport;

    internal ExpressionImports()
    {
        RootImport = new NamespaceImport("true");
    }

    #region "Properties - Public"

    public NamespaceImport RootImport { get; private set; }

    #endregion

    private static Dictionary<string, Type> CreateBuiltinTypeMap()
    {
        Dictionary<string, Type> map = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        map.Add("boolean", typeof(bool));
        map.Add("byte", typeof(byte));
        map.Add("sbyte", typeof(sbyte));
        map.Add("short", typeof(short));
        map.Add("ushort", typeof(ushort));
        map.Add("int", typeof(int));
        map.Add("uint", typeof(uint));
        map.Add("long", typeof(long));
        map.Add("ulong", typeof(ulong));
        map.Add("single", typeof(float));
        map.Add("double", typeof(double));
        map.Add("decimal", typeof(decimal));
        map.Add("char", typeof(char));
        map.Add("object", typeof(object));
        map.Add("string", typeof(string));

        return map;
    }

    #region "Methods - Non public"

    internal void SetContext(ExpressionContext context)
    {
        MyContext = context;
        RootImport.SetContext(context);
    }

    internal ExpressionImports Clone()
    {
        var copy = new ExpressionImports();

        copy.RootImport = (NamespaceImport)RootImport.Clone();
        copy.MyOwnerImport = MyOwnerImport;

        return copy;
    }

    internal void ImportOwner(Type ownerType)
    {
        MyOwnerImport = new TypeImport(ownerType,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, false);
        MyOwnerImport.SetContext(MyContext);
    }

    internal bool HasNamespace(string ns)
    {
        var import = RootImport.FindImport(ns) as NamespaceImport;
        return import != null;
    }

    internal NamespaceImport GetImport(string ns)
    {
        if (ns.Length == 0) return RootImport;

        var import = RootImport.FindImport(ns) as NamespaceImport;

        if (import == null)
        {
            import = new NamespaceImport(ns);
            RootImport.Add(import);
        }

        return import;
    }

    internal MemberInfo[] FindOwnerMembers(string memberName, MemberTypes memberType)
    {
        return MyOwnerImport.FindMembers(memberName, memberType);
    }

    internal Type FindType(string[] typeNameParts)
    {
        string[] namespaces = new string[typeNameParts.Length - 1];
        var typeName = typeNameParts[typeNameParts.Length - 1];

        Array.Copy(typeNameParts, namespaces, namespaces.Length);
        ImportBase currentImport = RootImport;

        foreach (var ns in namespaces)
        {
            currentImport = currentImport.FindImport(ns);
            if (currentImport == null) break; // TODO: might not be correct. Was : Exit For
        }

        return currentImport?.FindType(typeName);
    }

    internal static Type GetBuiltinType(string name)
    {
        Type t = null;

        if (OurBuiltinTypeMap.TryGetValue(name, out t)) return t;

        return null;
    }

    #endregion

    #region "Methods - Public"

    public void AddType(Type t, string ns)
    {
        Utility.AssertNotNull(t, "t");
        Utility.AssertNotNull(ns, "namespace");

        MyContext.AssertTypeIsAccessible(t);

        var import = GetImport(ns);
        import.Add(new TypeImport(t, BindingFlags.Public | BindingFlags.Static, false));
    }

    public void AddType(Type t)
    {
        AddType(t, string.Empty);
    }

    public void AddMethod(string methodName, Type t, string ns)
    {
        Utility.AssertNotNull(methodName, "methodName");
        Utility.AssertNotNull(t, "t");
        Utility.AssertNotNull(ns, "namespace");

        var mi = t.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);

        if (mi == null)
        {
            var msg = Utility.GetGeneralErrorMessage(GeneralErrorResourceKeys.CouldNotFindPublicStaticMethodOnType,
                methodName, t.Name);
            throw new ArgumentException(msg);
        }

        AddMethod(mi, ns);
    }

    public void AddMethod(MethodInfo mi, string ns)
    {
        Utility.AssertNotNull(mi, "mi");
        Utility.AssertNotNull(ns, "namespace");

        MyContext.AssertTypeIsAccessible(mi.ReflectedType);

        if ((mi.IsStatic == false) | (mi.IsPublic == false))
        {
            var msg = Utility.GetGeneralErrorMessage(GeneralErrorResourceKeys.OnlyPublicStaticMethodsCanBeImported);
            throw new ArgumentException(msg);
        }

        var import = GetImport(ns);
        import.Add(new MethodImport(mi));
    }

    public void ImportBuiltinTypes()
    {
        foreach (KeyValuePair<string, Type> pair in OurBuiltinTypeMap) AddType(pair.Value, pair.Key);
    }

    #endregion
}