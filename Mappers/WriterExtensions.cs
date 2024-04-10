namespace AdoNetHelpersGenerators.Mappers;
internal static class WriterExtensions
{
    public static IWriter PopulateITableMapperStart(this IWriter w, string className)
    {
        w.Write("AdoNetHelpersLibrary.MapHelpers.ITableMapper<")
            .Write(className)
            .Write(">");
        return w;
    }
}