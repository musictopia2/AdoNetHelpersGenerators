namespace AdoNetHelpersGenerators.SingleTableQueries;
internal record PropertyModel : ICustomProperty
{
    public EnumSimpleTypeCategory VariableCustomCategory { get; set; }
    public bool Nullable { get; set; }
    public string PropertyName { get; set; } = "";
    public string ContainingNameSpace { get; set; } = "";
    public string UnderlyingSymbolName { get; set; } = "";
    public string ColumnName { get; set; } = ""; //the column name could be different
}