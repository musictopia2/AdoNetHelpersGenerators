namespace AdoNetHelpersGenerators.JoinedTableQueries;
internal record JoinedTableModel : ICustomResult
{
    public string ClassName { get; set; } = "";
    public string Namespace { get; set; } = "";
    public string FullName => $"global::{Namespace}.{ClassName}";
    public string BoolVariableValue => $"has{ClassName}";
    public string ObjectVariableName => $"temp{ClassName}";
    public string TableName { get; set; } = ""; //this is needed to help with splitting.
    public string Instances { get; set; } = ""; //this is useful so when looping through, would be helpful for figuring out.
    public BasicList<PropertyModel> Properties { get; set; } = [];
}