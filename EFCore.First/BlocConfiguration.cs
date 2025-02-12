namespace EFCore.First;

using EFCore.First.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BlocConfiguration : IEntityTypeConfiguration<Bloc>
{
    public void Configure(EntityTypeBuilder<Bloc> builder)
    {
        
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(100);

        //builder.Property(e => e.NombreEmployee)
        //       .HasCheckConstraint("CHK_NombreEmployee", "NombreEmployee >= 1 AND NombreEmployee <= 100");

    }
}
