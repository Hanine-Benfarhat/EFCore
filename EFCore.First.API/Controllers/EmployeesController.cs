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
    [HttpGet]
    public IActionResult GetProducts()   //IActionResult un type flexible qui permet de renvoyer différents types de réponses
    {
        using var dbcontext = new HRContext();

        return Ok( dbcontext.Employees.ToList());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployee(int id) //La méthode est asynchrone (async) pour éviter de bloquer le programme en attendant la base de données
    {
        using var dbcontext = new HRContext();
        var employee = await dbcontext.Employees.FindAsync(id);

        return employee == null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDTO employeeDTO) // from body : l'employee dans le corps de la requete
    {
        using var dbcontext = new HRContext();
        if (employeeDTO == null)
            return BadRequest("Données invalides.");
        var employee = new Employee
        {
            Name = employeeDTO.Name,
            Age = employeeDTO.Age,
            Email = employeeDTO.Email,
            DepartementID = employeeDTO.DepartementID,
        };
        dbcontext.Employees.Add(employee);
        await dbcontext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee); //pas compris
        //return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeDTO updatedEmployee)
    {
        using var dbcontext = new HRContext();
        //verifier si l'employé existe?
        var employee = await dbcontext.Employees.FindAsync(id);
        if (employee == null)
            return NotFound();

        employee.Name = updatedEmployee.Name;
        employee.Age = updatedEmployee.Age;
        employee.Email = updatedEmployee.Email;
        await dbcontext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        using var dbcontext = new HRContext();
        var employee = await dbcontext.Employees.FindAsync(id);
        if (employee == null)
            return NotFound();

        dbcontext.Employees.Remove(employee);
        await dbcontext.SaveChangesAsync();
        return NoContent();
    }
}
