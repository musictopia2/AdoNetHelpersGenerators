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
    private BasicList<PropertyModel> GetProperties(INamedTypeSymbol classSymbol)
    {
        BasicList<PropertyModel> output = [];
        var firsts = classSymbol.GetAllPublicProperties();
        foreach (var item in firsts)
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
            rets = item.TryGetAttribute("ForeignKey", out attributes);
            if (rets == false)
            {
                p.ForeignKey = "";
            }
            else
            {
                p.ForeignKey = attributes.Single().ConstructorArguments.Single().Value!.ToString();
            }
            output.Add(p);
        }
        return output;
    }
}