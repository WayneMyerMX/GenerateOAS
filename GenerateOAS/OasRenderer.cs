using System.Text;
using Newtonsoft.Json;

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

//Autogen stubs of routes and models
/*
Route:
[Route("endpoint")]
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