using EFCore.First.Contract;
using EFCore.First.Entities;
using EFCore.First.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static EFCore.First.Services.EmployeeService;

namespace EFCore.First.API.Controllers;

//[ApiController] and[Route] Attributes: These attributes specify that this class is an API controller
//and define the base route for all actions in the controller.
[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly HRContext _dbcontext; //READ ONLY POUR 
    private readonly ILogger<EmployeesController> _logger;
    private readonly IEmployeeService _employeeService;
    public EmployeesController(HRContext context, ILogger<EmployeesController> logger, IEmployeeService employeeService)
    {
        _dbcontext = context;
        _logger = logger;
        _employeeService = employeeService;
    }

    [HttpGet]
    public IActionResult GetEmployees()   //IActionResult un type flexible qui permet de renvoyer différents types de réponses
    {
        try
        {
            return Ok(_employeeService.GetAll());
        }
        catch (ArgumentNullException)
        {
            _logger.LogWarning("Datbase con text was null!");
            return StatusCode(400, "Service unavailable: Data problem");
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error, contact the admin");
        }
    }

    //DONE
    [HttpGet("{id}")]
    public IActionResult GetEmployee(int id) 
    {
        try
        {
            var employee = _employeeService.Get(id);
            return employee is null ? NotFound() : Ok(employee);
        }
        catch (DbUpdateException )
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
            
            if (!ModelState.IsValid) // Automatically checks for validation
            {
                return BadRequest(ModelState); // Returns detailed validation error messages
            }
            if(employeeDTO == null)
            {
                return BadRequest(ModelState);
            }
            var employee = _employeeService.Create(employeeDTO);
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee); //pas compris
        }
        catch (DbUpdateConcurrencyException)
        {
            _logger.LogWarning("Database concurrency exception");
            return StatusCode(500, "Internal server error, try again");
        }
        catch (DbUpdateException )
        {
            return StatusCode(500, "Internal server error, contact admin.");
        }
        
       
    }
    //DONE

    [HttpPut("{id}")]
    public  IActionResult UpdateEmployee(int id, [FromBody] EmployeeDTO updatedEmployee)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //verifier si l'employé existe?
            var updated = _employeeService.UpdateEmployee(id, updatedEmployee);
            if (!updated)
                throw new ArgumentNullException(nameof(updatedEmployee)); //si updatedEmployee is null 
            return NoContent();
        }

        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Updated employee object was null.");
            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error while updating the employee in the database.");
            throw new Exception("Database update error occurred.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while updating the employee.");
            throw;
        }
    }
    //DONE

    [HttpDelete("{id}")]
    public IActionResult DeleteEmployee(int id)
    {
        try
        {
            bool deleted = _employeeService.DeleteEmployee(id);
            if (!deleted)
                return NotFound("L'employé n'existe pas.");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting employee with ID {Id}.", id);
            return StatusCode(500, "Internal server error. Please contact the administrator.");
        }
    }
    //DONE
}
