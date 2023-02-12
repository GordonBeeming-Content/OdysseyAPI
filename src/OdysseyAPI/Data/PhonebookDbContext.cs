namespace OdysseyAPI.Data;

#pragma warning disable CS8618
public sealed class PhonebookDbContext : DbContext
{
  public PhonebookDbContext(DbContextOptions options) : base(options)
  {
  }

  public DbSet<Contact> Contacts { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.ApplyConfigurationsFromAssembly(typeof(PhonebookDbContext).Assembly);
  }
}
