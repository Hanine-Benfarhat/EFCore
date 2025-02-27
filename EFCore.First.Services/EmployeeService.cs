using Microsoft.Extensions.Logging;

namespace EFCore.First.Services;
public class EmployeeService : IEmployeeService
{
    private readonly HRContext _dbcontext;
    private readonly ILogger<EmployeeService> _logger;


    public EmployeeService(HRContext dbcontext , ILogger<EmployeeService> logger)
    {
        _dbcontext = dbcontext;
        _logger = logger;

    }

    public List<Employee> GetAll()
    {
        try
        {
            return _dbcontext.Employees.ToList();
        }
        catch (ArgumentNullException)
        {
            Console.WriteLine("An error occurred while retrieving employees.");
            return new List<Employee>();
        }

    }
    public Employee? Get(int id)
    {
        try
        {
            var employee = _dbcontext.Employees.Find(id);
            return employee;
        }
        catch (Exception)
        {

            Console.WriteLine("An error occurred while retrieving the employee.");
            return null;
        }
    }

    public Employee? Create(EmployeeDTO employeeDTO)
    {
        try
        {
            var employee = new Employee
            {
                Name = employeeDTO.Name,
                Age = employeeDTO.Age,
                Email = employeeDTO.Email,
                DepartementID = employeeDTO.DepartementID,
            };
            _dbcontext.Employees.Add(employee);
            _dbcontext.SaveChanges();
            return employee;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new Exception("Database update error occurred.");
        }
        catch (DbUpdateException)
        {
            throw new Exception("Database update error occurred.");
        }
        catch (Exception)
        {

            Console.WriteLine("An error occurred while creating employee.");
            return null;
        }
    }

    public bool UpdateEmployee(int id, EmployeeDTO employeeDTO)
    {
        try
        {
            var employee = _dbcontext.Employees.Find(id);
            if (employee == null)
            {
                return false;
            }
            employee.Name = employeeDTO.Name;
            employee.Age = employeeDTO.Age;
            employee.Email = employeeDTO.Email;
            _dbcontext.SaveChanges();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new Exception("Database update error occurred.");
        }
        catch (DbUpdateException)
        {
            throw new Exception("Database update error occurred.");
        }
        catch (Exception)
        {

            Console.WriteLine("An error occurred while updating employees.");
            return false;
        }
    }

    public bool DeleteEmployee(int id)
    {
        try
        {
            var employee = _dbcontext.Employees.Find(id);
            if (employee is null)
                return false;

            _dbcontext.Employees.Remove(employee);
            _dbcontext.SaveChanges();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new Exception("Database update error occurred.");
        }
        catch (DbUpdateException)
        { 
            throw new Exception("Database update error occurred.");
        }
        
    }


    }
        
