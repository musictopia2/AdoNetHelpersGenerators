namespace AdoNetHelpersGenerators.Objects;
internal class EmitClass(ImmutableArray<ResultsModel> results, SourceProductionContext context)
{
    public void Emit()
    {
        foreach (var item in results)
        {
            WriteScalarItem(item);
            WriteQueryItem(item);
        }
    }
    private void WriteScalarItem(ResultsModel item)
    {
        SourceCodeStringBuilder builder = new();
        builder.StartPartialClassImplements(item, w =>
        {
            w.Write(": ");
            StrCat cats = new();
            foreach (var item in item.Properties)
            {
                cats.AddToString($"global::AdoNetHelpersLibrary.ConnectionHelpers.ICommandExecuteScalar{item.GenericObjectName}", ",");
            }
            string information = cats.GetInfo();
            w.Write(information);
        }, w =>
        {
            PopulateScalarDetails(w, item);
        });
        context.AddSource($"{item.ClassName}.ScalarObject.g.cs", builder.ToString()); //change sample to what you want.
    }
    private void WriteQueryItem(ResultsModel item)
    {
        SourceCodeStringBuilder builder = new();
        builder.StartPartialClassImplements(item, w =>
        {
            w.Write(": ");
            StrCat cats = new();
            foreach (var item in item.Properties)
            {
                cats.AddToString($"global::AdoNetHelpersLibrary.ConnectionHelpers.ICommandQuery{item.GenericObjectName}", ",");
            }
            string information = cats.GetInfo();
            w.Write(information);
        }, w =>
        {
            PopulateQueryDetails(w, item);
        });
        context.AddSource($"{item.ClassName}.QueryObject.g.cs", builder.ToString()); //change sample to what you want.
    }
    private void PopulateScalarDetails(ICodeBlock w, ResultsModel result)
    {
        foreach (var item in result.Properties)
        {
            w.PopulateScalarDetails(item);
        }
    }
    private void PopulateQueryDetails(ICodeBlock w, ResultsModel result)
    {
        foreach (var item in result.Properties)
        {
            w.PopulateQueryDetails(item);
        }
    }
}