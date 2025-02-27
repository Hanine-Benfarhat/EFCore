namespace EFCore.First.Entities;
public class Departement
{
    [Key]
    public int DepartementID { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Code { get; set; } = string.Empty ;

    public List<Employee>? Employees { get; set; } //relation one to many
}
