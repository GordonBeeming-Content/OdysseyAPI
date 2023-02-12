using OdysseyAPI.Models;

namespace OdysseyAPI.Tests.Integration.ContactsController;

[Collection(OdysseyAPICollection.Definition)]
public sealed class GetByIdTests
{
  private readonly HttpClient _httpClient;
  private readonly OdysseyAPIFactory _factory;
  private readonly string _userId;

  public GetByIdTests(OdysseyAPIFactory factory)
  {
    _userId = Guid.NewGuid().ToString();
    _factory = factory;
    _httpClient = _factory.CreateAuthenticatedClient(_userId);
  }

  [Fact]
  public async Task GetById_WhenNoAuth_ShouldReturnUnauthorized()
  {
    // Arrange
    var id = 999;
    var httpClient = _factory.CreateClient();

    // Act
    var response = await httpClient.GetAsync($"/Contacts/{id}");

    // Assert
    response.IsSuccessStatusCode.Should().BeFalse();
    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
  }

  [Fact]
  public async Task GetById_WhenCalledWithInValidId_ShouldReturnNotFound()
  {
    // Arrange
    var id = 999;

    // Act
    var response = await _httpClient.GetAsync($"/Contacts/{id}");

    // Assert
    response.IsSuccessStatusCode.Should().BeFalse();
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task GetById_WhenCalledWithValidId_ShouldReturnAContactModel()
  {
    // Arrange
    await _factory.CreateContacts(1);
    var id = 1;

    // Act
    var response = await _httpClient.GetAsync($"/Contacts/{id}");

    // Assert
    response.IsSuccessStatusCode.Should().BeTrue();
    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var result = await response.Content.ReadFromJsonAsync<ContactModel>();
    result.Should().NotBeNull();
    result!.Id.Should().Be(id);
    result.Name.Should().NotBeNullOrWhiteSpace();
    result.Number.Should().NotBeNullOrWhiteSpace();
    result.AvatarUrl.Should().NotBeNullOrWhiteSpace();
  }
}
