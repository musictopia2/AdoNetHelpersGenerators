namespace AdoNetHelpersGenerators.Mappers;
public record ResultsModel : ICustomResult
{
    public string ClassName { get; set; } = "";
    public string Namespace { get; set; } = "";
    public BasicList<PropertyModel> Properties { get; set; } = [];
    public string TableName { get; set; } = "";
    public bool AutoIncremented { get; set; }
}