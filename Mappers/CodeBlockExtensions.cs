namespace AdoNetHelpersGenerators.Mappers;
internal static class CodeBlockExtensions
{
    public static ICodeBlock PopulateSimpleMethods(this ICodeBlock w, ResultsModel result)
    {
        w.WriteLine(w =>
        {
            string implemented = result.AutoIncremented ? "true" : "false";
            w.Write("static bool ")
            .PopulateITableMapperStart(result.ClassName)
            .Write(".IsAutoIncremented => ")
            .Write(implemented).Write(";");
        })
        .WriteLine(w =>
        {
            w.Write("static string ")
            .PopulateITableMapperStart(result.ClassName)
            .Write(".TableName => ")
            .AppendDoubleQuote(result.TableName).Write(";");
        })
        .WriteLine(w =>
        {
            w.Write("static CommonBasicLibraries.DatabaseHelpers.SourceGeneratorHelpers.SourceGeneratedMap ")
            .PopulateITableMapperStart(result.ClassName)
            .Write(".GetTableMap(bool beingJoined)");
        })
        .WriteCodeBlock(w =>
        {
            w.WriteLine("return PrivateGetMap(null, false, beingJoined);");
        })
        .WriteLine(w =>
        {
            w.Write("static CommonBasicLibraries.DatabaseHelpers.SourceGeneratorHelpers.SourceGeneratedMap ")
            .PopulateITableMapperStart(result.ClassName)
            .Write(".GetTableMap(")
            .Write(result.ClassName)
            .Write(" payLoad, bool isAutoIncremented, bool beingJoined)");
        })
        .WriteCodeBlock(w =>
        {
            w.WriteLine("return PrivateGetMap(payLoad, isAutoIncremented, beingJoined);");
        })
        .WriteLine(w =>
        {
            w.Write("static string ")
            .PopulateITableMapperStart(result.ClassName)
            .Write(".GetForeignKey(string name)");
        })
        .WriteCodeBlock(w =>
        {
            w.PopulateGettingForeignKey(result);
        })
        .WriteLine(w =>
        {
            w.Write("static bool ")
            .PopulateITableMapperStart(result.ClassName)
            .Write(".HasForeignKey(string name)");
        })
        .WriteCodeBlock(w =>
        {
            w.PopulateHasForeignKey(result);
        });
        return w;
    }
    private static ICodeBlock PopulateGettingForeignKey(this ICodeBlock w, ResultsModel result)
    {
        foreach (var p in result.Properties)
        {
            if (p.ForeignKey != "")
            {
                string lowerName = p.ForeignKey.ToLower();
                w.WriteLine($"""
                    if (name.ToLower() == "{lowerName}")
                    """)
                .WriteCodeBlock(w =>
                {
                    w.WriteLine($"""
                        return "{p.ColumnName}";
                        """);
                });
            }
        }
        w.WriteLine("""
            return "";
            """);
        return w;
    }
    private static ICodeBlock PopulateHasForeignKey(this ICodeBlock w, ResultsModel result)
    {
        foreach (var p in result.Properties)
        {
            if (p.ForeignKey != "")
            {
                w.WriteLine($"""
                    if (name.ToLower() == "{p.ForeignKey.ToLower()}")
                    """)
                .WriteCodeBlock(w =>
                {
                    w.WriteLine("return true;");
                });
            }
        }
        w.WriteLine("return false;");
        return w;
    }
    public static ICodeBlock PopulatePrivateMaps(this ICodeBlock w, ResultsModel result)
    {
        w.WriteLine($"private static CommonBasicLibraries.DatabaseHelpers.SourceGeneratorHelpers.SourceGeneratedMap PrivateGetMap({result.ClassName}? payLoad, bool isAutoIncremented, bool beingJoined)")
            .WriteCodeBlock(w =>
            {
                w.WriteLine("CommonBasicLibraries.DatabaseHelpers.SourceGeneratorHelpers.SourceGeneratedMap output = new();")
                .WriteLine($"""
                    output.TableName = "{result.TableName}";
                    """)
                .WriteLine("global::CommonBasicLibraries.CollectionClasses.BasicList<CommonBasicLibraries.DatabaseHelpers.SourceGeneratorHelpers.ColumnModel> columns = [];")
                .WriteLine("output.Columns = columns;")
                .WriteLine("CommonBasicLibraries.DatabaseHelpers.SourceGeneratorHelpers.ColumnModel column;");
                var idField = result.Properties.Single(x => x.IsIDField);
                w.WriteLine("if (isAutoIncremented == false)")
                .WriteCodeBlock(w =>
                {
                    w.PopulatePropertyInformation(idField);
                });
                foreach (var p in result.Properties)
                {
                    if (p.IsIDField == true)
                    {
                        continue;
                    }
                    if (p.UseForPossibleJoin)
                    {
                        w.PopulatePropertyInformation(p);
                    }
                    else
                    {
                        w.WriteLine("if (beingJoined == false)")
                        .WriteCodeBlock(w => w.PopulatePropertyInformation(p));
                    }
                }
                w.WriteLine("return output;");
            });
        return w;
    }
    private static ICodeBlock PopulatePropertyInformation(this ICodeBlock w, PropertyModel property)
    {
        w.WriteLine("column = new();")
        .WriteLine("if (payLoad is not null)")
        .WriteCodeBlock(w =>
        {
            w.PopulateValue(property);
        })
        .WriteLine($"""
            column.ColumnName = "{property.ColumnName}";
            """)
        .WriteLine($"""
            column.ObjectName = "{property.PropertyName}";
            """)
        .WriteLine($"column.CommonForUpdating = {property.CommonForUpdating.ToString().ToLower()};")
        .WriteLine("column.TableName = output.TableName;")
        .WriteLine("column.HasMatch = column.ColumnName.Equals(column.ObjectName);")
        .PopulateDatabaseType(property)
        .WriteLine("output.Columns.Add(column);");
        return w;
    }
    private static ICodeBlock PopulateDatabaseType(this ICodeBlock w, PropertyModel property)
    {
        if (property.VariableCustomCategory == EnumSimpleTypeCategory.CustomEnum || property.VariableCustomCategory == EnumSimpleTypeCategory.StandardEnum || property.VariableCustomCategory == EnumSimpleTypeCategory.Int)
        {
            w.WriteLine("column.ColumnType = System.Data.DbType.Int32;");
        }
        else if (property.VariableCustomCategory == EnumSimpleTypeCategory.Double)
        {
            w.WriteLine("column.ColumnType = System.Data.DbType.Double;");
        }
        else if (property.VariableCustomCategory == EnumSimpleTypeCategory.Bool)
        {
            w.WriteLine("column.ColumnType = System.Data.DbType.Boolean;");
        }
        else if (property.VariableCustomCategory == EnumSimpleTypeCategory.Char || property.VariableCustomCategory == EnumSimpleTypeCategory.String)
        {
            w.WriteLine("column.ColumnType = System.Data.DbType.String;");
        }
        else if (property.VariableCustomCategory == EnumSimpleTypeCategory.DateOnly)
        {
            w.WriteLine("column.ColumnType = System.Data.DbType.Date;");
        }
        else if (property.VariableCustomCategory == EnumSimpleTypeCategory.DateTime)
        {
            w.WriteLine("column.ColumnType = System.Data.DbType.DateTime2;");
        }
        else if (property.VariableCustomCategory == EnumSimpleTypeCategory.TimeOnly)
        {
            w.WriteLine("column.ColumnType = System.Data.DbType.Time;");
        }
        else if (property.VariableCustomCategory == EnumSimpleTypeCategory.Decimal)
        {
            w.WriteLine("column.ColumnType = System.Data.DbType.Currency;");
        }
        return w;
    }
    private static ICodeBlock PopulateValue(this ICodeBlock w, PropertyModel property)
    {
        if (property.VariableCustomCategory == EnumSimpleTypeCategory.StandardEnum)
        {
            if (property.Nullable)
            {
                w.WriteLine($"column.Value = (int?)payLoad.{property.PropertyName};");
            }
            else
            {
                w.WriteLine($"column.Value = (int)payLoad.{property.PropertyName};");
            }
        }
        else if (property.VariableCustomCategory == EnumSimpleTypeCategory.CustomEnum)
        {
            w.WriteLine($"if (payLoad.{property.PropertyName}.IsNull)")
               .WriteCodeBlock(w =>
               {
                   w.WriteLine("column.Value = null;");
               })
               .WriteLine("else")
               .WriteCodeBlock(w =>
               {
                   w.WriteLine($"column.Value = payLoad.{property.PropertyName}.Value;");
               });
        }
        else
        {
            w.WriteLine($"column.Value = payLoad.{property.PropertyName};");
        }
        return w;
    }
}