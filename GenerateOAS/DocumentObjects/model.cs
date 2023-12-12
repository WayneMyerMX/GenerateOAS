public class Model
{
    public string?          ModelName {get;set;}
    public string?          Description {get;set;}
    public string?          Summary {get;set;}
    public List<Property>  Properties {get;set;}
    public string? Version {get;set;}

    public Model()
    {
        this.Properties = new List<Property>();
    }
}

public class Property
{
    public string?  Name {get;set;}
    public string?  DataType {get;set;}
    public string?  Description {get;set;}
    public bool?    IsRequired {get;set;}
    public bool?    IsNullable {get;set;}
    public string?  Example {get;set;}
}

//string output = YT.Split(':').Last();