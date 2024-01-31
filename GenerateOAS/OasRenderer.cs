using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

public class OpenApiConverter
{
    public string ConvertToOpenApi(string json)
    {
        var endpoint = JsonConvert.DeserializeObject<Endpoint>(json);
        var openApi = new { openApi = "3.0"};
        //Build OpenAPI intro block.

        //Build OpenAPI server block.

        //Build Open API paths block.
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

        return JsonConvert.SerializeObject(openApi, Formatting.Indented);
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
}

/*
{
    "openapi": "3.0.0",
    "info": {
        "title": "[titleConfigValue]",
        "description": "[descriptionConfigValue]",
        "version": "[versionConfigValue]"               ---but should this come from the endpoint versions?
    },
    "servers": [
        {
        "url": "[serversConfigValue]"
        }
    ],
    "paths": {
        "[endpoint.Path]": {
            "[endpoint.Verb]": {
                "tags": [
                    "[endpoint.Tags.ToArray().ToString()]"
                ],
                "operationId": ["endpoint.OperationId"],
                "parameters": [
                    "name": "paramName",
                    "description": "paramDesc",
                    "required": paramRequired,
                    "in": "paramLocation",
                    "schema": {
                        "type"/"$ref": "requestParamDataType" / "#/components/schemas/complexDataTypeName"
                    },
                    "name": "paramName",
                    "description": "paramDesc",
                    "required": paramRequired,
                    "in": "paramLocation",
                    "schema": {
                        "type"/"$ref": "requestParamDataType" / "#/components/schemas/complexDataTypeName"
                    }
                ],
                "responses": {
                    "default": {
                        "description": "descString",
                        "content": {
                            "application/json": {
                                "schema": {
                                    "$ref": "#/components/schemas/complexDataType"
                                },
                                "example": {
                                    "dataType": {
                                        fieldsAndExampleResponses
                                    }
                                }
                            }
                        }
                    },
                    "200": {
                        "description": "200Description"
                    }
                },
                "description": "This endpoint description."
                "summary": "This endpoint summary."
            }
        }
    },
    "tags": [
        {
            "name": "tagName",
            "description": "tagDescription"
        }
    ],
    "components": {
        "schemas": {
            "modelName": {
                "type": "object",
                "properties": {
                    "fieldName1": {
                        "type": "fieldDataType".
                        "example": "fieldExample"
                    },
                    "fieldName2": {
                        "type": "fieldDataType".
                        "example": "fieldExample"
                    }
                }
            },
            "modelName": {
                "type": "object",
                "properties": {
                    "fieldName1": {
                        "type": "fieldDataType".
                        "example": "fieldExample"
                    },
                    "fieldName2": {
                        "type": "fieldDataType".
                        "example": "fieldExample"
                    }
                }
            }
        }
    }
}



*/


//Autogen stubs of routes and models
/*
Route:
[Route("endpoint URL")]
public partial class EndpointName : Controller ()

[Route("api/[controller]")]
[ApiController]
public class ControllerName : ControllerBase
{
    // GET: api/Employee
    [HttpGet]
    public IEnumerable<Employee> Get()
    {
        return GetEmployees();
    }

    // GET: api/Employee/5
    [HttpGet("{id}", Name = "Get")]
    public Employee Get(int id)
    {
        return GetEmployees().Find(e => e.Id == id);
    }

    // POST: api/Employee
    [HttpPost]
    [Produces("application/json")]
    public Employee Post([FromBody] Employee employee)
    {
        // Logic to create new Employee
        return new Employee();
    }

    // PUT: api/Employee/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] Employee employee)
    {
        // Logic to update an Employee
    }

    // DELETE: api/Employee/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
    private List<Employee> GetEmployees()
    {
        return new List<Employee>()
        {
            new Employee()
            {
                Id = 1,
                FirstName= "John",
                LastName = "Smith",
                EmailId ="John.Smith@gmail.com"
            },
            new Employee()
            {
                Id = 2,
                FirstName= "Jane",
                LastName = "Doe",
                EmailId ="Jane.Doe@gmail.com"
            }
        };
    }
}

Model:
*/