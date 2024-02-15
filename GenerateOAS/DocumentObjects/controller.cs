using System.Reflection.Metadata;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Endpoint{
    /// <summary>
    /// Array of primitive data types for refining parameter schemas.
    /// </summary>
    public static string[] Primitives = {"array", "integer", "string", "double", "long", "short", "char", "number"};

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

/// <summary>
/// Converts an endpoint object into an OpenAPI JSON-serializable POCO.
/// </summary>
/// <returns>POCO that is serializable to OpenAPI JSON.</returns>
    public string ConvertEndpointToSerializeString()
    {
        //Build out list of parameters as a JArray.
        List<JObject> endPtParams = new List<JObject>();

        //Run through the list of endpoint parameters and build out name, description, required, in, and schema.
        foreach(Parameter param in this.Parameters)
        {
            JObject paramJObj = new JObject(
                new JProperty("name", param.ParamName)
                ,new JProperty("description", param.Description)
                ,new JProperty("required", param.IsRequired)
                ,new JProperty("in", param.Location.ToString())
                ,new JProperty("schema",
                    new JObject(new JProperty("type", param.DataType.ToString())))
            );

            endPtParams.Add(paramJObj);
        }

        //Build out the responses object.
        foreach(Response r in this.Responses)
        {
            string respdesc;

            //204 responses are empty and structurally different from other responses.
            if(r.ResponseCode == "204")
            {
                respdesc = r.ResponseDesc;
            }


            if(this.Responses.Count > 0)
            {
                respdesc = string.IsNullOrEmpty(this.Responses[0].ResponseDesc) ? string.Empty : this.Responses[0].ResponseDesc;
            }
            else
            {
                respdesc = string.Empty;
            }

            JObject response1 = new JObject(
                new JProperty("default", new JObject(
                    new JProperty("description", "responsedesc")
                    ,new JProperty("content", new JObject(
                        new JProperty("application/json", new JObject(
                            new JProperty("schema", new JObject(
                                new JProperty("$ref", "link to datatype")
                            ))
                        ))
                    ))
                ))
            );
        }




        // JObject response1 = new JObject(
        //     new JProperty("default",
        //         new JObject(new JProperty("description", respdesc))
        //         ,new JProperty("content"
        //             ,new JObject(new JProperty("content", "contentObj")))
        //         //  ,new JObject(
        //         //     new JProperty("application/json", new JObject(
        //         //         new JProperty("schema", new JObject(
        //         //             new JProperty("ref/schema", "#components/schema/Tag")
        //         //             )
        //         //         )
        //         //     ))
        //         // ))
        //     )
        // );
        /*
        "responses": {
            "default": {
                "description": "",
                "content": {
                    "application/json": {
                        "schema": {
                            primitive or $ref
                        }
                    }
                }
            }
        },
        [response code]: {
            "description": response description
        }
        */

        //Build out the endpoint template.
        JObject endpoint = JObject.FromObject(new
            {
                epPath = new {
                    epVerb = new {
                        tags = new JArray(this.Tags.ToArray())
                        ,operationId = string.IsNullOrEmpty(this.OperationId) ? string.Empty : this.OperationId
                        ,parameters = new JArray(endPtParams.ToArray())
                        ,responses = "responses here"
                        ,description = this.Description
                        ,summary = string.IsNullOrEmpty(this.Summary) ? string.Empty : this.Summary
                    }
                }
            }
        );

        //Serialize the endpoint template.
        StringBuilder sb = new StringBuilder(JsonConvert.SerializeObject(endpoint, Formatting.Indented));

        //Replace the path and verb, which can't be done with the anonymous object above.
        sb.Replace("epPath", this.Path);
        sb.Replace("epVerb", this.Verb.ToString().ToLower());

        return sb.ToString();
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
    /// <summary>
    /// Indicates whether this parameter is required to use this endpoint.
    /// </summary>
    public bool IsRequired{get; set;}

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