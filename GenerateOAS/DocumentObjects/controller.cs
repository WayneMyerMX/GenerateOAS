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
            //Create parameters object.
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

        //Build out the responses list.
        List<JObject> jsonResponsesList = new List<JObject>();

        foreach(Response r in this.Responses)
        {
            string respdesc;
            JObject formattedResponse = new JObject();

            //204 responses are structurally different from other responses.
            if(r.ResponseCode == "204")
            {
                respdesc = r.ResponseDesc;
                formattedResponse = new JObject(
                    new JProperty(r.ResponseCode, new JObject(
                        new JProperty("description", respdesc)
                        )
                    )
                );
                jsonResponsesList.Add(formattedResponse);
                continue;
            }

            respdesc = string.IsNullOrEmpty(this.Responses[0].ResponseDesc) ? string.Empty : this.Responses[0].ResponseDesc;
            formattedResponse = new JObject(
            new JProperty(r.ResponseCode, new JObject(
                new JProperty("description", r.ResponseDesc)
                ,new JProperty("content", new JObject(
                    new JProperty("application/json", new JObject(
                        new JProperty("schema", new JObject(
                            new JProperty("$ref", "link to datatype")
                        ))
                    ))
                ))
            ))
            );
            jsonResponsesList.Add(formattedResponse);
        }

        //TODO: prevent the responses from being parsed as an array
        //Build out the endpoint template.
        JObject endpoint = JObject.FromObject(new
            {
                epPath = new {
                    epVerb = new {
                        tags = new JArray(this.Tags.ToArray())
                        ,operationId = string.IsNullOrEmpty(this.OperationId) ? string.Empty : this.OperationId
                        ,parameters = new JArray(endPtParams.ToArray())
                        ,responses = jsonResponsesList
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

public class CompoundEndpoint
{
    /// <summary>
    /// Array of primitive data types for refining parameter schemas.
    /// </summary>
    public static string[] Primitives = {"array", "integer", "string", "double", "long", "short", "char", "number"};

    /// <summary>
    /// The URL for this endpoint.
    /// </summary>
    public string? Path {get;set;}

    /// <summary>
    /// The name of the resource to which this endpoint belongs.
    /// </summary>
    public string? Resource {get;set;}

    /// <summary>
    /// The description of the resource to which this endpoint belongs.
    /// </summary>
    public string? ResourceDesc{get;set;}

    public List<EndpointDescriptors> endpointDescriptors {get; set;}

    public CompoundEndpoint()
    {
        endpointDescriptors = new List<EndpointDescriptors>();
    }

    public class EndpointDescriptors
    {
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
        public EndpointDescriptors()
        {
            this.Parameters = new List<Parameter>();
            this.Tags       = new List<string>();
            this.Responses  = new List<Response>();
        }
    }

    /// <summary>
    /// Returns an list of endpoints, grouped by common path.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static List<CompoundEndpoint> AggregateEndpointsByPath(List<Endpoint> endpoints)
        {
            var aggdEndpoints = new List<CompoundEndpoint>();

            //Yes, there is a more elegant way to build this with LINQ, but I want to keep this easier for other people to pick up.
            //Build an array of all the paths.
            var paths = endpoints.Select(ep => ep.Path).Distinct().ToArray();

            //For each path, build out a compound endpoint.
            foreach(string p in paths)
            {
                CompoundEndpoint cep = new CompoundEndpoint{Path = p};
                var matchingEndpoints = endpoints.Where(ep=>ep.Path.Equals(p));

                //From the matching endpoints, fill out the compound endpoint.
                foreach(Endpoint match in matchingEndpoints)
                {
                    cep.Resource     = match.Resource;
                    cep.ResourceDesc = match.ResourceDesc;

                    //Build the individual descriptor, i.e. the individual endpoint.
                    var descriptor = new EndpointDescriptors
                    {
                        Name            = match.Name,
                        Description     = match.Description,
                        Summary         = match.Summary,
                        Parameters      = match.Parameters,
                        Tags            = match.Tags,
                        Verb            = match.Verb,
                        ExampleRequest  = match.ExampleRequest,
                        ExampleResponse = match.ExampleResponse,
                        Version         = match.Version,
                        Responses       = match.Responses,
                        ResponseDataType = match.ResponseDataType,
                        OperationId     = match.OperationId
                    };
                    cep.endpointDescriptors.Add(descriptor);
                }

                aggdEndpoints.Add(cep);
            }

            //Load parameters, examples, and properties into an endpoint descriptor object

            return aggdEndpoints;
        }
}