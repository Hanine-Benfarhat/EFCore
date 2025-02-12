using EFCore.First.Entities;

var dbcontext = new HRContext();
var DepSales = new Departement
{
    Name = "Sales",
    Code = "S15" , 

};
var DepHR = new Departement
{
    Name = "HR",
    Code = "H1" ,
 };
var alex = new Employee
{
    Age = 20,
    Name = "Alex",
    DepartementID = 1,
};

var david = new Employee
{
    Age = 20,
    Name = "David",
    DepartementID = 2,
};
dbcontext.Departements.Add(DepHR);
dbcontext.Departements.Add(DepSales);

dbcontext.Employees.Add(alex);

dbcontext.Employees.Add(david);

dbcontext.SaveChanges();


List<Employee> allEmployees = dbcontext.Employees.ToList();

List<Employee> employeesListThatStartWithA = dbcontext.Employees.Where(e => e.Name.StartsWith("A")).ToList();

foreach (var employee in employeesListThatStartWithA)
{
    Console.WriteLine(employee.Name);
}