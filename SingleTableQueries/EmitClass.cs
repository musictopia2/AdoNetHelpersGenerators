namespace AdoNetHelpersGenerators.SingleTableQueries;
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
            w.PopulateInterface(item);
        }, w =>
        {
            PopulateDetails(w, item);
        });
        context.AddSource($"{item.ClassName}.CommandQuerySimple.g.cs", builder.ToString()); //change sample to what you want.
    }
    private void PopulateDetails(ICodeBlock w, ResultsModel result)
    {
        w.PopulateQuery(result)
            .PopulateQueryAsync(result)
            .PopulateReadItem(result);
    }
}