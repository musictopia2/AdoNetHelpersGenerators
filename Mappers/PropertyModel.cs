namespace AdoNetHelpersGenerators.Mappers;
public class PropertyModel : ICustomProperty
{
    public EnumSimpleTypeCategory VariableCustomCategory { get; set; }
    public bool Nullable { get; set; }
    public string PropertyName { get; set; } = "";
    public string ContainingNameSpace { get; set; } = "";
    public string UnderlyingSymbolName { get; set; } = "";
    public bool NoIncrement { get; set; }
    public bool CommonForUpdating { get; set; }
    public bool IsIDField { get; set; }
    public bool UseForPossibleJoin { get; set; }
    public string ColumnName { get; set; } = "";
    public string ForeignKey { get; set; } = "";
}