namespace AdoNetHelpersGenerators.Objects;
internal static class CodeBlockExtensions
{
    public static ICodeBlock PopulateScalarDetails(this ICodeBlock w, PropertyModel p)
    {
        w.WriteLine($"static {p.ReturnType} global::CommonBasicLibraries.DatabaseHelpers.SourceGeneratorHelpers.ICommandExecuteScalar{p.GenericObjectName}.ExecuteScalar(System.Data.IDbCommand command)")
            .WriteCodeBlock(w =>
            {
                if (p.VariableCustomCategory == EnumSimpleTypeCategory.CustomEnum)
                {
                    w.PopulateCustomEnumScalar(p);
                }
                else if (p.VariableCustomCategory == EnumSimpleTypeCategory.StandardEnum)
                {
                    w.PopulateStandardEnumScalar(p);
                }
                else
                {
                    w.WriteLine($"return global::CommonBasicLibraries.DatabaseHelpers.Extensions.CommandScalarExtensions.{p.ScalarInfo}(command);");
                }
            });

        return w;
    }
    private static ICodeBlock PopulateStandardQueryFunctions(this ICodeBlock w, PropertyModel p, bool async)
    {
        string extras = "";
        if (async)
        {
            extras = "Async";
        }
        string starts = "";
        if (async)
        {
            starts = "await";
        }
        string ends = "";
        if (p.VariableCustomCategory == EnumSimpleTypeCategory.DateOnly || p.VariableCustomCategory == EnumSimpleTypeCategory.DateTime || p.VariableCustomCategory == EnumSimpleTypeCategory.TimeOnly)
        {
            ends = ", category";
        }
        w.WriteLine($"return {starts} global::CommonBasicLibraries.DatabaseHelpers.Extensions.CommandReaderExtensions.{p.QueryMethod}{extras}(command{ends});");
        return w;
    }
    public static ICodeBlock PopulateQueryDetails(this ICodeBlock w, PropertyModel p)
    {
        w.WriteLine($"static global::CommonBasicLibraries.CollectionClasses.BasicList<{p.ReturnType}> global::CommonBasicLibraries.DatabaseHelpers.SourceGeneratorHelpers.ICommandQuery{p.GenericObjectName}.Query(System.Data.IDbCommand command, CommonBasicLibraries.DatabaseHelpers.MiscClasses.EnumDatabaseCategory category)")
            .WriteCodeBlock(w =>
            {
                if (p.VariableCustomCategory == EnumSimpleTypeCategory.CustomEnum || p.VariableCustomCategory == EnumSimpleTypeCategory.StandardEnum)
                {
                    w.PopulateAnyEnumQuery(p);
                }
                else
                {
                    w.PopulateStandardQueryFunctions(p, false);
                }
            });
        w.WriteLine($"static async Task<global::CommonBasicLibraries.CollectionClasses.BasicList<{p.ReturnType}>> global::CommonBasicLibraries.DatabaseHelpers.SourceGeneratorHelpers.ICommandQuery{p.GenericObjectName}.QueryAsync(System.Data.IDbCommand command, CommonBasicLibraries.DatabaseHelpers.MiscClasses.EnumDatabaseCategory category)")
            .WriteCodeBlock(w =>
            {
                if (p.VariableCustomCategory == EnumSimpleTypeCategory.CustomEnum || p.VariableCustomCategory == EnumSimpleTypeCategory.StandardEnum)
                {
                    w.PopulateAnyEnumQueryAsync(p);
                }
                else
                {
                    w.PopulateStandardQueryFunctions(p, true);
                }
            });
        if (p.VariableCustomCategory == EnumSimpleTypeCategory.CustomEnum)
        {
            w.PopulateCustomEnumReading(p);
        }
        if (p.VariableCustomCategory == EnumSimpleTypeCategory.StandardEnum)
        {
            if (p.Nullable)
            {
                w.PopulateStandardNullableEnumReading(p);
            }
            else
            {
                w.PopulateStandardEnumReading(p);
            }
        }
        return w;
    }
    private static ICodeBlock PopulateCustomEnumScalar(this ICodeBlock w, PropertyModel p)
    {
        w.WriteLine("object? results = command.ExecuteScalar();")
            .WriteLine("if (results is null)")
            .WriteCodeBlock(w =>
            {
                w.WriteLine($"""
                    {p.ReturnType} option = {p.ReturnType}.FromName("");
                    """)
                .WriteLine("return option;");
            })
            .WriteLine("string input = results.ToString()!;")
            .WriteLine("if (string.IsNullOrEmpty(input))")
            .WriteCodeBlock(w =>
            {
                w.WriteLine($"""
                    {p.ReturnType} option = {p.ReturnType}.FromName("");
                    """)
                .WriteLine("return option;");
            });
        w.WriteLine("int temp = int.Parse(results.ToString()!);")
        .WriteLine($"return {p.ReturnType}.FromValue(temp);");
        return w;
    }
    private static ICodeBlock PopulateStandardEnumScalar(this ICodeBlock w, PropertyModel p)
    {
        w.WriteLine("object? results = command.ExecuteScalar();")
            .WriteLine("if (results is null)")
            .WriteCodeBlock(w =>
            {
                if (p.Nullable)
                {
                    w.WriteLine("return null;");
                }
                else
                {
                    w.WriteLine("return default;");
                }
            })
            .WriteLine("int temp = int.Parse(results.ToString()!);")
            .WriteLine($"return ({p.ReturnType}) temp;");
        return w;
    }
    private static ICodeBlock PopulateCustomEnumReading(this ICodeBlock w, PropertyModel p)
    {
        w.WriteLine($"private static {p.ReturnType} {p.QueryMethod}(System.Data.Common.DbDataReader reader)")
            .WriteCodeBlock(w =>
            {
                w.WriteLine($"{p.ReturnType} output = default;")
                .WriteLine("var list = System.Data.Common.DbDataReaderExtensions.GetColumnSchema(reader);")
                .WriteLine("if (list.Count == 0)")
                .WriteCodeBlock(w =>
                {
                    w.WriteLine("return output;");
                })
                .WriteLine("if (list.Count > 1)")
                .WriteCodeBlock(w =>
                {
                    w.WriteLine(w => w.CustomExceptionLine(w =>
                    {
                        w.Write("Cannot have more than one item");
                    }));
                })
                .WriteLine("if (System.Data.DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)")
                .WriteCodeBlock(w =>
                {
                    w.WriteLine("int chosen = System.Data.DataReaderExtensions.GetInt32(reader, list.Single().ColumnName);")
                    .WriteLine($"output = {p.ReturnType}.FromValue(chosen);"); //try that (?)
                })
                .WriteLine("return output;");
            });
        return w;
    }
    private static ICodeBlock PopulateStandardEnumReading(this ICodeBlock w, PropertyModel p)
    {
        w.WriteLine($"private static {p.ReturnType} {p.QueryMethod}(System.Data.Common.DbDataReader reader)")
            .WriteCodeBlock(w =>
            {
                w.FinishStandardEnumReading(p, false);
            });
        return w;
    }
    private static ICodeBlock PopulateStandardNullableEnumReading(this ICodeBlock w, PropertyModel p)
    {
        w.WriteLine($"private static {p.ReturnType} {p.QueryMethod}(System.Data.Common.DbDataReader reader)")
            .WriteCodeBlock(w =>
            {
                w.FinishStandardEnumReading(p, true);
            });
        return w;
    }
    private static ICodeBlock FinishStandardEnumReading(this ICodeBlock w, PropertyModel p, bool nullable)
    {
        if (nullable)
        {
            w.WriteLine("int? output = default;");
        }
        else
        {
            w.WriteLine("int output = default;");
        }
        w.WriteLine("var list = System.Data.Common.DbDataReaderExtensions.GetColumnSchema(reader);")
        .WriteLine("if (list.Count == 0)")
        .WriteCodeBlock(w =>
        {
            if (nullable)
            {
                w.WriteLine("return null;");
            }
            else
            {
                w.WriteLine("return default;");
            }
        })
        .WriteLine("if (list.Count > 1)")
        .WriteCodeBlock(w =>
        {
            w.WriteLine(w => w.CustomExceptionLine(w =>
            {
                w.Write("Cannot have more than one item");
            }));
        })
        .WriteLine("if (System.Data.DataReaderExtensions.IsDBNull(reader, list.Single().ColumnName) == false)")
        .WriteCodeBlock(w =>
        {
            w.WriteLine("output = System.Data.DataReaderExtensions.GetInt32(reader, list.Single().ColumnName);");
            w.WriteLine($"return ({p.ReturnType}) output;");
        });
        if (nullable)
        {
            w.WriteLine("return null;");
        }
        else
        {
            w.WriteLine("return default;");
        }
        return w;
    }
    private static ICodeBlock PopulateAnyEnumQuery(this ICodeBlock w, PropertyModel p)
    {
        w.StartEnumQuery(p);
        w.WriteLine("while (reader.Read())")
            .FinishEnumQuery(p);
        return w;
    }
    private static ICodeBlock StartEnumQuery(this ICodeBlock w, PropertyModel p)
    {
        w.WriteLine("""
            using System.Data.Common.DbDataReader? reader = command.ExecuteReader() as System.Data.Common.DbDataReader ?? throw new CustomBasicException("No reader found");
            """)
            .WriteLine($"global::CommonBasicLibraries.CollectionClasses.BasicList<{p.ReturnType}> output = [];");
        return w;
    }
    private static ICodeBlock FinishEnumQuery(this ICodeBlock w, PropertyModel p)
    {
        w.WriteCodeBlock(w =>
        {
            w.WriteLine($"output.Add({p.QueryMethod}(reader));");
        }).WriteLine("return output;");
        return w;
    }
    private static ICodeBlock PopulateAnyEnumQueryAsync(this ICodeBlock w, PropertyModel p)
    {
        w.StartEnumQuery(p);
        w.WriteLine("while (await reader.ReadAsync())")
            .FinishEnumQuery(p);
        return w;
    }
}