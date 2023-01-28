using Api.Blog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Blog.Data.Mappings;

public class PostMap : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        // tabela
        builder.ToTable("Post");

        // chave primaria
        builder.HasKey(x => x.Id);

        // identity
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();

        // propriedades
        builder.Property(x => x.Title)
            .IsRequired()
            .HasColumnType("NVARCHAR")
            .HasMaxLength(100);

        builder.Property(x => x.Summary)
            .IsRequired()
            .HasColumnType("NVARCHAR")
            .HasMaxLength(200);

        builder.Property(x => x.Body)
            .IsRequired()
            .HasColumnType("NVARCHAR")
            .HasMaxLength(300);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasColumnType("NVARCHAR")
            .HasMaxLength(80);

        builder.Property(x => x.CreateDate)
            .IsRequired()
            .HasColumnType("SMALLDATETIME")
            .HasMaxLength(60)
            .HasDefaultValueSql("getdate()");

        builder.Property(x => x.LastUpdateDate)
            .IsRequired()
            .HasColumnType("SMALLDATETIME")
            .HasMaxLength(60)
            .HasDefaultValueSql("getdate()");

        // relacionamentos
        builder.HasOne(x => x.Author)
            .WithMany(x => x.Posts)
            .HasForeignKey(x => x.AuthorId)
            .HasConstraintName("FK_Post_Author")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Posts)
            .HasForeignKey(x => x.CategoryId)
            .HasConstraintName("FK_Post_Category")
            .OnDelete(DeleteBehavior.Cascade);

        // indice
        builder.HasIndex(x => x.Slug, "IX_Post_Slug")
            .IsUnique();

        // relacionamento muitos-para-muitos
        builder.HasMany(x => x.Tags)
            .WithMany(x => x.Posts)
            .UsingEntity<Dictionary<string, object>>(
                "PostTag",
                post => post
                .HasOne<Tag>()
                .WithMany()
                .HasForeignKey("PostId")
                .HasConstraintName("FK_PostTag_PostId")
                .OnDelete(DeleteBehavior.Cascade),
                tag => tag
                .HasOne<Post>()
                .WithMany()
                .HasForeignKey("TagId")
                .HasConstraintName("FK_PostTag_TagId")
                .OnDelete(DeleteBehavior.Cascade)
            );

    }
}
