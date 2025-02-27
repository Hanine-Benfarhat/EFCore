namespace EFCore.First.API.Controllers;

//[ApiController] and[Route] Attributes: These attributes specify that this class is an API controller
//and define the base route for all actions in the controller.
[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly HRContext _dbcontext; 
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
            _logger.LogInformation("fetching all employee");
            return Ok(_employeeService.GetAll());
        }
        catch (ArgumentNullException)
        {
            _logger.LogWarning("Datbase con text was null!");
            return StatusCode(400, "Service unavailable: Data problem");
        }
        catch (Exception)
        {
            _logger.LogError( "Error fetching employees");
            return StatusCode(500, "Internal server error, contact the admin");
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetEmployee(int id) 
    {
        try
        {
            var employee = _employeeService.Get(id);
            return employee is null ? NotFound($"Employee with ID {id} not found.") : Ok(employee);
        }
        catch (DbUpdateException )
        {
            _logger.LogError( "Error fetching employee with ID {Id}", id);
            return StatusCode(500, "Database error occurred while creating the employee.");
        }
    }

    [HttpPost]
    public IActionResult CreateEmployee([FromBody] EmployeeDTO employeeDTO) // from body : l'employee dans le corps de la requete
    {
        try
        {
            
            if (!ModelState.IsValid) // Automatically checks for validation
            {
                _logger.LogWarning("Invalid employee data ");
                return BadRequest(ModelState); // Returns detailed validation error messages
            }
            if(employeeDTO == null)
            {
                _logger.LogWarning("Received a null employeeDTO");
                return BadRequest(ModelState);
            }
            var employee = _employeeService.Create(employeeDTO);
            _logger.LogInformation("Employee created with ID {Id}", employee.Id);
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee); 
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

    [HttpPut("{id}")]
    public  IActionResult UpdateEmployee(int id, [FromBody] EmployeeDTO updatedEmployee)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid employee data ");
                return BadRequest(ModelState);
            }
            //verifier si l'employé existe?
            var updated = _employeeService.UpdateEmployee(id, updatedEmployee);
            if (!updated)
                throw new ArgumentNullException(nameof(updatedEmployee)); //si updatedEmployee is null 
            return NoContent();
        }

        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "Updated employee object was null.");
            
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error while updating the employee in the database.");
            return StatusCode(500, "Database update error. Please contact the administrator.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while updating the employee.");
            return StatusCode(500, "Internal server error. Please contact the administrator.");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteEmployee(int id)
    {
        try
        {
            bool deleted = _employeeService.DeleteEmployee(id);
            if (!deleted)
            {
                _logger.LogWarning($"Employee with ID {id} not found for deletion");
                return NotFound("L'employé n'existe pas.");
            }
            _logger.LogInformation($"Employee with ID {id} deleted successfully");
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while deleting employee with ID {id} .");
            return StatusCode(500, "Internal server error. Please contact the administrator.");
        }
    }
    
}
