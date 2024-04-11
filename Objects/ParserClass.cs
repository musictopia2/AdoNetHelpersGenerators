namespace AdoNetHelpersGenerators.Objects;
internal class ParserClass(IEnumerable<ClassDeclarationSyntax> list, Compilation compilation)
{
    public BasicList<ResultsModel> GetResults()
    {
        BasicList<ResultsModel> output = [];
        foreach (var item in list)
        {
            ResultsModel results = GetResult(item);
            if (output.Any(x => x.ClassName == results.ClassName) == false)
            {
                output.Add(results);
            }
        }
        return output;
    }
    private ResultsModel GetResult(ClassDeclarationSyntax classDeclaration)
    {
        ResultsModel output;
        INamedTypeSymbol symbol = compilation.GetClassSymbol(classDeclaration)!;
        output = symbol.GetStartingResults<ResultsModel>();
        output.Properties = GetProperties(symbol, output);
        return output;
    }
    private HashSet<PropertyModel> GetProperties(INamedTypeSymbol classSymbol, ResultsModel result)
    {
        HashSet<PropertyModel> output = [];
        var firsts = classSymbol.GetAllPublicProperties();
        foreach (var item in firsts)
        {
            if (item.HasAttribute("NotMapped") == false &&  item.IsReadOnly == false)
            {
                PropertyModel property = GetProperty(item, result);
                if (property.VariableCustomCategory != EnumSimpleTypeCategory.None)
                {
                    output.Add(property);
                }
            }
        }
        return output;
    }
    private PropertyModel GetProperty(IPropertySymbol symbol, ResultsModel result)
    {
        PropertyModel output = symbol.GetStartingPropertyInformation<PropertyModel>();
        output.PropertyName = "";
        output.ScalarInfo = GetScalarMethod(output.VariableCustomCategory, output.Nullable);
        output.QueryMethod = GetQueryMethod(output.VariableCustomCategory, output.UnderlyingSymbolName, output.Nullable);
        output.ReturnType = GetReturnMethod(output.VariableCustomCategory, output.UnderlyingSymbolName, output.ContainingNameSpace, output.Nullable);
        output.GenericObjectName = GetGenericMethod(output, result);
        return output;
    }
    private string GetReturnMethod(EnumSimpleTypeCategory category, string underlyingName, string @namespace, bool nullable)
    {
        string symbolName;
        if (category == EnumSimpleTypeCategory.CustomEnum || category == EnumSimpleTypeCategory.StandardEnum)
        {
            symbolName = $"{@namespace}.{underlyingName}";
        }
        else if (category == EnumSimpleTypeCategory.DateOnly || category == EnumSimpleTypeCategory.DateTime || category == EnumSimpleTypeCategory.TimeOnly)
        {
            symbolName = category.ToString();
        }
        else
        {
            symbolName = category.ToString().ToLower();
        }
        if (category == EnumSimpleTypeCategory.String)
        {
            symbolName = "string?";
        }
        string output;
        if (nullable)
        {
            output = $"{symbolName}?";
        }
        else
        {
            output = symbolName;
        }
        return output;
    }
    private string GetGenericMethod(PropertyModel p, ResultsModel r)
    {
        if (p.VariableCustomCategory == EnumSimpleTypeCategory.None)
        {
            return "";
        }
        if (p.VariableCustomCategory == EnumSimpleTypeCategory.CustomEnum || p.VariableCustomCategory == EnumSimpleTypeCategory.StandardEnum && p.Nullable == false)
        {
            return $"<{r.ClassName}, {p.ContainingNameSpace}.{p.UnderlyingSymbolName}>";
        }
        if (p.VariableCustomCategory == EnumSimpleTypeCategory.StandardEnum)
        {
            return $"<{r.ClassName}, {p.ContainingNameSpace}.{p.UnderlyingSymbolName}?>";
        }
        return $"<{r.ClassName}, {p.ReturnType}>";
    }
    private string GetScalarMethod(EnumSimpleTypeCategory category, bool nullable)
    {
        if (category == EnumSimpleTypeCategory.StandardEnum || category == EnumSimpleTypeCategory.None || category == EnumSimpleTypeCategory.CustomEnum)
        {
            return "";
        }
        if (nullable)
        {
            return $"ParseNullable{category}";
        }
        if (category == EnumSimpleTypeCategory.String)
        {
            return "ParseString";
        }
        if (category == EnumSimpleTypeCategory.DateTime || category == EnumSimpleTypeCategory.DateOnly || category == EnumSimpleTypeCategory.TimeOnly)
        {
            return $"Parse<{category}>";
        }
        return $"Parse<{category.ToString().ToLower()}>";
    }
    private string GetQueryMethod(EnumSimpleTypeCategory category, string name, bool nullable)
    {
        if (category == EnumSimpleTypeCategory.None)
        {
            return "";
        }
        if (category == EnumSimpleTypeCategory.CustomEnum || category == EnumSimpleTypeCategory.StandardEnum)
        {
            string other = name.Replace("Enum", "");
            if (nullable == false)
            {
                return $"Read{other}";
            }
            return $"ReadNullable{other}";
        }
        if (nullable == false)
        {
            return $"Get{category}List";
        }
        return $"GetNullable{category}List";
    }
}