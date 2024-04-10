namespace AdoNetHelpersGenerators.JoinedTableQueries;
internal record ResultsModel : ICustomResult
{
    public string ClassName { get; set; } = "";
    public string Namespace { get; set; } = "";
    public string FullName => $"{Namespace}.{ClassName}";
    public string TableName { get; set; } = "";
    public BasicList<JoinedGenericModel> Joins { get; set; } = [];
    public BasicList<PropertyModel> Properties { get; set; } = [];
}