namespace AdoNetHelpersGenerators.SingleTableQueries;
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
        output.Properties = GetProperties(symbol);
        return output;
    }
    private BasicList<PropertyModel> GetProperties(INamedTypeSymbol classSymbol)
    {
        BasicList<PropertyModel> output = [];
        var firsts = classSymbol.GetAllPublicProperties();
        foreach (var item in firsts)
        {
            if (item.HasAttribute("NotMapped") == false)
            {
                PropertyModel property = GetProperty(item);
                if (property.VariableCustomCategory != EnumSimpleTypeCategory.None && item.IsReadOnly == false)
                {
                    output.Add(property);
                }
            }
        }
        return output;
    }
    private PropertyModel GetProperty(IPropertySymbol symbol)
    {
        PropertyModel output = symbol.GetStartingPropertyInformation<PropertyModel>();
        bool rets = symbol.TryGetAttribute("Column", out var attributes);
        if (rets == false)
        {
            output.ColumnName = output.PropertyName;
        }
        else
        {
            output.ColumnName = attributes.Single().ConstructorArguments.Single().Value!.ToString();
        }
        return output;
    }
}