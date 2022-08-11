# ASP.NET 6 REST API Tutorial | MongoDB Database
![image](https://user-images.githubusercontent.com/87810853/184068578-ab827756-fb33-4129-893b-76491984b8f8.png)

> Building a RESTful API service using Visual Studio 2022, ASP.NET 6 and MongoDB database.
## Create new ASP.NET Core Web API project
I named the project `EmployeeManagement`. While creating the new ASP.NET Core Web API project, select the following options on `Additional information` page.
+ .NET 6.0 as the Target Framework
+ Configure for HTTPS - Check the checkbox
+ Use controllers - Check the checkbox
+ Enable OpenAPI support - Check the checkbox

![image](https://user-images.githubusercontent.com/87810853/184073668-2f98fc81-d74b-4058-ba89-eab501c983fa.png)
We discussed What is `Swagger and OpenAPI specification` in detail in Parts 29 and 30 of Azure tutorial. If you are new to these concepts please check out these 2 videos from the following course page.
## Install MongoDB.Driver nuget package
![image](https://user-images.githubusercontent.com/87810853/184073844-27024d93-bb4f-4d82-aa9c-3b03e1dbb122.png)
## Map MongoDB to C# objects
Add `Models` folder and place the following `Employee.cs` class file in it. When employee data is retrieved from Mongo, the JSON data is mapped to this `Employee` class in .NET and vice-versa.
```C#
[BsonIgnoreExtraElements]
    public class Employee
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = String.Empty;
        [BsonElement("Name")]
        public string Name { get; set; } = String.Empty;
        [BsonElement("Address")]
        public string Address { get; set; } = String.Empty;
        [BsonElement("Phone")]
        public string Phone { get; set; } = String.Empty;
        [BsonElement("Email")]
        public string Email { get; set; } = String.Empty;
    }
```
+ `[BsonId]` attribute specifies that this is the Id field or property. In this example, the property Id maps to _id field in Mongo document.
+ `[BsonRepresentation]` attribute automatically converts Mongo data type to a .Net data type and vice-versa. In this example, Mongo data type `ObjectId` is automatically converted to string datatype and vice-versa.
+ `Name` property is decorated with the `[BsonElement]` attribute. So this means, `Name` property corresponds to `name` field in Mongo document. So, `[BsonElement]` attribute specifies the field in the Mongo document the decorated property corresponds to.
+ The property names (`Name`, `Address`, `Phone`, `Email`) have the same name as the fields in the Mongo document (`Name`, `Address`, `Phone`, `Email`). However the casing is different. In C# the properties start with an uppercase letter whereas in Mongo the field starts with lowercase. There are several approaches to handle this case sensitive mapping. One of the easiest and cleanest approaches is to use [BsonElement] attribute.
+ What to do if the JSON document in MongoDB contains more fields than the properties in the corresponding C# class? Well, we can use `[BsonIgnoreExtraElements]` attribute and instruct the serializer to ignore the extra elements.
All the following attributes are present in `MongoDB.Bson.Serialization.Attributes`
+ BsonId
+ BsonElement
+ BsonRepresentation
+ BsonIgnoreExtraElements
## MongoDB connection string in ASP.NET Core
Store MongoDB connection information in `appsettings.json` file.
```C#
{
  "EmployeeDatabaseSetting": {
    "EmployeesCollectionName": "Employees",
    "ConnectionString": "mongodb+srv://<username>:<password>@cluster0.jscfz7b.mongodb.net/test",
    "DatabaseName": "EmployeeDB"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```
## Read MongoDB connection information

![image](https://user-images.githubusercontent.com/87810853/184075019-302a7825-cf25-4380-9e7b-aace54deab95.png)

Add the following  `IEmployeeStoreDatabaseSettings.cs` files in the Models folder. This interface and the class provide strongly typed access to MongoDB connection information.

### EmployeeDatabasesetting.cs
```C#
 public class EmployeeDatabasesetting : IEmployeeDatabaseSettings
    {
        public string EmployeesCollectionName { get; set; } = String.Empty;
        public string ConnectionString { get; set; } = String.Empty;
        public string DatabaseName { get; set; } = String.Empty;
    }
    public interface IEmployeeDatabaseSettings
    {
        public string EmployeesCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
```
## How to call MongoDB API from C#
![image](https://user-images.githubusercontent.com/87810853/184075801-bf5efdf0-ec96-4c63-a7e3-c26ec102cfef.png)
For separation of concerns we will keep the code that calls Mongo API in a separate service layer.
ASP.NET Web API Controller calls this service. 
Add `Services` folder and place the 2 files in it (`IEmployeeService.cs` and `EmployeeService.cs`)

### IEmployeeService.cs
```C#
public interface IEmployeeService
    {
        List<Employee> Get();
       
        Employee Get(string id);
        Employee Create(Employee employee);
        void Delete(string id);
        void Update(string id, Employee employee);
    }
```
### EmployeeService.cs
```C#
public class EmployeeService : IEmployeeService
    {
        private readonly IMongoCollection<Employee> _employees;
        public EmployeeService(IEmployeeDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _employees = database.GetCollection<Employee>(settings.EmployeesCollectionName);

        }

        public Employee Create(Employee employee)
        {
            _employees.InsertOne(employee);
            return employee;
        }
        public List<Employee> Get()
        {
            return _employees.Find(employee => true).ToList();
        }
        public Employee Get(string id)
        {
            return _employees.Find(employee => employee.Id == id).FirstOrDefault();
        }

        public void Delete(string id)
        {
            _employees.DeleteOne(employee => employee.Id == id);
        }

        public void Update(string id, Employee employee)
        {
            _employees.ReplaceOne(employee => employee.Id == id, employee);
        }
    }
```
## ASP.NET Core REST API Controller
Add the following `EmployeesController.cs` file in the Controllers folder.
```C#
[Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : Controller
    {
        private readonly EmployeeService emloyeesService;

        public EmployeesController(EmployeeService emloyeesService)
        {
            this.emloyeesService = emloyeesService;
        }
        // GET: api/<EmployeesController>
        [HttpGet]
        public ActionResult<List<Employee>> Get()
        {
            return emloyeesService.Get();
        }

        // GET api/<EmployeesController>/5
        [HttpGet("{id}")]
        public ActionResult<Employee> Get(string id)
        {
            var Employee = emloyeesService.Get(id);

            if (Employee == null)
            {
                return NotFound($"Employee with Id = {id} not found");
            }

            return Employee;
        }

        // POST api/<EmployeesController>
        [HttpPost]
        public ActionResult<Employee> Post([FromBody] Employee Employee)
        {

            emloyeesService.Create(Employee);

            return CreatedAtAction(nameof(Get), new { id = Employee.Id }, Employee);
        }

        // PUT api/<EmployeesController>/5
        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] Employee Employee)
        {
            var existingEmployee = emloyeesService.Get(id);

            if (existingEmployee == null)
            {
                return NotFound($"Employee with Id = {id} not found");
            }

            emloyeesService.Update(id, Employee);

            return NoContent();
        }

        // DELETE api/<EmployeesController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var Employee = emloyeesService.Get(id);

            if (Employee == null)
            {
                return NotFound($"Employee with Id = {id} not found");
            }

            emloyeesService.Delete(Employee.Id);

            return Ok($"Employee with Id = {id} deleted");
        }
    }
```
## Configure Services 
In `Startup.cs` file, configure the required services.
```C#
 public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.  
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<EmployeeDatabasesetting>(
                Configuration.GetSection(nameof(EmployeeDatabasesetting)));

            services.AddSingleton<IEmployeeDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<EmployeeDatabasesetting>>().Value);

            services.AddSingleton<EmployeeService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.  
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
```
## Test REST API
+ `GET`   `/api/Employees`
+ `GET`   `/api/Employees/{id}`
+ `POST`   `/api/Employees`
+ `PUT`   `/api/Employees/{id}`
+ `DELETE`   `/api/Employees/{id}`
