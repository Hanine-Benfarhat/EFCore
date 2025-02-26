using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCore.First.Entities;
using EFCore.First.Contract;
using static EFCore.First.Services.EmployeeService;

namespace EFCore.First.Services;

public interface IEmployeeService
{
    List<Employee> GetAll();
    Employee? Get(int id);
    Employee Create(EmployeeDTO employeeDTO);
    bool UpdateEmployee (int id , EmployeeDTO employeeDTO);
    bool DeleteEmployee(int id);
}
