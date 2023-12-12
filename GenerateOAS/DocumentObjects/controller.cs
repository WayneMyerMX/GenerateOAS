public class Endpoint{
    //The name of the resource to which this endpoint belongs.
    public string? Resource {get;set;}

    //The description of the resource to which this endpoint belongs.
    public string? ResourceDesc{get;set;}

    //The name of the endpoint.
    public string? Name {get;set;}

    //Description of what the endpoint does and any other related information.
    public string? Description {get;set;}
    public string? Summary {get;set;}

    //The URL for this endpoint.
    public string? Path {get;set;}
    public List<Parameter> Parameters {get;set;}
    public List<string> Tags {get;set;}
    public Verbs Verb {get;set;}
    public string? ExampleRequest {get;set;}
    public string? ExampleResponse {get;set;}
    public string? Version {get;set;}
    public List<Response> Responses {get;set;}
    public string? ResponseDataType {get;set;}

/// <summary>
/// Default Endpoint constructor and initializer.
/// </summary>
    public Endpoint()
    {
        this.Parameters = new List<Parameter>();
        this.Tags       = new List<string>();
        this.Responses  = new List<Response>();
    }
}

/// <summary>
///
/// </summary>
public class Parameter
{
    public string? ParamName {get;set;}
    public string? DataType {get;set;}
    public string? Example {get;set;}
    public string? Description {get;set;}
    public ParamLocation Location {get;set;}
    public Verbs Verb {get;set;}
    public List<Response> Responses {get;set;}

    public Parameter()
    {
        this.Responses = new List<Response>();
    }
}

public class Response
{
    public string ResponseCode {get;set;}
    public string? ResponseDesc {get;set;}
}

public enum ParamLocation {path, body, query, formData};

public enum Verbs
{
    GET,
    POST,
    PUT,
    DELETE
};