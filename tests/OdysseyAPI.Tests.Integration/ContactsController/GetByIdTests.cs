using System.Net;
using System.Net.Http.Json;
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
  public async Task GetById_WhenCalledWithInValidId_ShouldReturnNotFound()
  {
    // Arrange
    var id = 999;
    var expectedStatusCode = HttpStatusCode.NotFound;

    // Act
    var response = await _httpClient.GetAsync($"/Contacts/{id}");

    // Assert
    Assert.False(response.IsSuccessStatusCode);
    Assert.Equal(expectedStatusCode, response.StatusCode);
  }

  [Fact]
  public async Task GetById_WhenCalledWithValidId_ShouldReturnAContactModel()
  {
    // Arrange
    await _factory.CreateContacts(1);
    var id = 1;
    var expectedStatusCode = HttpStatusCode.OK;

    // Act
    var response = await _httpClient.GetAsync($"/Contacts/{id}");

    // Assert
    Assert.True(response.IsSuccessStatusCode);
    Assert.Equal(expectedStatusCode, response.StatusCode);

    var result = await response.Content.ReadFromJsonAsync<ContactModel>();
    Assert.NotNull(result);
    Assert.Equal(id, result.Id);
  }
}
