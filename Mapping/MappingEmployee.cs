using EFCore.First.Entities;

namespace Mapping;

public static class MappingEmployee
{
    public static Employee ToEntity( this EmployeeDTO employeeDTO)
    {
        return new Employee
        {
            Name = employeeDTO.Name,
            Age = employeeDTO.Age,
            Email = employeeDTO.Email,
            DepartementID = employeeDTO.DepartementID,
        };
    }

    public static EmployeeDTO ToDto(this Employee employee)
    {
        return new EmployeeDTO
        {
            Name = employee.Name,
            Age = employee.Age,
            Email = employee.Email,
            DepartementID = employee.DepartementID,
        };
    }

    public static Employee UpdateFromDTO(this Employee employee, EmployeeDTO employeeDTO)
    {
        employee.Name = employeeDTO.Name;
        employee.Email = employeeDTO.Email;
        employee.Age = employeeDTO.Age;
        employee.DepartementID = employeeDTO.DepartementID;
        return employee;
    }
}
