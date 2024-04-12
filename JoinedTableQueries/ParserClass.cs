namespace AdoNetHelpersGenerators.JoinedTableQueries;
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
        output.Joins = GetJoinedGenerics(symbol, output);
        output.Properties = GetProperties(symbol);
        bool rets = symbol.TryGetAttribute("Table", out var attributes);
        if (rets == false)
        {
            output.TableName = output.ClassName;
        }
        else
        {
            output.TableName = attributes.Single().ConstructorArguments.Single().Value!.ToString();
        }
        return output;
    }
    private BasicList<JoinedGenericModel> GetJoinedGenerics(INamedTypeSymbol symbol, ResultsModel result)
    {
        var firsts = GetJoinedTables(symbol);
        BasicList<JoinedGenericModel> output = [];
        JoinedGenericModel generic;
        foreach (var item in firsts)
        {
            generic = new();
            var nexts = item.TypeArguments;
            StrCat cats1 = new();
            StrCat cats2 = new();
            string realName = $"global::{result.Namespace}.{result.ClassName}";
            cats1.AddToString(realName, ", ");
            cats2.AddToString(realName, ", ");
            int instances = 1;
            foreach (var next in nexts)
            {
                INamedTypeSymbol ss = (INamedTypeSymbol)next;
                JoinedTableModel table = ss.GetStartingResults<JoinedTableModel>();
                instances++;
                table.Instances = instances.ToString();
                table.Properties = GetProperties(ss);
                generic.Tables.Add(table);
                cats1.AddToString(table.FullName, ", ");
                cats2.AddToString($"{table.FullName}?", ", ");
            }
            cats1.AddToString(realName, ", ");
            cats2.AddToString(realName, ", ");
            string middles = cats1.GetInfo();
            generic.FullInterfaceName = $"<{middles}>";
            middles = cats2.GetInfo();
            generic.FullFunctionName = $"Func<{middles}>";
            output.Add(generic);
        }
        return output;
    }
    private BasicList<PropertyModel> GetProperties(INamedTypeSymbol symbol)
    {
        BasicList<PropertyModel> output = [];
        var firsts = symbol.GetAllPublicProperties();
        foreach (var item in firsts)
        {
            if (item.HasAttribute("NotMapped") == false && item.IsReadOnly == false)
            {
                PropertyModel property = GetProperty(item);
                if (property.VariableCustomCategory != EnumSimpleTypeCategory.None)
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
        if (output.PropertyName.ToLower() == "id")
        {
            output.IsIDField = true;
        }
        bool rets = symbol.TryGetAttribute("Column", out var attributes);
        if (rets)
        {
            string value = attributes.Single().ConstructorArguments.Single().Value!.ToString();
            if (value.ToLower() == "id")
            {
                output.IsIDField = true;
            }
        }
        rets = symbol.TryGetAttribute("ForeignKey", out attributes);
        if (rets)
        {
            output.ForeignTableName = attributes.Single().ConstructorArguments.Single().Value!.ToString();
        }
        return output;
    }
    private BasicList<INamedTypeSymbol> GetJoinedTables(INamedTypeSymbol symbol)
    {
        var list = symbol.AllInterfaces;
        BasicList<INamedTypeSymbol> output = [];
        foreach (var item in list)
        {
            if (item.Name == "IJoinedEntity")
            {
                output.Add(item);
            }
        }
        return output;
    }
}