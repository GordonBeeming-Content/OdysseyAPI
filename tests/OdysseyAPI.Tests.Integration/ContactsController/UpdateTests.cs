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
public sealed class UpdateTests
{
  private readonly HttpClient _httpClient;
  private readonly OdysseyAPIFactory _factory;
  private readonly string _userId;
  private readonly Faker<Contact> _faker;

  public UpdateTests(OdysseyAPIFactory factory)
  {
    _userId = Guid.NewGuid().ToString();
    _factory = factory;
    _httpClient = _factory.CreateAuthenticatedClient(_userId);
    _faker = _factory.ContactFaker();
  }

  [Fact]
  public async Task Update_WhenNoAuth_ShouldReturnUnauthorized()
  {
    // Arrange
    var idUpdating = 1;
    UpdateContactRequest? request = null;
    var httpClient = _factory.CreateClient();

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await httpClient.PostAsync($"/Contacts/{idUpdating}", requestContent);

    // Assert
    response.IsSuccessStatusCode.Should().BeFalse();
    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task Update_WhenCalledWithInValidModel_ShouldReturnBadRequest()
  {
    // Arrange
    var idUpdating = 1;
    UpdateContactRequest? request = null;

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await _httpClient.PostAsync($"/Contacts/{idUpdating}", requestContent);

    // Assert
    response.IsSuccessStatusCode.Should().BeFalse();
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task Update_WhenGivenAnId_ShouldUpdateRecordWithTheId()
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
    var request = new UpdateContactRequest
    {
      Name = $"Gordon Beeming - {Guid.NewGuid()}",
      Number = "078 111 1234",
      Email = "a@b.com",
      AvatarUrl = "http://noused"
    };

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await _httpClient.PostAsync($"/Contacts/{testData[0].Id}", requestContent);

    // Assert
    response.IsSuccessStatusCode.Should().BeTrue();
    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var result = await response.Content.ReadFromJsonAsync<ContactModel>();
    result.Should().NotBeNull();
    result!.Name.Should().Be(request.Name);
    result!.Number.Should().Be(request.Number);
    result!.AvatarUrl.Should().Be(request.AvatarUrl);
    using (var scope = _factory.Services.CreateScope())
    {
      var scopedServices = scope.ServiceProvider;
      var dbContext = scopedServices.GetRequiredService<PhonebookDbContext>();

      var record = await dbContext.Contacts.FirstOrDefaultAsync(o => o.Id == testData[0].Id);
      record.Should().NotBeNull();
      record!.Name.Should().Be(request.Name);
      record!.Number.Should().Be(request.Number);
      record!.Email.Should().Be(request.Email);
      record!.AvatarUrl.Should().Be(request.AvatarUrl);
    }
  }

  [Fact]
  public async Task Update_WhenCalledWithValidId_ShouldReturnAContactModel()
  {
    // Arrange
    var testData = await _factory.CreateContacts(1);
    var request = new UpdateContactRequest
    {
      Name = $"Gordon Beeming - {Guid.NewGuid()}",
      Number = "078 555 1234",
    };

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await _httpClient.PostAsync($"/Contacts/{testData[0].Id}", requestContent);

    // Assert
    response.IsSuccessStatusCode.Should().BeTrue();
    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var result = await response.Content.ReadFromJsonAsync<ContactModel>();
    result.Should().NotBeNull();
  }

  [Fact]
  public async Task Update_WhenAvatarUrlSupplied_ShouldReturnAvatarUrl()
  {
    // Arrange
    var testData = await _factory.CreateContacts(1);
    var record = _faker.Generate();
    var request = new UpdateContactRequest
    {
      Name = record.Name,
      Number = record.Number,
      AvatarUrl = record.AvatarUrl,
    };
    var expectedAvatarUrl = "https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/321.jpg";

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await _httpClient.PostAsync($"/Contacts/{testData[0].Id}", requestContent);

    // Assert
    response.IsSuccessStatusCode.Should().BeTrue();
    
    var result = await response.Content.ReadFromJsonAsync<ContactModel>();
    result.Should().NotBeNull();
    result!.AvatarUrl.Should().Be(expectedAvatarUrl);
  }

  [Fact]
  public async Task Update_WhenAvatarUrlNotSupplied_ShouldReturnNoAvatarUrl()
  {
    // Arrange
    var testData = await _factory.CreateContacts(1);
    var record = _faker.Generate();
    var request = new UpdateContactRequest
    {
      Name = record.Name,
      Number = record.Number,
      AvatarUrl = null,
    };

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await _httpClient.PostAsync($"/Contacts/{testData[0].Id}", requestContent);

    // Assert
    response.IsSuccessStatusCode.Should().BeTrue();

    var result = await response.Content.ReadFromJsonAsync<ContactModel>();
    result.Should().NotBeNull();
    result!.AvatarUrl.Should().BeNull();
  }

  [Fact]
  public async Task Update_WhenAvatarUrlNotSuppliedWithGravatarEmailSupplied_ShouldReturnGravatarAvatarUrl()
  {
    // Arrange
    var testData = await _factory.CreateContacts(1);
    var record = _faker.Generate();
    var request = new UpdateContactRequest
    {
      Name = record.Name,
      Number = record.Number,
      AvatarUrl = null,
      Email = ValidGravatarEmail,
    };
    var expectedAvatarUrl = $"{_factory.GravatarServerUrl}{ValidGravatarEmailAvatarRelativeUrl}";

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await _httpClient.PostAsync($"/Contacts/{testData[0].Id}", requestContent);

    // Assert
    response.IsSuccessStatusCode.Should().BeTrue();
    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var result = await response.Content.ReadFromJsonAsync<ContactModel>();
    result.Should().NotBeNull();
    result!.AvatarUrl.Should().Be(expectedAvatarUrl);
  }

  [Fact]
  public async Task Update_WhenAvatarUrlNotSuppliedWithInvalidGravatarEmail_ShouldReturnNoAvatarUrl()
  {
    // Arrange
    var testData = await _factory.CreateContacts(1);
    var record = _faker.Generate();
    var request = new UpdateContactRequest
    {
      Name = record.Name,
      Number = record.Number,
      AvatarUrl = null,
      Email = InValidGravatarEmail,
    };

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await _httpClient.PostAsync($"/Contacts/{testData[0].Id}", requestContent);

    // Assert
    response.IsSuccessStatusCode.Should().BeTrue();
    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var result = await response.Content.ReadFromJsonAsync<ContactModel>();
    result.Should().NotBeNull();
    result!.AvatarUrl.Should().BeNull();
  }
}
