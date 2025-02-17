using EFCore.First.Contract;
using EFCore.First.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCore.First.API.Controllers;

//[ApiController] and[Route] Attributes: These attributes specify that this class is an API controller
//and define the base route for all actions in the controller.
[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    //instancier le dbcontext
    private readonly HRContext dbcontext; //READ ONLY POUR 
    public EmployeesController(HRContext context)
    {
        dbcontext = context;
    }

    [HttpGet]
    public IActionResult GetEmployees()   //IActionResult un type flexible qui permet de renvoyer différents types de réponses
    {
        try
        {
            return Ok(dbcontext.Employees.ToList());
        }
        catch (DbUpdateException dbEx)
        {
            return StatusCode(500, "Database error occurred while creating the employee.");
        }
    }
    //DONE
    [HttpGet("{id}")]
    public IActionResult GetEmployee(int id) 
    {
        try
        {
            var employee = dbcontext.Employees.Find(id);
            return employee is null ? NotFound() : Ok(employee);
        }
        catch (DbUpdateException dbEx)
        {
            return StatusCode(500, "Database error occurred while creating the employee.");
        }
    }
    //DONE
    [HttpPost]
    public IActionResult CreateEmployee([FromBody] EmployeeDTO employeeDTO) // from body : l'employee dans le corps de la requete
    {
        try
        {
            if (employeeDTO is null)
                return BadRequest("Veuillez entrer les données de l'employé.");

            if (!ModelState.IsValid) // Automatically checks for validation
            {
                return BadRequest(ModelState); // Returns detailed validation error messages
            }
            var employee = new Employee
            {
                Name = employeeDTO.Name,
                Age = employeeDTO.Age,
                Email = employeeDTO.Email,
                DepartementID = employeeDTO.DepartementID,
            };
            dbcontext.Employees.Add(employee);
            dbcontext.SaveChanges();
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee); //pas compris
        }
        catch (DbUpdateException dbEx)
        {
            return StatusCode(500, "Database error occurred while creating the employee.");
        }
       
    }
    //DONE

    [HttpPut("{id}")]
    public  IActionResult UpdateEmployee(int id, [FromBody] EmployeeDTO updatedEmployee)
    {
        try
        {
            //verifier si l'employé existe?
            var employee = dbcontext.Employees.Find(id);
            if (employee is null)
                return NotFound();
            if (!ModelState.IsValid) // Automatically checks for validation
            {
                return BadRequest(ModelState); // Returns detailed validation error messages
            }
            employee.Name = updatedEmployee.Name;
            employee.Age = updatedEmployee.Age;
            employee.Email = updatedEmployee.Email;
            dbcontext.SaveChanges();
            return NoContent();
        }
        catch (DbUpdateException dbEx)
        {
            return StatusCode(500, "Database error occurred while creating the employee.");
        }
    }
    //DONE

    [HttpDelete("{id}")]
    public IActionResult DeleteEmployee(int id)
    {
        try
        {
            //using var dbcontext = new HRContext();
            var employee = dbcontext.Employees.Find(id);
            if (employee is null)
                return BadRequest("Employé n'exsite pas ");

            dbcontext.Employees.Remove(employee);
            dbcontext.SaveChanges();
            return NoContent();
        }
        catch (DbUpdateException dbEx)
        {
            return StatusCode(500, "Database error occurred while creating the employee.");
        }
    }
    //DONE
}
