using Bogus;
using OdysseyAPI.Data;
using OdysseyAPI.Data.Domain;

namespace OdysseyAPI.Tests.Integration.Extensions;

public static class DataExtensions
{
  public static async Task CreateContacts(this OdysseyAPIFactory factory, int rows)
  {
    var testContacts = new Faker<Contact>()
      .RuleFor(_ => _.Name, o => o.Name.FullName())
      .RuleFor(_ => _.Number, o => o.Phone.PhoneNumber("### ### ####"));

    var data = testContacts.Generate(rows);
    using (var scope = factory.Services.CreateScope())
    {
      var scopedServices = scope.ServiceProvider;
      var dbContext = scopedServices.GetRequiredService<PhonebookDbContext>();

      await dbContext.Contacts.AddRangeAsync(data);
      await dbContext.SaveChangesAsync();
    }
    
  }
}
