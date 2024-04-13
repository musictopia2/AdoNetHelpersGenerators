namespace AdoNetHelpersGenerators.JoinedTableQueries;
internal static class CodeBlockExtensions
{
    public static ICodeBlock PopulateQuery(this ICodeBlock w, ResultsModel result, JoinedGenericModel generic)
    {
        w.WriteLine($"static global::CommonBasicLibraries.CollectionClasses.BasicList<{result.FullName}> global::CommonBasicLibraries.DatabaseHelpers.SourceGeneratorHelpers.ICommandQuery{generic.FullInterfaceName}.Query(global::System.Data.IDbCommand command, {generic.FullFunctionName} action, global::CommonBasicLibraries.DatabaseHelpers.MiscClasses.EnumDatabaseCategory category)")
            .WriteCodeBlock(w =>
            {
                w.FinishQuery(result, false);
            });
        return w;
    }
    public static ICodeBlock PopulateQueryAsync(this ICodeBlock w, ResultsModel result, JoinedGenericModel generic)
    {
        w.WriteLine($"static async Task<global::CommonBasicLibraries.CollectionClasses.BasicList<{result.FullName}>> global::CommonBasicLibraries.DatabaseHelpers.SourceGeneratorHelpers.ICommandQuery{generic.FullInterfaceName}.QueryAsync(global::System.Data.IDbCommand command, {generic.FullFunctionName} action, global::CommonBasicLibraries.DatabaseHelpers.MiscClasses.EnumDatabaseCategory category)")
            .WriteCodeBlock(w =>
            {
                w.FinishQuery(result, true);
            });
        return w;
    }
    private static ICodeBlock FinishQuery(this ICodeBlock w, ResultsModel result, bool async)
    {
        w.WriteLine("""
            using System.Data.Common.DbDataReader? reader = command.ExecuteReader() as System.Data.Common.DbDataReader ?? throw new global::CommonBasicLibraries.BasicDataSettingsAndProcesses.CustomBasicException("No reader found");
            """)
            .WriteLine($"global::CommonBasicLibraries.CollectionClasses.BasicList<{result.FullName}> output = [];")
            .PopulateMiddle(async, w =>
            {
                w.WriteLine($"{result.FullName} item = ReadItem(reader, action, category);")
                .WriteLine("output.Add(item);");
            });
        w.WriteLine("return output;");
        return w;
    }
    private static ICodeBlock PopulateMiddle(this ICodeBlock w, bool async, Action<ICodeBlock> action)
    {
        if (async)
        {
            w.WriteLine("while (await reader.ReadAsync())").WriteCodeBlock(action);
        }
        else
        {
            w.WriteLine("while (reader.Read())").WriteCodeBlock(action);
        }
        return w;
    }
    public static ICodeBlock PopulateReadItem(this ICodeBlock w, ResultsModel result, JoinedGenericModel generic)
    {
        w.WriteLine($"private static {result.FullName} ReadItem(System.Data.Common.DbDataReader reader, {generic.FullFunctionName} action, global::CommonBasicLibraries.DatabaseHelpers.MiscClasses.EnumDatabaseCategory category)")
            .WriteCodeBlock(w =>
            {
                //w.WriteLine("bool hasForeignKey = true;");
                w.WriteLine($"{result.FullName} temp{result.ClassName} = new();");
                foreach (var item in generic.Tables)
                {
                    w.WriteLine($"{item.FullName}? {item.ObjectVariableName} = new();")
                    .WriteLine($"bool {item.BoolVariableValue} = true;");
                }

                w.WriteLine("var list = System.Data.Common.DbDataReaderExtensions.GetColumnSchema(reader);")
                .WriteLine("int instances = 0;")
                .WriteLine("int index = 0;")
                .WriteLine("foreach (var item in list)")
                .WriteCodeBlock(w =>
                {
                    w.WriteLine("""
                        if (item.ColumnName == "ID")
                        """)
                    .WriteCodeBlock(w =>
                    {
                        w.WriteLine("instances++;")
                        .WriteLine("int? results;")
                        .WriteLine("if (reader.IsDBNull(index) == false)")
                        .WriteCodeBlock(w =>
                        {
                            w.WriteLine("results = reader.GetInt32(index);");
                        })
                        .WriteLine("else")
                        .WriteCodeBlock(w =>
                        {
                            w.WriteLine("results = null;");
                        });
                        foreach (var item in generic.Tables)
                        {
                            w.WriteLine($"if (instances == {item.Instances} && results is null)")
                            .WriteCodeBlock(w =>
                            {
                                w.WriteLine($"{item.BoolVariableValue} = false;");
                            });
                        }
                        var property = result.Properties.Single(x => x.IsIDField);
                        w.WriteLine("if (instances == 1)")
                        .WriteCodeBlock(w =>
                        {
                            w.WriteLine($"temp{result.ClassName}.{property.PropertyName} = results!.Value;");
                        });
                        foreach (var item in generic.Tables)
                        {
                            property = item.Properties.Single(x => x.IsIDField);
                            w.WriteLine($"else if (results is not null  && instances == {item.Instances})")
                            .WriteCodeBlock(w =>
                            {
                                w.WriteLine($"{item.ObjectVariableName}.{property.PropertyName} = results.Value;");
                            });
                        }
                        w.WriteLine("index++;")
                        .WriteLine("continue;");
                    });

                    w.WriteLine("if (reader.IsDBNull(index) == false)")
                    .WriteCodeBlock(w =>
                    {
                        foreach (var item in result.Properties)
                        {
                            if (item.IsIDField == false)
                            {
                                w.WriteLine($"""
                                    if (item.ColumnName == "{item.PropertyName}")
                                    """)
                               .WriteCodeBlock(w =>
                               {
                                   w.PopulatePropertyInformation($"temp{result.ClassName}", item);
                               });
                            }
                        }
                        foreach (var item in generic.Tables)
                        {
                            foreach (var p in item.Properties)
                            {
                                if (p.ForeignTableName == "" && p.IsIDField == false)
                                {
                                    w.WriteLine($"""
                                            if (item.ColumnName == "{p.PropertyName}")
                                            """)
                                   .WriteCodeBlock(w =>
                                   {
                                       w.PopulatePropertyInformation(item.ObjectVariableName, p);
                                   });
                                }
                            }
                        }
                    })
                    .WriteLine("index++;");
                });
                var main = result.Properties.Single(x => x.IsIDField);
                foreach (var item in generic.Tables)
                {
                    var p = item.Properties.FirstOrDefault(x => x.ForeignTableName.ToLower() == result.TableName.ToLower());
                    w.WriteLine($"if ({item.BoolVariableValue} == false)")
                    .WriteCodeBlock(w =>
                    {
                        w.WriteLine($"{item.ObjectVariableName} = null;");
                    });

                    if (p is not null)
                    {
                        w.WriteLine("else")
                        .WriteCodeBlock(w =>
                        {
                            w.WriteLine($"{item.ObjectVariableName}.{p.PropertyName} = temp{result.ClassName}.{main.PropertyName};");
                        });
                    }
                }
                w.WriteLine(w =>
                {
                    w.Write(result.FullName)
                    .Write(" main = action(temp")
                    .Write(result.ClassName)
                    .Write(",");
                    StrCat cats = new();
                    foreach (var item in generic.Tables)
                    {
                        cats.AddToString(item.ObjectVariableName, ", ");
                    }
                    w.Write(cats.GetInfo())
                    .Write(");");
                });
                w.WriteLine("return main;");
            });
        return w;
    }
    private static ICodeBlock PopulatePropertyInformation(this ICodeBlock w, string variableName, PropertyModel property)
    {
        if (property.VariableCustomCategory == EnumSimpleTypeCategory.CustomEnum)
        {
            w.WriteLine("int temp = reader.GetInt32(index);");
            w.WriteLine($"{variableName}.{property.PropertyName} = {property.ContainingNameSpace}.{property.UnderlyingSymbolName}.FromValue(temp);");
            return w;
        }
        if (property.VariableCustomCategory == EnumSimpleTypeCategory.StandardEnum)
        {
            w.WriteLine("int temp = reader.GetInt32(index);");
            w.WriteLine($"{variableName}.{property.PropertyName} = ({property.ContainingNameSpace}.{property.UnderlyingSymbolName}) temp;");
            return w;
        }
        if (property.VariableCustomCategory == EnumSimpleTypeCategory.DateOnly)
        {
            w.WriteLine("if (category == CommonBasicLibraries.DatabaseHelpers.MiscClasses.EnumDatabaseCategory.SQLite)")
                .WriteCodeBlock(w =>
                {
                    w.WriteLine("string dateUsed = reader.GetString(index);")
                    .WriteLine($"{variableName}.{property.PropertyName}  = DateOnly.Parse(dateUsed);");
                })
           .WriteLine("else")
               .WriteCodeBlock(w =>
               {
                   w.WriteLine("string dateUsed = reader.GetDateTime(index);")
                   .WriteLine($"{variableName}.{property.PropertyName} = new(dateUsed.Year, dateUsed.Month, dateUsed.Day);");
               });
            return w;
        }
        if (property.VariableCustomCategory == EnumSimpleTypeCategory.TimeOnly)
        {
            w.WriteLine("if (category == CommonBasicLibraries.DatabaseHelpers.MiscClasses.EnumDatabaseCategory.SQLite)")
                .WriteCodeBlock(w =>
                {
                    w.WriteLine("string timeUsed = reader.GetString(index);")
                    .WriteLine($"{variableName}.{property.PropertyName}  = TimeOnly.Parse(timeUsed);");
                })
           .WriteLine("else")
               .WriteCodeBlock(w =>
               {
                   w.WriteLine("string timeUsed = reader.GetDateTime(index);")
                   .WriteLine($"{variableName}.{property.PropertyName} = new(timeUsed.Hour, timeUsed.Minute, timeUsed.Second, timeUsed.Millisecond);");
               });
            return w;
        }
        if (property.VariableCustomCategory == EnumSimpleTypeCategory.DateTime)
        {
            w.WriteLine("if (category == CommonBasicLibraries.DatabaseHelpers.MiscClasses.EnumDatabaseCategory.SQLite)")
                .WriteCodeBlock(w =>
                {
                    w.WriteLine("string dateUsed = reader.GetString(index);")
                    .WriteLine($"{variableName}.{property.PropertyName} = DateTime.Parse(dateUsed);");
                })
           .WriteLine("else")
               .WriteCodeBlock(w =>
               {
                   w.WriteLine($"{variableName}.{property.PropertyName} = reader.GetDateTime(index);");
               });
            return w;
        }
        if (property.VariableCustomCategory == EnumSimpleTypeCategory.Char)
        {
            w.WriteLine("string temp = reader.GetString(index);")
                .WriteLine($"{variableName}.{property.PropertyName} = temp.SingleOrDefault();");
            return w;
        }
        string method = property.VariableCustomCategory.GetMethodToUse();
        w.WriteLine($"{variableName}.{property.PropertyName} = reader.{method}(index);");
        return w;
    }
    private static string GetMethodToUse(this EnumSimpleTypeCategory category)
    {
        if (category == EnumSimpleTypeCategory.Bool)
        {
            return "GetBoolean";
        }
        if (category == EnumSimpleTypeCategory.String)
        {
            return "GetString";
        }
        if (category == EnumSimpleTypeCategory.Double)
        {
            return "GetDouble";
        }
        if (category == EnumSimpleTypeCategory.DateTime)
        {
            return "GetDateTime";
        }
        if (category == EnumSimpleTypeCategory.Decimal)
        {
            return "GetDecimal";
        }
        if (category == EnumSimpleTypeCategory.Float)
        {
            return "GetFloat";
        }
        if (category == EnumSimpleTypeCategory.Int)
        {
            return "GetInt32";
        }
        return "";
    }
}