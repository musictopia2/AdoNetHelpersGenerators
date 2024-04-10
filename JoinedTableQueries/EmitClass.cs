namespace AdoNetHelpersGenerators.JoinedTableQueries;
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
            w.Write(": ");

            StrCat cats = new();
            foreach (var g in item.Joins)
            {
                cats.AddToString($"global::AdoNetHelpersLibrary.ConnectionHelpers.ICommandQuery{g.FullInterfaceName}", ", ");
            }
            string information = cats.GetInfo();
            w.Write(information);
        }, w =>
        {
            PopulateDetails(w, item);
        });
        context.AddSource($"{item.ClassName}.CommandQueryJoins.g.cs", builder.ToString()); //change sample to what you want.
    }
    private void PopulateDetails(ICodeBlock w, ResultsModel result)
    {
        foreach (var item in result.Joins)
        {
            PopulateDetails(w, result, item);
        }
    }
    private void PopulateDetails(ICodeBlock w, ResultsModel result, JoinedGenericModel generic)
    {
        w.PopulateQuery(result, generic)
            .PopulateQueryAsync(result, generic)
            .PopulateReadItem(result, generic);
    }
}