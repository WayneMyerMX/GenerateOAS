using System.Text;

internal partial class Program
{
    #region CONSTANTS
    static string[] MODEL_KEYWORDS     = { "@!model", "@model" };
    static string[] NULL_OR_REQ        = { "nullable", "required" };
    static string[] PARAM_LOCS         = { "query", "path", "body" };
    static string[] ENDPOINT_KEYWORDS  = { "@path", "@parameter", "@response" };
    const string PATH           = "@path";
    const string PARAM          = "@parameter";
    const string RESPONSE       = "@response "; //Trailing space to discrimate between this and @response_type.
    const string RESPONSE_TYPE  = "@response_type";
    const string EXAMPLE        = "@example";
    const string EXAMPLE_REQ    = "@example_request";
    const string SWAGGER_KEY    = "##";
    const string RESOURCE_TAG   = "@resource";
    #endregion


    private static void Main(string[] args)
    {
        #region Program Variables

        //List of model docs
        List<Model> _models = new List<Model>();
        //List of controller docs
        List<Endpoint> _endpoints = new List<Endpoint>();
        //Get paths of all models
        string[] modelFiles = Directory.GetFiles("/Users/wayne.myer/gitlab/harvey-master/harvey/app/models/platform", "*.rb", SearchOption.TopDirectoryOnly);
        //Get paths of all controllers.
        string[] controllerFiles = Directory.GetFiles("/Users/wayne.myer/gitlab/harvey-master/harvey/app/controllers/api/platform", "*.rb", SearchOption.TopDirectoryOnly);

        #endregion

        //Iterate over list of model paths, parsing out name, properties, descriptions, and examples
        ParseModelDocs(_models, modelFiles);

        //Go through list of controller paths.
        ParseControllerDocs(_endpoints, controllerFiles);

        string deleteMe = ClassesToJsonConverter.ConvertObjects(_endpoints);
        Console.WriteLine(deleteMe);
    }

    /// <summary>
    /// Parses out the property lines from model docs.
    /// </summary>
    /// <param name="modelLines"></param>
    /// <param name="i"></param>
    /// <param name="propertyLine"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    static int ParseModelProperty(List<string> modelLines, int i, string propertyLine, Property property)
    {
        //Get the description and save to property.Description.
        string? description =
            propertyLine.Split(']')
                .Last()
                .Trim();

        if (!string.IsNullOrEmpty(description))
        {
            property.Description = description;
        }

        //Check for multiline property description and parse if present.
        bool hasMoreLines = modelLines.Count > i + 1;
        bool isPropertyLine = modelLines[i].Contains("@property");
        bool isExtendedDescription = hasMoreLines &&
                                        isPropertyLine &&
                                        modelLines[i + 1][0] == '#' &&
                                        !modelLines[i + 1].Contains("@example") &&
                                        !modelLines[i + 1].Contains("@property");

        //If there is an extended property description, add that line to the mode.
        if (isExtendedDescription)
        {
            i++;
            property.Description += "\n\n" + modelLines[i].Split('#').Last().Trim();
        }

        return i;
    }

    /// <summary>
    /// Runs through the controller and parses out the resource block.
    /// </summary>
    /// <param name="SWAGGER_KEY"></param>
    /// <param name="RESOURCE_TAG"></param>
    /// <param name="controllerLines"></param>
    /// <param name="resource"></param>
    /// <param name="resourceDesc"></param>
    /// <param name="i"></param>
    /// <returns></returns>
    static int ParseControllerResourceBlock(string SWAGGER_KEY, string RESOURCE_TAG, List<string> controllerLines, StringBuilder resource, StringBuilder resourceDesc, int i)
    {
        string line = controllerLines[i];

        //Parse the @resource block.
        resource.Append(
            controllerLines[i].Split(RESOURCE_TAG)
                                .Last()
                                .Trim());
        i++;

        //Get the resource description, which can be multiline.
        //Walk forward from the @resource line until the first line without a #.
        int endOfResourceBlock = 0;
        StringBuilder resourceDescSBuilder = new StringBuilder();

        while (controllerLines[i].Contains('#'))
        {
            var currentLine = controllerLines[i + endOfResourceBlock].Split('#').Last().Trim();

            if (currentLine.Length > 1)
            {
                resourceDesc.AppendLine(currentLine);
            }

            i++;
        }

        //Done with @resource block.
        return i;
    }


    /// <summary>
    /// Drives the parsing of all controllers in the designated folders.
    /// </summary>
    /// <param name="_endpoints"></param>
    /// <param name="controllerFiles"></param>
    private static void ParseControllerDocs(List<Endpoint> _endpoints, string[] controllerFiles)
    {
        //For every individual controller file, walk through the swagger_yard syntax and serialize into POCO.
        foreach (string controllerFilePath in controllerFiles)
        {
            ParseIndividualController(_endpoints, controllerFilePath);
        }
    }

    private static void ParseIndividualController(List<Endpoint> _endpoints, string controllerFilePath)
    {
        //Hold the resource name and description for all of the endpoints in this controller.
        StringBuilder resource = new StringBuilder();
        StringBuilder resourceDesc = new StringBuilder();
        List<string> controllerLines;
        //Pull all of this controller's lines into a list for parsing.
        using (StreamReader srControllerReader = new StreamReader(controllerFilePath))
        {
            controllerLines = new List<string>();

            while (!srControllerReader.EndOfStream)
            {
                controllerLines.Add(srControllerReader.ReadLine());
            }

            //Check for the @resource tag. If tag is present, controller is swaggerized. Otherwise go to the next controller.
            if (controllerLines.Any(p => p.Contains(RESOURCE_TAG)))
            {
                int resourceTagIndex = controllerLines.FindIndex(res => res.Contains(RESOURCE_TAG));

                //Parse resource block.
                int controllerIndex = ParseControllerResourceBlock(SWAGGER_KEY, RESOURCE_TAG, controllerLines, resource, resourceDesc, resourceTagIndex);

                //Proceed with parsing all of the endpoints in this controller
                controllerIndex = ParseControllerEndpoints(_endpoints, resource, resourceDesc, controllerLines, controllerIndex);
            }
        }
    }

    private static int ParseControllerEndpoints(List<Endpoint> _endpoints, StringBuilder resource, StringBuilder resourceDesc, List<string> controllerLines, int controllerIndex)
    {
        for (; controllerIndex < controllerLines.Count; controllerIndex++)
        {
            //Advance the index until a SWAGGER_KEY, path, and end of endpoint block are found.
            while (controllerIndex < controllerLines.Count && !controllerLines[controllerIndex].Contains('#'))
            {
                controllerIndex++;
            }

            //Look for endpoint blocks.
            List<string> endpointRawBlock = new List<string>();

            while (controllerIndex < controllerLines.Count && controllerLines[controllerIndex].Contains('#'))
            {
                endpointRawBlock.Add(controllerLines[controllerIndex]);
                controllerIndex++;
            }

            //Does the endpoint block contain @path?  Yes, it's an endpoint.
            if (endpointRawBlock.Any(p => p.Contains(PATH)))
            {
                Endpoint endpoint = new Endpoint();

                //Assemble the endpoint docs that are in this raw block.
                //Format is:
                //##
                //# description text (can be multiline)
                //# @path verb /pathName
                //# @parameter (location: body | path | query) [dataType] parameterDescription (can be multiline)
                //# @example_request
                //#     example request text
                //# @response_type [responseDataType]
                //# @response 200 200ResponseText
                //# @response 500 500ResponseText
                //# @example_response
                //#     example response text

                //Assign resource name and resource description.
                endpoint.Resource = resource.ToString();
                endpoint.ResourceDesc = resourceDesc.ToString();
                int pathIndex = endpointRawBlock.FindIndex(path => path.Contains(PATH));
                //Parse the HTTP verb.
                endpoint.Verb = (Verbs)Enum.Parse(typeof(Verbs), endpointRawBlock[pathIndex].Split('[').Last().Split(']').First());
                //Parse the endpoint path.
                endpoint.Path = endpointRawBlock[pathIndex].Split(']').Last().Trim();
                //Parse description text (every line between index = 1 and pathIndex).
                StringBuilder description = new StringBuilder();
                for (int d = 1; d < pathIndex; d++)
                {
                    string descLine = endpointRawBlock[d].Split('#').Last().Trim();

                    if (descLine.Length > 0)
                    {
                        description.AppendLine(descLine);
                    }
                }

                endpoint.Description = description.ToString();

                //Parse parameters.
                //Find the first and last parameter.
                int firstParamIndex = endpointRawBlock.FindIndex(param => param.Contains(PARAM));
                int lastParamIndex = endpointRawBlock.FindLastIndex(param => param.Contains(PARAM));

                for (int pIdx = firstParamIndex; pIdx <= lastParamIndex && pIdx > 0; pIdx++)
                {
                    string paramLine = endpointRawBlock[pIdx];
                    Parameter parameter = new Parameter();
                    //Split the string after "parameter".
                    //Everything up to '(' is param name.
                    parameter.ParamName = paramLine.Split(PARAM).Last().Split('(').First().Trim();
                    //Get param location (in parens).
                    parameter.Location = (ParamLocation)Enum.Parse(typeof(ParamLocation), paramLine.Split('(')
                        .Last()
                        .Split(')')
                        .First()
                        .Trim());
                    //Get param data type (in []).
                    //TODO: Warn if data type is missing in this line.
                    parameter.DataType = paramLine.Split('[').Last().Split(']').First().Trim();
                    //Get param description (can be multiline).
                    StringBuilder sbParaDesc = new StringBuilder();
                    sbParaDesc.Append(paramLine.Split(']').Last().Trim());
                    int paramBlockIdx = 1;
                    while (pIdx + 1 < lastParamIndex && !endpointRawBlock[pIdx + paramBlockIdx].Contains(PARAM))
                    {
                        sbParaDesc.AppendLine(endpointRawBlock[pIdx + paramBlockIdx].Split('#').Last().Trim());
                        paramBlockIdx++;
                    }
                    parameter.Description = sbParaDesc.ToString();
                    endpoint.Parameters.Add(parameter);
                    //Move to the next parameter.
                }

                //Endpoints can return multiple resonse codes. Parse responses docs for this endpoint.
                int firstRespIndex = endpointRawBlock.FindIndex(resp => resp.Contains(RESPONSE));
                int lastRespIndex = endpointRawBlock.FindLastIndex(resp => resp.Contains(RESPONSE));

                for (int respIdx = firstRespIndex; respIdx <= lastRespIndex && respIdx > 0; respIdx++)
                {
                    Response response = new Response();
                    string respLine = endpointRawBlock[respIdx];
                    string responseCode = respLine.Split(RESPONSE).Last().Trim().Split(' ').First().Trim();
                    string responseDesc = respLine.Split(responseCode).Last().Trim();
                    response.ResponseCode = responseCode;
                    response.ResponseDesc = responseDesc;
                    endpoint.Responses.Add(response);
                }

                //Parse response_type for this endpoint.
                //TODO: some Platform endpoints can return multiple datatypes. That is probably best handled by
                //an additional documentation block.
                if(endpointRawBlock.Any(rType => rType.Contains(RESPONSE_TYPE)))
                {
                    string respTypeLine = endpointRawBlock[endpointRawBlock.FindIndex(respType => respType.Contains(RESPONSE_TYPE))];
                    string respType = respTypeLine.Split('[').Last().Trim('[').Trim(']').Trim();
                    endpoint.ResponseDataType = respType;
                }

                //Parse example request for this endpoint.
                int exampleReqIdx = endpointRawBlock.FindIndex(exReq => exReq.Contains(EXAMPLE_REQ));

                if(exampleReqIdx > 1)
                {
                    //The next line is the JSON of the example request.
                    endpoint.ExampleRequest = endpointRawBlock[exampleReqIdx + 1];
                }

                //Parse example response for this endpoint. Disambiguate between @example and @example_request.
                int exampleRespIdx = endpointRawBlock.FindIndex(exResp => exResp.Contains(EXAMPLE) && !exResp.Contains(EXAMPLE_REQ));

                if(exampleRespIdx > 1)
                {
                    //The next line is the JSON of the example response.
                    endpoint.ExampleResponse = endpointRawBlock[exampleRespIdx + 1].Trim().Trim('#').Trim();
                }

                //Finshed parsing this endpoint. Save the object, clear the controllerLines list.
                _endpoints.Add(endpoint);
            }
        }

        return controllerIndex;
    }

    /// <summary>
    /// Parses swagger_yard syntax from models folders.
    /// </summary>
    /// <param name="_models"></param>
    /// <param name="modelFiles"></param>
    private static void ParseModelDocs(List<Model> _models, string[] modelFiles)
    {
        foreach (string modelFilePath in modelFiles)
        {
            //Open model file and use StreamReader to parse swagger syntax
            using (StreamReader srModelReader = new StreamReader(modelFilePath))
            {
                while (!srModelReader.EndOfStream)
                {
                    //Read line, looking for swagger_yard syntax - ##
                    var line = srModelReader.ReadLine();

                    //Line is not null and contains the key indicating swagger_yard block potentially follows
                    if (line != null && line.Contains("##"))
                    {
                        //Found the first key
                        var nextLine = srModelReader.ReadLine();

                        //Is it a model or just a comment?
                        if (nextLine != null && MODEL_KEYWORDS.Any(nextLine.Contains))
                        {
                            //This is a model. Continue with parsing.
                            //Store the model name.
                            //Split the string after @!model or @model
                            string modelNameLine = nextLine.Split("model").Last().Trim();
                            Model model = new Model
                            {
                                ModelName = modelNameLine
                            };

                            //Pull everything up until a line without #, load into a list of model docs lines.
                            List<string> modelLines = new List<string>();
                            string? currentLine = srModelReader.ReadLine().Trim();

                            while (!string.IsNullOrWhiteSpace(currentLine) && currentLine[0] == '#' && !srModelReader.EndOfStream)
                            {
                                //Warn if model contains an @parameter tag, which is a common mistake.
                                if (currentLine.Contains("@parameter"))
                                {
                                    Console.WriteLine(string.Format("Warning: {0} contains @parameter tag in line {1}.", model.ModelName, currentLine));
                                }

                                modelLines.Add(currentLine);

                                if (!srModelReader.EndOfStream)
                                {
                                    currentLine = srModelReader.ReadLine().Trim();
                                }
                            }

                            //There are available properties.
                            //Iterate over the lines in the model docs.
                            for (int i = 0; i < modelLines.Count(); i++)
                            {
                                string propertyLine = modelLines[i];

                                //Is it a property line?
                                if (propertyLine.Contains("@property"))
                                {
                                    //Property: peel off the property tag and save the property name to property object.
                                    string propertyName =
                                        propertyLine.Split("property")
                                        .Last()
                                        .Trim()
                                        .Split(new char[] { '(', '[' })
                                        .First()
                                        .Trim();
                                    Property property = new Property
                                    {
                                        Name = propertyName
                                    };

                                    //Is the property nullable or required? Save that if present.
                                    if (propertyLine.Contains("(required)"))
                                    {
                                        property.IsRequired = true;
                                    }

                                    if (propertyLine.Contains("(nullable)"))
                                    {
                                        property.IsNullable = true;
                                    }

                                    //Get the data type and save the data type to property.DataType
                                    string datatype =
                                        propertyLine.Split('[')
                                            .Last()
                                            .Trim()
                                            .Split(']')
                                            .First()
                                            .Trim();
                                    property.DataType = datatype;

                                    //Parse multiline descriptions.
                                    i = ParseModelProperty(modelLines, i, propertyLine, property);

                                    //Look ahead to see if there is an example
                                    if (modelLines.Count > i + 1 && modelLines[i + 1].Contains("@example"))
                                    {
                                        //There is an example line. Peel off the example tag and advance the stream.
                                        //Trim whitespace and save the example text.
                                        i += 2;
                                        string exampleText =
                                            modelLines[i].Split("#")
                                                .Last()
                                                .Trim('"')
                                                .Trim();
                                        //Add property to model.
                                        property.Example = exampleText;
                                    }

                                    //Add property to model.
                                    model.Properties.Add(property);
                                }
                            }

                            //Add the current model to the list.
                            _models.Add(model);
                        }
                    }
                }
            }
        }
    }
}