namespace AdoNetHelpersGenerators.Mappers;
internal static class WriterExtensions
{
    public static IWriter PopulateITableMapperStart(this IWriter w, string className)
    {
        w.Write("CommonBasicLibraries.DatabaseHelpers.SourceGeneratorHelpers.ITableMapper<")
            .Write(className)
            .Write(">");
        return w;
    }
}