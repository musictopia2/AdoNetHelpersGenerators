namespace AdoNetHelpersGenerators.SingleTableQueries;
internal static class CodeBlockExtensions
{
    public static ICodeBlock PopulateQuery(this ICodeBlock w, ResultsModel result)
    {
        w.WriteLine(w =>
        {
            w.Write("static ")
            .PopulateListOfModel(result)
            .Write(" global::AdoNetHelpersLibrary.ConnectionHelpers.ICommandQuery<")
            .Write(result.ClassName)
            .Write(">.Query(global::System.Data.IDbCommand command, CommonBasicLibraries.DatabaseHelpers.MiscClasses.EnumDatabaseCategory category)");
        })
        .WriteCodeBlock(w =>
        {
            w.StartQuery(result)
            .WriteLine("while (reader.Read())")
            .WriteCodeBlock(w =>
            {
                w.ReadQuery(result);
            })
            .EndCode();
        });
        return w;
    }
    public static ICodeBlock PopulateQueryAsync(this ICodeBlock w, ResultsModel result)
    {
        w.WriteLine(w =>
        {
            w.Write("static async Task<")
            .PopulateListOfModel(result)
            .Write("> global::AdoNetHelpersLibrary.ConnectionHelpers.ICommandQuery<")
            .Write(result.ClassName)
            .Write(">.QueryAsync(global::System.Data.IDbCommand command, CommonBasicLibraries.DatabaseHelpers.MiscClasses.EnumDatabaseCategory category)");
        })
       .WriteCodeBlock(w =>
       {
           w.StartQuery(result)
           .WriteLine("while (await reader.ReadAsync())")
           .WriteCodeBlock(w =>
           {
               w.ReadQuery(result);
           })
           .EndCode();
       });
        return w;
    }
    public static ICodeBlock PopulateReadItem(this ICodeBlock w, ResultsModel result)
    {
        w.WriteLine(w =>
        {
            w.Write("private static ")
            .Write(result.ClassName)
            .Write(" ReadItem(global::System.Data.Common.DbDataReader reader, CommonBasicLibraries.DatabaseHelpers.MiscClasses.EnumDatabaseCategory category)");
        })
        .WriteCodeBlock(w =>
        {
            w.WriteLine(w =>
            {
                w.Write(result.ClassName)
                .Write(" output = new();");
            })
            .WriteLine("var list = global::System.Data.Common.DbDataReaderExtensions.GetColumnSchema(reader);")
            .WriteLine("foreach (var item in list)")
            .WriteCodeBlock(w =>
            {
                foreach (var item in result.Properties)
                {
                    w.WritePropertyBlock(item);
                }
            })
            .EndCode();
        });
        return w;
    }
    private static ICodeBlock StartQuery(this ICodeBlock w, ResultsModel result)
    {
        w.WriteLine(w =>
        {
            w.Write("using global::System.Data.Common.DbDataReader? reader = command.ExecuteReader() as global::System.Data.Common.DbDataReader ?? throw new global::CommonBasicLibraries.BasicDataSettingsAndProcesses.CustomBasicException(")
            .AppendDoubleQuote("No reader found")
            .Write(");");
        })
        .WriteLine(w =>
        {
            w.PopulateListOfModel(result)
            .Write(" output = [];");
        });
        return w;
    }
    private static ICodeBlock ReadQuery(this ICodeBlock w, ResultsModel result)
    {
        w.WriteLine(w =>
        {
            w.Write(result.ClassName)
            .Write(" item = ReadItem(reader, category);");
        })
        .WriteLine("output.Add(item);");
        return w;
    }
    private static ICodeBlock EndCode(this ICodeBlock w)
    {
        w.WriteLine("return output;");
        return w;
    }
    private static ICodeBlock WritePropertyBlock(this ICodeBlock w, PropertyModel property)
    {
        if (property.VariableCustomCategory == EnumSimpleTypeCategory.Int && property.Nullable == false)
        {
            if (property.PropertyName == "ID" || property.ColumnName == "ID")
            {
                w.WriteLine(w =>
                {
                    w.Write("if (item.ColumnName == ")
                    .AppendDoubleQuote("Id")
                    .Write(")");
                })
                .WriteCodeBlock(w =>
                {
                    w.WriteLine(w =>
                    {
                        w.Write("output.")
                        .Write(property.PropertyName)
                        .Write("= global::System.Data.DataReaderExtensions.GetInt32(reader, ")
                        .AppendDoubleQuote("Id")
                        .Write(");");
                    });
                });
                w.WriteLine(w =>
                {
                    w.Write("if (item.ColumnName == ")
                    .AppendDoubleQuote("ID")
                    .Write(")");
                })
                .WriteCodeBlock(w =>
                {
                    w.WriteLine(w =>
                    {
                        w.Write("output.")
                        .Write(property.PropertyName)
                        .Write("= global::System.Data.DataReaderExtensions.GetInt32(reader, ")
                        .AppendDoubleQuote("ID")
                        .Write(");");
                    });
                });
            }
            return w;
        }
        w.WriteLine(w =>
        {
            w.Write("if (item.ColumnName == ")
            .AppendDoubleQuote(property.ColumnName)
            .Write(")");
        })
        .WriteCodeBlock(w =>
        {
            if (property.Nullable || property.VariableCustomCategory == EnumSimpleTypeCategory.CustomEnum)
            {
                w.WriteLine(w =>
                {
                    w.Write("if (global::System.Data.DataReaderExtensions.IsDBNull(reader, ")
                    .AppendDoubleQuote(property.ColumnName)
                    .Write(") == false)");
                })
                .WriteCodeBlock(w =>
                {
                    w.WriteInnerRead(property);
                });
            }
            else
            {
                w.WriteInnerRead(property);
            }
        });
        return w;
    }
    private static ICodeBlock WriteInnerRead(this ICodeBlock w, PropertyModel property)
    {
        if (property.VariableCustomCategory == EnumSimpleTypeCategory.StandardEnum || property.VariableCustomCategory == EnumSimpleTypeCategory.CustomEnum)
        {
            w.WriteLine(w =>
            {
                w.Write("int temp = global::System.Data.DataReaderExtensions.GetInt32(reader, ")
                .AppendDoubleQuote(property.ColumnName)
                .Write(");");
            });
            if (property.VariableCustomCategory == EnumSimpleTypeCategory.StandardEnum)
            {
                w.WriteLine(w =>
                {
                    w.Write("output.")
                    .Write(property.PropertyName)
                    .Write(" = (")
                    .GlobalWrite()
                    .Write(property.ContainingNameSpace)
                    .Write(".")
                    .Write(property.UnderlyingSymbolName)
                    .Write(") temp;");
                });
            }
            else if (property.VariableCustomCategory == EnumSimpleTypeCategory.CustomEnum)
            {
                w.WriteLine(w =>
                {
                    w.Write("output.")
                    .Write(property.PropertyName)
                    .Write(" = ")
                    .GlobalWrite()
                    .Write(property.ContainingNameSpace)
                    .Write(".")
                    .Write(property.UnderlyingSymbolName)
                    .Write(".FromValue(temp);");
                });
            }
            return w;
        }
        if (property.VariableCustomCategory == EnumSimpleTypeCategory.DateOnly)
        {
            w.WriteLine("if (category == CommonBasicLibraries.DatabaseHelpers.MiscClasses.EnumDatabaseCategory.SQLite)")
            .WriteCodeBlock(w =>
            {
                w.WriteLine($"""
                    string dateUsed = System.Data.DataReaderExtensions.GetString(reader, "{property.ColumnName}");
                    """)
                .WriteLine($"output.{property.PropertyName} = DateOnly.Parse(dateUsed);");
            })
            .WriteLine("else")
            .WriteCodeBlock(w =>
            {
                w.WriteLine(w =>
                {
                    w.Write("DateTime dateUsed = global::System.Data.DataReaderExtensions.GetDateTime(reader, ")
                    .AppendDoubleQuote(property.ColumnName)
                    .Write(");");
                }).WriteLine(w =>
                {
                    w.Write("output.")
                    .Write(property.PropertyName)
                    .Write(" = new(dateUsed.Year, dateUsed.Month, dateUsed.Day);");
                });
            });
            return w;
        }
        if (property.VariableCustomCategory == EnumSimpleTypeCategory.DateTime)
        {
            w.WriteLine("if (category == CommonBasicLibraries.DatabaseHelpers.MiscClasses.EnumDatabaseCategory.SQLite)")
            .WriteCodeBlock(w =>
            {
                w.WriteLine($"""
                    string dateUsed = System.Data.DataReaderExtensions.GetString(reader, "{property.ColumnName}");
                    """)
                .WriteLine($"output.{property.PropertyName} = DateTime.Parse(dateUsed);");
            })
            .WriteLine("else")
            .WriteCodeBlock(w =>
            {
                w.WriteLine($"""
                    output.{property.PropertyName} = System.Data.DataReaderExtensions.GetDateTime(reader, "{property.ColumnName}");
                    """);
            });
            return w;
        }
        if (property.VariableCustomCategory == EnumSimpleTypeCategory.TimeOnly)
        {
            w.WriteLine("if (category == CommonBasicLibraries.DatabaseHelpers.MiscClasses.EnumDatabaseCategory.SQLite)")
            .WriteCodeBlock(w =>
            {
                w.WriteLine($"""
                    string timeUsed = System.Data.DataReaderExtensions.GetString(reader, "{property.ColumnName}");
                    """)
                .WriteLine($"output.{property.PropertyName} = TimeOnly.Parse(timeUsed);");
            })
            .WriteLine("else")
            .WriteCodeBlock(w =>
            {
                w.WriteLine(w =>
                {
                    w.Write("DateTime timeUsed = global::System.Data.DataReaderExtensions.GetDateTime(reader, ")
                    .AppendDoubleQuote(property.ColumnName)
                    .Write(");");
                }).WriteLine(w =>
                {
                    w.Write("output.")
                    .Write(property.PropertyName)
                    .Write(" = new(timeUsed.Hour, timeUsed.Minute, timeUsed.Second, timeUsed.Millisecond);");
                });
            });
            return w;
        }
        if (property.VariableCustomCategory == EnumSimpleTypeCategory.Char)
        {
            w.WriteLine(w =>
            {
                w.Write("string temp = global::System.Data.DataReaderExtensions.GetString(reader, ")
                .AppendDoubleQuote(property.ColumnName)
                .Write(");");
            })
            .WriteLine(w =>
            {
                w.Write("output.")
                .Write(property.PropertyName)
                .Write(" = temp.SingleOrDefault();");
            });
            return w;
        }
        string method = property.VariableCustomCategory.GetMethodToUse();
        w.WriteLine(w =>
        {
            w.Write("output.")
            .Write(property.PropertyName)
            .Write(" = global::System.Data.DataReaderExtensions.")
            .Write(method)
            .Write("(reader, ")
            .AppendDoubleQuote(property.ColumnName)
            .Write(");");
        });
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