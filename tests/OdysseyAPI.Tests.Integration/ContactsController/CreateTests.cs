using Bogus;
using Bogus.DataSets;
using OdysseyAPI.Data.Domain;
using OdysseyAPI.Models;

namespace OdysseyAPI.Tests.Integration.ContactsController;

[Collection(OdysseyAPICollection.Definition)]
public sealed class CreateTests
{
  private readonly HttpClient _httpClient;
  private readonly OdysseyAPIFactory _factory;
  private readonly string _userId;
  private readonly Faker<Contact> _faker;

  public CreateTests(OdysseyAPIFactory factory)
  {
    _userId = Guid.NewGuid().ToString();
    _factory = factory;
    _httpClient = _factory.CreateAuthenticatedClient(_userId);
    _faker = _factory.ContactFaker();
  }

  [Fact]
  public async Task Create_WhenNoAuth_ShouldReturnUnauthorized()
  {
    // Arrange
    CreateContactRequest? request = null;
    var httpClient = _factory.CreateClient();

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await httpClient.PutAsync($"/Contacts", requestContent);

    // Assert
    response.IsSuccessStatusCode.Should().BeFalse();
    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task Create_WhenCalledWithInValidModel_ShouldReturnBadRequest()
  {
    // Arrange
    CreateContactRequest? request = null;

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await _httpClient.PutAsync($"/Contacts", requestContent);

    // Assert
    response.IsSuccessStatusCode.Should().BeFalse();
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task Create_WhenCalledWithValidId_ShouldReturnAContactModel()
  {
    // Arrange
    var request = new CreateContactRequest
    {
      Name = $"Gordon Beeming - {Guid.NewGuid()}",
      Number = "078 555 1234",
    };

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await _httpClient.PutAsync($"/Contacts", requestContent);

    // Assert
    response.IsSuccessStatusCode.Should().BeTrue();
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    var locationHeader = response.Headers.Location?.ToString();
    locationHeader.Should().NotBeNull();
    locationHeader.Should().StartWith("/Contacts/");

    var result = await response.Content.ReadFromJsonAsync<ContactModel>();
    result.Should().NotBeNull();
    locationHeader.Should().Be($"/Contacts/{result!.Id}");
  }

  [Fact]
  public async Task Create_WhenAvatarUrlSupplied_ShouldReturnAvatarUrl()
  {
    // Arrange
    var record = _faker.Generate();
    var request = new CreateContactRequest
    {
      Name = record.Name,
      Number = record.Number,
      AvatarUrl = record.AvatarUrl,
    };
    var expectedAvatarUrl = "https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/321.jpg";

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await _httpClient.PutAsync($"/Contacts", requestContent);

    // Assert
    response.IsSuccessStatusCode.Should().BeTrue();
    
    var result = await response.Content.ReadFromJsonAsync<ContactModel>();
    result.Should().NotBeNull();
    result!.AvatarUrl.Should().Be(expectedAvatarUrl);
  }

  [Fact]
  public async Task Create_WhenAvatarUrlNotSupplied_ShouldReturnNoAvatarUrl()
  {
    // Arrange
    var record = _faker.Generate();
    var request = new CreateContactRequest
    {
      Name = record.Name,
      Number = record.Number,
      AvatarUrl = null,
    };

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await _httpClient.PutAsync($"/Contacts", requestContent);

    // Assert
    response.IsSuccessStatusCode.Should().BeTrue();

    var result = await response.Content.ReadFromJsonAsync<ContactModel>();
    result.Should().NotBeNull();
    result!.AvatarUrl.Should().BeNull();
  }

  [Fact]
  public async Task Create_WhenAvatarUrlNotSuppliedWithGravatarEmailSupplied_ShouldReturnGravatarAvatarUrl()
  {
    // Arrange
    var record = _faker.Generate();
    var request = new CreateContactRequest
    {
      Name = record.Name,
      Number = record.Number,
      AvatarUrl = null,
      Email = ValidGravatarEmail,
    };
    var expectedAvatarUrl = $"{_factory.GravatarServerUrl}{ValidGravatarEmailAvatarRelativeUrl}";

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await _httpClient.PutAsync($"/Contacts", requestContent);

    // Assert
    response.IsSuccessStatusCode.Should().BeTrue();
    response.StatusCode.Should().Be(HttpStatusCode.Created);

    var result = await response.Content.ReadFromJsonAsync<ContactModel>();
    result.Should().NotBeNull();
    result!.AvatarUrl.Should().Be(expectedAvatarUrl);
  }

  [Fact]
  public async Task Create_WhenAvatarUrlNotSuppliedWithInvalidGravatarEmail_ShouldReturnNoAvatarUrl()
  {
    // Arrange
    var record = _faker.Generate();
    var request = new CreateContactRequest
    {
      Name = record.Name,
      Number = record.Number,
      AvatarUrl = null,
      Email = InValidGravatarEmail,
    };

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await _httpClient.PutAsync($"/Contacts", requestContent);

    // Assert
    response.IsSuccessStatusCode.Should().BeTrue();
    response.StatusCode.Should().Be(HttpStatusCode.Created);

    var result = await response.Content.ReadFromJsonAsync<ContactModel>();
    result.Should().NotBeNull();
    result!.AvatarUrl.Should().BeNull();
  }
}
