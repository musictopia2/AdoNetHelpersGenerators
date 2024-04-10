namespace AdoNetHelpersGenerators.Objects;
internal record ResultsModel : ICustomResult
{
    public string ClassName { get; set; } = "";
    public string Namespace { get; set; } = "";
    public HashSet<PropertyModel> Properties { get; set; } = []; //hopefully does not have to be immutable (?)
}