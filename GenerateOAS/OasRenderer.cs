using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

public class OpenApiConverter
{
    //Build the full OpenAPI JObject
    public JObject BuildOpenApi(List<Endpoint> endpoints, List<Model> models)
    {
        //There be dragons here. Modify at your own great risk.
        JObject oas = new JObject(
            //OpenAPI intro block
            new JProperty("openapi", "3.0.0")
            ,new JProperty("info", new JObject(
                new JProperty("title", "Platform")
                ,new JProperty("description", "Mx Platform OpenAPI Spec")
                ,new JProperty("version", "1.0")
            ))
            //Servers
            ,new JProperty("servers", new JArray{
                new JObject(new JProperty("url", "http://api.mx.local:3000"))
            })
            //Paths
            ,new JProperty("paths", new JObject(
                new JProperty("endpoint path 1", new JObject(
                    new JProperty("endpoint verb", new JObject(
                        new JProperty("tags", new JArray{
                            "endpoint tags like Holdings",
                            "another endpoint tag"
                        })
                        ,new JProperty("operationId", "operationId here")
                        ,new JProperty("parameters", new JArray{
                            new JObject(new JProperty("name", "paramName")
                                        ,new JProperty("description", "param description")
                                        ,new JProperty("required", "isRequired")
                                        ,new JProperty("in", "paramLocation")
                                        ,new JProperty("schema", new JObject(
                                            new JProperty("type", "dataType")
                                        )))
                        })
                        ,new JProperty("responses", new JObject(
                            new JProperty("responseCode", new JObject(
                                new JProperty("description", "response description")
                                ,new JProperty("content", new JObject(
                                    new JProperty("response content type (application/json)", new JObject(
                                        new JProperty("schema", new JObject(
                                            new JProperty("type", "array")
                                            ,new JProperty("items", new JObject(
                                                new JProperty("$ref", "#/components/Holding")
                                            ))
                                        ))
                                        ,new JProperty("example", new JObject(
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
            ,new JProperty("tags", new JArray{
                new JObject(new JProperty("name", "tagName1")
                            ,new JProperty("description", "tag description 1"))
                ,new JObject(new JProperty("name", "tagName2")
                            ,new JProperty("description", "tag description 2"))
            })
            //Components (complex datatypes)
            ,new JProperty("components", new JObject(
                new JProperty("schemas", new JObject(
                    new JProperty("complexDataType 1", new JObject(
                        new JProperty("type", "object")
                        ,new JProperty("properties", new JObject(
                            new JProperty("component datatype name 1 (like 'guid')", new JObject(
                                new JProperty("type", "string/datatype")
                                ,new JProperty("example", "string literal example")
                            ))
                            ,new JProperty("component datatype 2(like 'value')", new JObject(
                                new JProperty("type", "string")
                                ,new JProperty("example", "string literal example")
                            ))
                        ))
                    ))

                ))
            ))

        );

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

