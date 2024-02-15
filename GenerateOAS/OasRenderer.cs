using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

public class OpenApiConverter
{
    /// <summary>
    /// Converts the lists of models and endpoints into OpenAPI JSON.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <param name="models"></param>
    /// <returns></returns>
    public string ConvertToOpenApi(List<Endpoint> endpoints, List<Model> models)
    {
        //Get the OpenAPI introduction blocks.
        object introBlock = BuildOpenApiIntroBlock();

        //Get OpenAPI paths block.
        foreach(var endpoint in endpoints)
        {
            object serializableObj = endpoint.ConvertEndpointToSerializeString;
        }

        //Iterate through list of Endpoints
        /*
         for each Endpoint
            "[path]" :{
                "[verb]": {
                    "tags": [
                        list of tags
                    ],
                    "parameters": [
                        {
                            "name": paramName,
                            "description": desc,
                            "required": isRequired,
                            "in": paramLoc,
                            "schema": {
                                "type": dataTypeOrRef
                            }
                        }
                    ],
                    "responses": {
                        "responseCode": {
                            "description": "responseDesc",
                            "content": {
                                "application/json": {
                                    "schema": {
                                        "type": dataType,
                                        "items (if array)": {
                                            "$ref": "#/components/schemas/dataType"
                                        }
                                    },
                                    "example": {
                                        "jsonExampleResponse"
                                    }
                                }
                            }
                        },
                        "nextResponseCode": {

                        }
                    },
                    "description": endpointDesc,
                    "summary": endpointSummary
                }
            },
        */

        //Build tags array
        /*
        "tags": [
            {
                "name": tagName,
                "description": tagDesc
            }
        ],
        */

        //Build components (datatypes) section
        /*
        "components": {
            "schemas": {
                "dataTypeName": {
                    "type": "dataType"
                }
            }
        }
        */

        //return JsonConvert.SerializeObject(openApi, Formatting.Indented);
        return "";
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

