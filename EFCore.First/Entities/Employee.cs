using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.First.Entities;
public class Employee
{
    [Key] //la clé primaire
    public int Id { get; set; }

    [MaxLength(150)] //contrainte sur la longueur de la chaine (Varchar(150))
    public string Name { get; set; } = string.Empty;

    [Range(18,65)] //contrainte sur l'age
    public int Age { get; set; }

    public string Email { get; set; } = string.Empty;   //contrainte OnModelCreating

    [ForeignKey("Departement")]
    public int DepartementID { get; set; }
    
    public Departement Departement { get; set; }


}