namespace EFCore.First.Services;

public interface IEmployeeService
{
    List<Employee> GetAll();
    Employee? Get(int id);
    Employee? Create(EmployeeDTO employeeDTO);
    bool UpdateEmployee (int id , EmployeeDTO employeeDTO);
    bool DeleteEmployee(int id);
}
