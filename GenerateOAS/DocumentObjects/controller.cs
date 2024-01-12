public class Endpoint{
    /// <summary>
    /// The name of the resource to which this endpoint belongs.
    /// </summary>
    public string? Resource {get;set;}

    /// <summary>
    /// The description of the resource to which this endpoint belongs.
    /// </summary>
    public string? ResourceDesc{get;set;}

    /// <summary>
    /// The name of the endpoint.
    /// </summary>
    public string? Name {get;set;}

    /// <summary>
    /// Description of what the endpoint does and any other related information.
    /// </summary>
    public string? Description {get;set;}

    /// <summary>
    /// Summary text of the endpoint.
    /// </summary>
    public string? Summary {get;set;}

    /// <summary>
    /// The URL for this endpoint.
    /// </summary>
    public string? Path {get;set;}

    /// <summary>
    /// List of parameters used to call this endpoint.
    /// </summary>
    public List<Parameter> Parameters {get;set;}

    /// <summary>
    /// List of Tags to apply to this endpoint.
    /// </summary>
    public List<string> Tags {get;set;}

    /// <summary>
    /// HTTP verb for this endpoint.
    /// </summary>
    public Verbs Verb {get;set;}

    /// <summary>
    /// JSON representation of an acceptable example request for this endpoint.
    /// </summary>
    public string? ExampleRequest {get;set;}

    /// <summary>
    /// JSON representation of an example response from this endpoint.
    /// </summary>
    public string? ExampleResponse {get;set;}

    /// <summary>
    /// The API version to which this endpoint belongs.
    /// </summary>
    public string? Version {get;set;}

    /// <summary>
    /// List of possible standard responses from this endpoint.
    /// </summary>
    public List<Response> Responses {get;set;}

    /// <summary>
    /// The datatype, if applicable, of a successful API response.
    /// </summary>
    public string? ResponseDataType {get;set;}

    /// <summary>
    /// Unique string used to identify the operation. The id MUST be unique among all operations described in the API. Tools and libraries MAY use the operationId to uniquely identify an operation, therefore, it is RECOMMENDED to follow common programming naming conventions.
    /// </summary>
    public string? OperationId {get; set;}

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
/// Holds the relevant values for a single endpoint paramter, including the name, datatype of the param, description of the param, and where the param is located in the request.
/// </summary>
public class Parameter
{
    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public string? ParamName {get;set;}

    /// <summary>
    /// The datatype of the parameter.
    /// </summary>
    public string? DataType {get;set;}

    /// <summary>
    /// Text description of the parameter.
    /// </summary>
    public string? Description {get;set;}

    /// <summary>
    /// Where the parameter is located in the request.
    /// </summary>
    public ParamLocation Location {get;set;}

}

/// <summary>
/// Holds the HTTP response code and description for a given response from an endpoint.
/// </summary>
public class Response
{
    public string ResponseCode {get;set;}
    public string? ResponseDesc {get;set;}
}

public enum ParamLocation {path, body, query, formData, header};

public enum Verbs
{
    GET,
    POST,
    PUT,
    DELETE
};