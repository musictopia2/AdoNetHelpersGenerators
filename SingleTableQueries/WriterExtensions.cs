namespace AdoNetHelpersGenerators.SingleTableQueries;
internal static class WriterExtensions
{
    public static IWriter PopulateListOfModel(this IWriter w, ResultsModel result)
    {
        w.BasicListWrite()
            .Write("<")
            .Write(result.ClassName)
            .Write(">");
        return w;
    }
    public static IWriter PopulateInterface(this IWriter w, ResultsModel result)
    {
        w.Write(": global::CommonBasicLibraries.DatabaseHelpers.SourceGeneratorHelpers.ICommandQuery<")
            .Write(result.ClassName)
            .Write(">");
        return w;
    }
}