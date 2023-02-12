using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OdysseyAPI.Data.Configurations;

public sealed class ContactConfiguration
{
  public void Configure(EntityTypeBuilder<Contact> builder)
  {
    builder.ToTable("tb_Contacts");

    builder.HasKey(e => new { e.Id });
    builder.Property(t => t.Id)
        .UseIdentityColumn();

    builder.Property(t => t.Name)
        .HasMaxLength(50)
        .IsRequired();

    builder.Property(t => t.Number)
        .HasMaxLength(20)
        .IsRequired();

    builder.Property(t => t.Email)
        .HasMaxLength(100);

    builder.Property(t => t.AvatarUrl)
        .HasMaxLength(1024);
  }
}
