using mongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mongo.Services
{
    public interface IEmployeeService
    {
        List<Employee> Get();
       
        Employee Get(string id);
        Employee Create(Employee employee);
        void Delete(string id);
        void Update(string id, Employee employee);
    }
}
