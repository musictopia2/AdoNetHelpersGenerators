namespace AdoNetHelpersGenerators.JoinedTableQueries;
public record PropertyModel : ICustomProperty
{
    public EnumSimpleTypeCategory VariableCustomCategory { get; set; }
    public bool Nullable { get; set; }
    public string PropertyName { get; set; } = "";
    public string ContainingNameSpace { get; set; } = "";
    public string UnderlyingSymbolName { get; set; } = "";
    public bool IsIDField { get; set; }
    public string ForeignTableName { get; set; } = "";
    public int Index { get; set; }
}