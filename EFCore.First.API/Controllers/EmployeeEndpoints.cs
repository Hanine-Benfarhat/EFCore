using EFCore.First.Contract;
using EFCore.First.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Query;
namespace EFCore.First.API.Controllers;


public class EmployeesEndpoints : CarterModule
{
    public EmployeesEndpoints()
        :base("api/employees")
    {

    }
    public override void AddRoutes( IEndpointRouteBuilder app)
    {
        app.MapGet("", GetEmployees);
        app.MapGet("/{id}", GetEmployee);
        app.MapPost("", CreateEmployee);
        app.MapDelete("", DeleteEmployee);
    }
    //private readonly HRContext _dbcontext; 
    //private readonly ILogger<EmployeesController> _logger;

    [ProducesResponseType(typeof(List<Employee>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public static Results<Ok<List<Employee>>, ProblemHttpResult> GetEmployees(
        IEmployeeService employeeService,
        Serilog.ILogger logger)
    {
        try
        {
            logger.Information("Fetching all employees");
            var employees = employeeService.GetAll(); // Assuming GetAllAsync is an asynchronous method
            return TypedResults.Ok(employees);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error fetching employees");
            return TypedResults.Problem(
                detail: "An error occurred while fetching employees.",
            statusCode: 500);
        }
    }


    [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public static Results<Ok<Employee>, NotFound<string> , ProblemHttpResult> GetEmployee(
        int id,
        Serilog.ILogger logger,
        IEmployeeService employeeService
        )
    {
        try
        {
            var employee = employeeService.Get(id);
            return employee is null 
                ? TypedResults.NotFound($"Employee with ID {id} not found.") 
                : TypedResults.Ok(employee);
        }
        catch (DbUpdateException)
        {
            logger.Error("Error fetching employee with ID {Id}", id);
            return TypedResults.Problem("An error occurred while fetching employees.",
                statusCode: 500);
        }
    }

    public static Results<BadRequest<string>, Created<Employee>, ProblemHttpResult> CreateEmployee(
        [FromBody] EmployeeDTO employeeDTO,
        IEmployeeService employeeService,
        Serilog.ILogger logger)
    {
        try
        {
            if (employeeDTO == null)
            {
                logger.Warning("Received a null employeeDTO");
                return TypedResults.BadRequest( "Employee data cannot be null." );
            }
            //manual validation of modelstate
            var validationErrors = new List<string>();
            if (string.IsNullOrWhiteSpace(employeeDTO.Name))
            {
                validationErrors.Add("Name is required.");
            }

            if (employeeDTO.Age <= 18 && employeeDTO.Age>100)
            {
                validationErrors.Add("Invalid age.");
            }
            if (validationErrors.Any())
            {
                logger.Warning("Invalid employee data: {Errors}", string.Join(", ", validationErrors));
                return TypedResults.BadRequest(string.Join(", ", validationErrors));
            }
            var employee = employeeService.Create(employeeDTO);
            logger.Information("Employee created with ID {Id}", employee.Id);
            return TypedResults.Created($"/employees/{employee.Id}", employee);

        }
        catch (DbUpdateConcurrencyException)
        {
            logger.Warning("Database concurrency exception");
            return TypedResults.Problem("An error occurred while fetching employees.", statusCode: 500);
        }
        catch (DbUpdateException)
        {
            logger.Warning("Database exception");
            return TypedResults.Problem("An error occurred while fetching employees.", statusCode: 500);
        }


    }

    public static Results<BadRequest<string>, Created<Employee>, ProblemHttpResult , NoContent> UpdateEmployee(
        int id,
        [FromBody] EmployeeDTO updatedEmployee,
        Serilog.ILogger logger,
        IEmployeeService employeeService)
    {
        try
        {
            var validationErrors = new List<string>();
            if (string.IsNullOrWhiteSpace(updatedEmployee.Name))
            {
                validationErrors.Add("Name is required.");
            }

            if (updatedEmployee.Age <= 18 && updatedEmployee.Age > 100)
            {
                validationErrors.Add("Invalid age.");
            }
            if (validationErrors.Any())
            {
                logger.Warning("Invalid employee data: {Errors}", string.Join(", ", validationErrors));
                return TypedResults.BadRequest(string.Join(", ", validationErrors));
            }
            //verifier si l'employé existe?
            var updated = employeeService.UpdateEmployee(id, updatedEmployee);
            if (!updated)
                throw new ArgumentNullException(nameof(updatedEmployee)); //si updatedEmployee is null 
            return TypedResults.NoContent();
        }

        catch (ArgumentNullException)
        {
            logger.Error("Updated employee object was null.");
            return TypedResults.Problem("An error occurred while fetching employees.", statusCode: 500);
        }
        catch (DbUpdateException)
        {
            logger.Error("Error while updating the employee in the database.");
            return TypedResults.Problem("An error occurred while fetching employees.", statusCode: 500);
        }
        catch (Exception)
        {
            logger.Error("An unexpected error occurred while updating the employee.");
            return TypedResults.Problem("An error occurred while fetching employees.", statusCode: 500);
        }
    }

    public static Results<NotFound<string>, NoContent, ProblemHttpResult> DeleteEmployee(
        int id,
        Serilog.ILogger logger,
        IEmployeeService employeeService)
    {
        try
        {
            bool deleted = employeeService.DeleteEmployee(id);
            if (!deleted)
            {
                logger.Warning("Employee with ID {id} not found for deletion", id);
                return TypedResults.NotFound("L'employé n'existe pas.");
            }
            logger.Information("Employee with ID {id} deleted successfully", id);
            return TypedResults.NoContent();
        }
        catch (Exception)
        {
            logger.Error("An error occurred while deleting employee with ID {id} .", id);
            return TypedResults.Problem("An error occurred while fetching employees.", statusCode: 500);
        }
    }

}
