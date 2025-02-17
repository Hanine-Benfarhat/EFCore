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

    }
}
