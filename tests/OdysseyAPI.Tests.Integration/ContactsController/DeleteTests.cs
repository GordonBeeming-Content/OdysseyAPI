using Bogus;
using Bogus.DataSets;
using Docker.DotNet.Models;
using Microsoft.EntityFrameworkCore;
using OdysseyAPI.Data;
using OdysseyAPI.Data.Domain;
using OdysseyAPI.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OdysseyAPI.Tests.Integration.ContactsController;

[Collection(OdysseyAPICollection.Definition)]
public sealed class DeleteTests
{
  private readonly HttpClient _httpClient;
  private readonly OdysseyAPIFactory _factory;
  private readonly string _userId;
  private readonly Faker<Contact> _faker;

  public DeleteTests(OdysseyAPIFactory factory)
  {
    _userId = Guid.NewGuid().ToString();
    _factory = factory;
    _httpClient = _factory.CreateAuthenticatedClient(_userId);
    _faker = _factory.ContactFaker();
  }

  [Fact]
  public async Task Delete_WhenNoAuth_ShouldReturnUnauthorized()
  {
    // Arrange
    var idDeleting = 1;
    UpdateContactRequest? request = null;
    var httpClient = _factory.CreateClient();

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await httpClient.DeleteAsync($"/Contacts/{idDeleting}");

    // Assert
    response.IsSuccessStatusCode.Should().BeFalse();
    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task Delete_WhenGivenAnId_ShouldDeleteRecordWithTheId()
  {
    // Arrange
    var testData = await _factory.CreateContacts(1);
    using (var scope = _factory.Services.CreateScope())
    {
      var scopedServices = scope.ServiceProvider;
      var dbContext = scopedServices.GetRequiredService<PhonebookDbContext>();

      var record = await dbContext.Contacts.FirstOrDefaultAsync(o=>o.Id == testData[0].Id);
      record.Should().NotBeNull();
      record!.Name.Should().Be(testData[0].Name);
      record!.Number.Should().Be(testData[0].Number);
      record!.Email.Should().Be(testData[0].Email);
      record!.AvatarUrl.Should().Be(testData[0].AvatarUrl);
    }

    // Act
    var response = await _httpClient.DeleteAsync($"/Contacts/{testData[0].Id}");

    // Assert
    response.IsSuccessStatusCode.Should().BeTrue();
    response.StatusCode.Should().Be(HttpStatusCode.OK);

    using (var scope = _factory.Services.CreateScope())
    {
      var scopedServices = scope.ServiceProvider;
      var dbContext = scopedServices.GetRequiredService<PhonebookDbContext>();

      var record = await dbContext.Contacts.FirstOrDefaultAsync(o => o.Id == testData[0].Id);
      record.Should().BeNull();
    }
  }
}
