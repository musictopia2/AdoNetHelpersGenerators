namespace AdoNetHelpersGenerators.Objects;
internal record PropertyModel : ICustomProperty
{
    public EnumSimpleTypeCategory VariableCustomCategory { get; set; }
    public bool Nullable { get; set; }
    public string PropertyName { get; set; } = "";
    public string ContainingNameSpace { get; set; } = "";
    public string UnderlyingSymbolName { get; set; } = "";
    public string ScalarInfo { get; set; } = "";
    public string QueryMethod { get; set; } = "";
    public string GenericObjectName { get; set; } = "";
    public string ReturnType { get; set; } = "";
}