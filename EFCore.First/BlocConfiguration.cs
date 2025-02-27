namespace EFCore.First;

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
