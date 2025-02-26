using System.ComponentModel.DataAnnotations;
namespace EFCore.First.Contract;

public class EmployeeDTO
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Age is required")]
    [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
    public int Age { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }= string.Empty;

    [Required(ErrorMessage = "Department ID is required")]
    public int DepartementID { get; set; }
}