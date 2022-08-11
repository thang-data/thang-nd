using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mongo.Models;
using mongo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mongo.Controllers
{
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
    
}
