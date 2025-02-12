using EFCore.First;
using EFCore.First.Entities;
using Microsoft.EntityFrameworkCore;

public class HRContext : DbContext
{

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Departement> Departements { get; set; }   
    public DbSet<Bloc> Blocs { get; set; }

    //
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=THE_HA9;Database=HRDatabase;Integrated Security=True;TrustServerCertificate=True;");
    }

    //Fluent API
    //OnModelCreating une méthode qui configure les relations entre les entités dans EF core
    //les contraintes d'intégrité et le comportement des suppressions
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Departement) //un employé appartient a un seul departement
            .WithMany(d => d.Employees) //un departement contient plusieur employés // relation 1 to many
            .HasForeignKey(e => e.DepartementID) // la clé etrangere dans la table employe est DepartementId
            .OnDelete(DeleteBehavior.Restrict); // Empêche la suppression en cascade
        
        modelBuilder.Entity<Departement>()
            .HasKey(e => e.DepartementID);

        modelBuilder.Entity<Departement>()
            .HasIndex("Code");

        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique();

        modelBuilder.Entity<Departement>()
            .HasIndex(d => d.Name)
            .HasDatabaseName("IX_Department_Name")  // Nom de l'index (optionnel)
            .IsUnique();  // les valeurs dans cette colonne uniques

        modelBuilder.ApplyConfiguration(new BlocConfiguration());

    }
}