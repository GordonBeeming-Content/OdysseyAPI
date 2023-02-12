using OdysseyAPI.Models;

namespace OdysseyAPI.Tests.Integration.ContactsController;

[Collection(OdysseyAPICollection.Definition)]
public sealed class GetTests
{
  private readonly HttpClient _httpClient;
  private readonly OdysseyAPIFactory _factory;
  private readonly string _userId;

  public GetTests(OdysseyAPIFactory factory)
  {
    _userId = Guid.NewGuid().ToString();
    _factory = factory;
    _httpClient = _factory.CreateAuthenticatedClient(_userId);
  }

  [Fact]
  public async Task Get_WhenCalled_ShouldReturnAListOfContactModel()
  {
    // Arrange
    await _factory.CreateContacts(5);

    // Act
    var response = await _httpClient.GetAsync("/Contacts");

    // Assert
    response.IsSuccessStatusCode.Should().BeTrue();
    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var result = await response.Content.ReadFromJsonAsync<List<ContactModel>>();
    result.Should().NotBeNull();
    result.Should().NotBeEmpty();
  }
}
