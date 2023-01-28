using Api.Blog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Blog.Data.Mappings;

public class RoleMap : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // tabela
        builder.ToTable("Role");

        // chave primaria
        builder.HasKey(x => x.Id);

        // identity
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();

        // propriedades
        builder.Property(x => x.Name)
            .IsRequired()
            .HasColumnType("NVARCHAR")
            .HasMaxLength(80);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasColumnType("NVARCHAR")
            .HasMaxLength(80);

        // indice
        builder.HasIndex(x => x.Slug, "IX_Post_Slug")
            .IsUnique();

        // relacionamentos
        builder
            .HasMany(x => x.Users)
            .WithMany(x => x.Roles)
            .UsingEntity<Dictionary<string, object>>(
                "UserRole",
                user => user
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .HasConstraintName("FK_UserRole_UserId")
                    .OnDelete(DeleteBehavior.Cascade),
                 role => role
                    .HasOne<Role>()
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .HasConstraintName("FK_UserRole_RoleId")
                    .OnDelete(DeleteBehavior.Cascade));
    }
}
