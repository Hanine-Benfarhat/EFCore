using System.Diagnostics.Contracts;
using System.Web.Http.ModelBinding;
using EFCore.First.Entities;
using Microsoft.EntityFrameworkCore;
using EFCore.First.Contract;
using EmployeeDTO = EFCore.First.Contract.EmployeeDTO;
using Microsoft.Extensions.Logging;
namespace EFCore.First.Services;

public class EmployeeService : IEmployeeService
{
    private readonly HRContext _dbcontext;

    public EmployeeService(HRContext dbcontext)
    {
        _dbcontext = dbcontext;
        
    }

    public List<Employee> GetAll()
    {
        return _dbcontext.Employees.ToList();
       
    }
    public Employee? Get(int id)
    {
        var employee = _dbcontext.Employees.Find(id);
        return employee;
    }

    public Employee Create(EmployeeDTO employeeDTO)
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

    public bool UpdateEmployee(int id, EmployeeDTO employeeDTO)
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
        catch (DbUpdateException ex)
        { 
            throw new Exception("Database update error occurred.");
        }
        catch (Exception ex)
        { 
            throw;
        }
    }


    }
        
