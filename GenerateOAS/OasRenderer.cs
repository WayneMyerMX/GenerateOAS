using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

public class OpenApiConverter
{
    private const string OPENAPI        = "openapi";
    private const string INFO           = "info";
    private const string TITLE          = "title";
    private const string DESCRIPTION    = "description";
    private const string VERSION        = "version";
    private const string SERVERS        = "servers";
    private const string URL            = "url";
    private const string PATHS          = "paths";
    private const string TAGS           = "tags";
    private const string OP_ID          = "operationId";
    private const string PARAMETERS     = "parameters";
    private const string NAME           = "name";
    private const string REQUIRED       = "required";
    private const string IN             = "in";
    private const string SCHEMA         = "schema";
    private const string SCHEMAS        = "schemas";
    private const string TYPE           = "type";
    private const string RESPONSES      = "responses";
    private const string RESP_CODE      = "responseCode";
    private const string CONTENT        = "content";
    private const string ITEMS          = "items";
    private const string EXAMPLE        = "example";
    private const string COMPONENTS     = "components";
    private const string PROPERTIES     = "properties";

    //Build the full OpenAPI JObject
    public static JObject BuildOpenApi(List<CompoundEndpoint> endpoints, List<Model> models)
    {
        //Seriously, here there be dragons. Modify at your own great risk.
        JObject oas = new JObject(
            //OpenAPI intro block
            new JProperty(OPENAPI, "3.0.0")
            ,new JProperty(INFO, new JObject(
                new JProperty(TITLE, "Platform")
                ,new JProperty(DESCRIPTION, "MX Platform OpenAPI Spec")
                ,new JProperty(VERSION, "1.0")
            ))
            //Servers
            ,new JProperty(SERVERS, new JArray{
                new JObject(new JProperty(URL, "http://api.mx.local:3000"))
            })
            //Paths
            ,new JProperty(PATHS, new JObject(
                from ep in endpoints
                select new JProperty(ep.Path, new JObject(
                    from epdesc in ep.endpointDescriptors
                    select new JProperty(epdesc.Verb.ToString().ToLower(), new JObject(
                        new JProperty(TAGS, new JArray(
                            from tags in epdesc.Tags
                            select new JValue(tags)
                        ))
                        ,new JProperty(OP_ID, new JValue(epdesc.OperationId))
                        ,new JProperty(PARAMETERS, new JArray(
                            from epParam in epdesc.Parameters
                            select new JObject(
                                new JProperty(NAME, epParam.ParamName)
                                ,new JProperty(DESCRIPTION, epParam.Description)
                                ,new JProperty(REQUIRED, epParam.IsRequired)
                                ,new JProperty(IN, epParam.Location.ToString().ToLower())
                                ,new JProperty(SCHEMA, new JObject(
                                    new JProperty(TYPE, epParam.DataType)
                                )))
                        ))
                        //,new JProperty(RESPONSES, )
                    ))
                    )
                )

                ,new JProperty("endpoint path 1", new JObject(
                    new JProperty("endpoint verb", new JObject(
                        new JProperty(TAGS, new JArray{
                            "endpoint tags like Holdings",
                            "another endpoint tag"
                        })
                        ,new JProperty(OP_ID, "operationId here")
                        ,new JProperty(PARAMETERS, new JArray{
                            new JObject(new JProperty(NAME, "paramName")
                                        ,new JProperty(DESCRIPTION, "param description")
                                        ,new JProperty(REQUIRED, "isRequired")
                                        ,new JProperty(IN, "paramLocation")
                                        ,new JProperty(SCHEMA, new JObject(
                                            new JProperty(TYPE, "dataType")
                                        )))
                        })
                        ,new JProperty(RESPONSES, new JObject(
                            new JProperty(RESP_CODE, new JObject(
                                new JProperty(DESCRIPTION, "response description")
                                ,new JProperty(CONTENT, new JObject(
                                    new JProperty("response content type (application/json)", new JObject(
                                        new JProperty(SCHEMA, new JObject(
                                            new JProperty(TYPE, "array")
                                            ,new JProperty(ITEMS, new JObject(
                                                new JProperty("$ref", "#/components/Holding")
                                            ))
                                        ))
                                        ,new JProperty(EXAMPLE, new JObject(
                                            new JProperty("example, like HoldingsResponse", "example string literal of JSON response")
                                        ))
                                    ))
                                ))
                            ))
                            ,new JProperty("nextResponseCode", "more response code objects")
                        ))
                    ))
                ))
            ))
            //Tags array
            ,new JProperty(TAGS, new JArray{
                new JObject(new JProperty(NAME, "tagName1")
                            ,new JProperty(DESCRIPTION, "tag description 1"))
                ,new JObject(new JProperty(NAME, "tagName2")
                            ,new JProperty(DESCRIPTION, "tag description 2"))
            })
            //Components (complex datatypes)
            ,new JProperty(COMPONENTS, new JObject(
                new JProperty(SCHEMAS, new JObject(
                    new JProperty("complexDataType 1", new JObject(
                        new JProperty(TYPE, "object")
                        ,new JProperty(PROPERTIES, new JObject(
                            new JProperty("component datatype name 1 (like 'guid')", new JObject(
                                new JProperty(TYPE, "string/datatype")
                                ,new JProperty(EXAMPLE, "string literal example")
                            ))
                            ,new JProperty("component datatype 2(like 'value')", new JObject(
                                new JProperty(TYPE, "string")
                                ,new JProperty(EXAMPLE, "string literal example")
                            ))
                        ))
                    ))

                ))
            ))

        );

        //There be dragons here. Modify at your own great risk.
        // JObject oas = new JObject(
        //     //OpenAPI intro block
        //     new JProperty(OPENAPI, "3.0.0")
        //     ,new JProperty(INFO, new JObject(
        //         new JProperty(TITLE, "Platform")
        //         ,new JProperty(DESCRIPTION, "MX Platform OpenAPI Spec")
        //         ,new JProperty(VERSION, "1.0")
        //     ))
        //     //Servers
        //     ,new JProperty(SERVERS, new JArray{
        //         new JObject(new JProperty(URL, "http://api.mx.local:3000"))
        //     })
        //     //Paths
        //     ,new JProperty(PATHS, new JObject(
        //         new JProperty("endpoint path 1", new JObject(
        //             new JProperty("endpoint verb", new JObject(
        //                 new JProperty(TAGS, new JArray{
        //                     "endpoint tags like Holdings",
        //                     "another endpoint tag"
        //                 })
        //                 ,new JProperty(OP_ID, "operationId here")
        //                 ,new JProperty(PARAMETERS, new JArray{
        //                     new JObject(new JProperty(NAME, "paramName")
        //                                 ,new JProperty(DESCRIPTION, "param description")
        //                                 ,new JProperty(REQUIRED, "isRequired")
        //                                 ,new JProperty(IN, "paramLocation")
        //                                 ,new JProperty(SCHEMA, new JObject(
        //                                     new JProperty(TYPE, "dataType")
        //                                 )))
        //                 })
        //                 ,new JProperty(RESPONSES, new JObject(
        //                     new JProperty(RESP_CODE, new JObject(
        //                         new JProperty(DESCRIPTION, "response description")
        //                         ,new JProperty(CONTENT, new JObject(
        //                             new JProperty("response content type (application/json)", new JObject(
        //                                 new JProperty(SCHEMA, new JObject(
        //                                     new JProperty(TYPE, "array")
        //                                     ,new JProperty(ITEMS, new JObject(
        //                                         new JProperty("$ref", "#/components/Holding")
        //                                     ))
        //                                 ))
        //                                 ,new JProperty(EXAMPLE, new JObject(
        //                                     new JProperty("example, like HoldingsResponse", "example string literal of JSON response")
        //                                 ))
        //                             ))
        //                         ))
        //                     ))
        //                     ,new JProperty("nextResponseCode", "more response code objects")
        //                 ))
        //             ))
        //         ))
        //     ))
        //     //Tags array
        //     ,new JProperty(TAGS, new JArray{
        //         new JObject(new JProperty(NAME, "tagName1")
        //                     ,new JProperty(DESCRIPTION, "tag description 1"))
        //         ,new JObject(new JProperty(NAME, "tagName2")
        //                     ,new JProperty(DESCRIPTION, "tag description 2"))
        //     })
        //     //Components (complex datatypes)
        //     ,new JProperty(COMPONENTS, new JObject(
        //         new JProperty(SCHEMAS, new JObject(
        //             new JProperty("complexDataType 1", new JObject(
        //                 new JProperty(TYPE, "object")
        //                 ,new JProperty(PROPERTIES, new JObject(
        //                     new JProperty("component datatype name 1 (like 'guid')", new JObject(
        //                         new JProperty(TYPE, "string/datatype")
        //                         ,new JProperty(EXAMPLE, "string literal example")
        //                     ))
        //                     ,new JProperty("component datatype 2(like 'value')", new JObject(
        //                         new JProperty(TYPE, "string")
        //                         ,new JProperty(EXAMPLE, "string literal example")
        //                     ))
        //                 ))
        //             ))

        //         ))
        //     ))

        // );

        return oas;
    }

    /// <summary>
    /// Builds the OpenAPI, Info, and Servers blocks
    /// </summary>
    /// <returns>An anonymous object containing the intro block</returns>
    public object BuildOpenApiIntroBlock()
    {
        //Create anonymous object as intro block.
        //TODO: make these app.config values.
        var intro = new
            {
                openapi = "3.0.0",
                info = new {
                    title = "Platform",
                    description = "MX Platform OpenAPI",
                    version = "1.0"
                },
                servers = new []
                    {
                        new {url = "http://api.mx.local:3000"}
                    }
            };

        return intro;
    }
}



public class ClassesToJsonConverter
{
    public static string ConvertObjects<T>(List<T> listOfOjbects)
    {
        StringBuilder sbJson = new StringBuilder();
        Type listType = listOfOjbects[0].GetType();

        foreach(var obj in listOfOjbects)
        {
            sbJson.AppendLine(JsonConvert.SerializeObject(obj, Formatting.None));
        }

        return sbJson.ToString();
    }

    public static string ConvertEndpoints(List<Endpoint> endpoints)
    {
        StringBuilder sbJson = new StringBuilder();

        foreach(var ep in endpoints)
        {
           sbJson.AppendLine(ep.ConvertEndpointToSerializeString());
        }

        return sbJson.ToString();
    }
}

