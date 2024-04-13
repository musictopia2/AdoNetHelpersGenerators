namespace AdoNetHelpersGenerators.JoinedTableQueries;
internal record JoinedGenericModel
{
    public BasicList<JoinedTableModel> Tables { get; set; } = [];
    public string FullInterfaceName { get; set; } = ""; //this would be used to loop to show implementing these interfaces.
    public string FullFunctionName { get; set; } = "";
    public string ClassName { get; set; } = ""; //this is needed for variable values.
}