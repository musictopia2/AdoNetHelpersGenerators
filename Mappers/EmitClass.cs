namespace AdoNetHelpersGenerators.Mappers;
internal class EmitClass(ImmutableArray<ResultsModel> results, SourceProductionContext context)
{
    public void Emit()
    {
        foreach (var item in results)
        {
            WriteItem(item);
        }
    }
    private void WriteItem(ResultsModel item)
    {
        SourceCodeStringBuilder builder = new();
        builder.StartPartialClassImplements(item, w =>
        {
            w.Write(": ")
            .PopulateITableMapperStart(item.ClassName);
        }, w =>
        {
            PopulateDetails(w, item);
        });
        context.AddSource($"{item.ClassName}.SqlMapping.g.cs", builder.ToString());
    }
    private void PopulateDetails(ICodeBlock w, ResultsModel result)
    {
        w.PopulateSimpleMethods(result)
            .PopulatePrivateMaps(result);
    }
}