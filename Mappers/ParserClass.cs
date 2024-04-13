namespace AdoNetHelpersGenerators.Mappers;
public class ParserClass(IEnumerable<ClassDeclarationSyntax> list, Compilation compilation)
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
        
        bool rets = symbol.TryGetAttribute("Table", out var attributes);
        if (rets == false)
        {
            output.TableName = output.ClassName;
        }
        else
        {
            output.TableName = attributes.Single().ConstructorArguments.Single().Value!.ToString();
        }
        output.Properties = GetProperties(symbol);
        output.AutoIncremented = output.Properties.All(x => x.NoIncrement == false);
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
    private BasicList<PropertyModel> GetProperties(INamedTypeSymbol classSymbol)
    {
        BasicList<PropertyModel> output = [];
        //has to get the information needed here for joined tables.
        var firsts = GetJoinedTables(classSymbol);
        HashSet<INamedTypeSymbol> possibleFroms = [];
        foreach (var item in firsts)
        {
            var nexts = item.TypeArguments;
            foreach (var next in nexts)
            {
                INamedTypeSymbol ss = (INamedTypeSymbol)next;
                possibleFroms.Add(ss);
            }
        }
        var seconds = classSymbol.GetAllPublicProperties();
        foreach (var item in seconds)
        {
            if (item.IsReadOnly)
            {
                continue;
            }
            if (item.HasAttribute("NotMapped"))
            {
                continue;
            }
            PropertyModel p = item.GetStartingPropertyInformation<PropertyModel>();
            if (p.VariableCustomCategory == EnumSimpleTypeCategory.None)
            {
                continue;
            }
            p.NoIncrement = item.HasAttribute("NoIncrement");
            p.CommonForUpdating = item.HasAttribute("CommonField");
            p.UseForPossibleJoin = item.HasAttribute("PrimaryJoinedData");

            if (p.PropertyName.ToLower() == "id")
            {
                p.IsIDField = true;
                p.CommonForUpdating = false;
            }
            bool rets = item.TryGetAttribute("Column", out var attributes);
            if (rets == false)
            {
                p.ColumnName = p.PropertyName;
            }
            else
            {
                p.ColumnName = attributes.Single().ConstructorArguments.Single().Value!.ToString();
            }
            if (p.ColumnName.ToLower() == "id")
            {
                p.IsIDField = true;
                p.CommonForUpdating = false; //try this to fix the mapping problem for cases where another property was used but still maps over.
            }
            rets = item.TryGetAttribute("ForeignKey", out attributes);
            string possibleForeignKey;
            if (rets == false)
            {
                possibleForeignKey = "";
                //p.ForeignKey = "";
            }
            else
            {
                possibleForeignKey = attributes.Single().ConstructorArguments.Single().Value!.ToString();
                //p.ForeignKey = attributes.Single().ConstructorArguments.Single().Value!.ToString();
            }
            if (possibleForeignKey != "")
            {
                p.ForeignKey = GetRealKey(possibleForeignKey, possibleFroms);
            }
            output.Add(p);
        }
        return output;
    }
    private string GetRealKey(string possibleKey, HashSet<INamedTypeSymbol> joinings)
    {
        INamedTypeSymbol? found = joinings.SingleOrDefault(x => x.Name == possibleKey);
        if (found is null)
        {
            return possibleKey;
        }
        bool rets = found.TryGetAttribute("Table", out var attributes);
        if (rets == false)
        {
            return possibleKey; //because nothing else.
        }
        return attributes.Single().ConstructorArguments.Single().Value!.ToString();
    }
}