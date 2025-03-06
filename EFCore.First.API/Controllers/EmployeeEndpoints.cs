using EFCore.First.Contract;
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

    public static IResult GetEmployees(
        IEmployeeService employeeService,
        Serilog.ILogger logger)
    {
        try
        {
            logger.Information("Fetching all employees");
            var employees = employeeService.GetAll(); // Assuming GetAllAsync is an asynchronous method
            return Results.Ok(employees);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error fetching employees");
            return Results.Problem("An error occurred while fetching employees.", statusCode: 500);
        }
    }



    public static IResult GetEmployee(
        int id,
        Serilog.ILogger logger,
        IEmployeeService employeeService
        )
    {
        try
        {
            var employee = employeeService.Get(id);
            return employee is null ? Results.NotFound($"Employee with ID {id} not found.") : Results.Ok(employee);
        }
        catch (DbUpdateException)
        {
            logger.Error("Error fetching employee with ID {Id}", id);
            return Results.Problem("An error occurred while fetching employees.", statusCode: 500);
        }
    }

    public static IResult CreateEmployee(
        [FromBody] EmployeeDTO employeeDTO,
        IEmployeeService employeeService,
        Serilog.ILogger logger)
    {
        try
        {
            if (employeeDTO == null)
            {
                logger.Warning("Received a null employeeDTO");
                return Results.BadRequest();
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
                return Results.BadRequest(new { Errors = validationErrors });
            }
            var employee = employeeService.Create(employeeDTO);
            logger.Information("Employee created with ID {Id}", employee.Id);
            return Results.Created($"/employees/{employee.Id}", employee);

        }
        catch (DbUpdateConcurrencyException)
        {
            logger.Warning("Database concurrency exception");
            return Results.Problem("An error occurred while fetching employees.", statusCode: 500);
        }
        catch (DbUpdateException)
        {
            logger.Warning("Database exception");
            return Results.Problem("An error occurred while fetching employees.", statusCode: 500);
        }


    }

    public static IResult UpdateEmployee(
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
                return Results.BadRequest(new { Errors = validationErrors });
            }
            //verifier si l'employé existe?
            var updated = employeeService.UpdateEmployee(id, updatedEmployee);
            if (!updated)
                throw new ArgumentNullException(nameof(updatedEmployee)); //si updatedEmployee is null 
            return Results.NoContent();
        }

        catch (ArgumentNullException)
        {
            logger.Error("Updated employee object was null.");
            return Results.Problem("An error occurred while fetching employees.", statusCode: 500);
        }
        catch (DbUpdateException)
        {
            logger.Error("Error while updating the employee in the database.");
            return Results.Problem("An error occurred while fetching employees.", statusCode: 500);
        }
        catch (Exception)
        {
            logger.Error("An unexpected error occurred while updating the employee.");
            return Results.Problem("An error occurred while fetching employees.", statusCode: 500);
        }
    }

    public static IResult DeleteEmployee(
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
                return Results.NotFound("L'employé n'existe pas.");
            }
            logger.Information("Employee with ID {id} deleted successfully", id);
            return Results.NoContent();
        }
        catch (Exception)
        {
            logger.Error("An error occurred while deleting employee with ID {id} .", id);
            return Results.Problem("An error occurred while fetching employees.", statusCode: 500);
        }
    }

}
