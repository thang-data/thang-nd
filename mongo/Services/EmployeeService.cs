using mongo.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mongo.Services
{
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
    
}
