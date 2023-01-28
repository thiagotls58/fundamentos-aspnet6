using Api.Blog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Blog.Data.Mappings;

public class TagMap : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        // tabela
        builder.ToTable("Tag");

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
        builder.HasIndex(x => x.Slug, "IX_Tag_Slug")
            .IsUnique();

        // relacionamento muitos-para-muitos
        builder.HasMany(x => x.Posts)
            .WithMany(x => x.Tags)
            .UsingEntity<Dictionary<string, object>>(
                "PostTag",
                tag => tag
                .HasOne<Post>()
                .WithMany()
                .HasForeignKey("TagId")
                .HasConstraintName("FK_PostTag_TagId")
                .OnDelete(DeleteBehavior.Cascade),
                post => post
                .HasOne<Tag>()
                .WithMany()
                .HasForeignKey("PostId")
                .HasConstraintName("FK_PostTag_PostId")
                .OnDelete(DeleteBehavior.Cascade)
            );
    }
}
