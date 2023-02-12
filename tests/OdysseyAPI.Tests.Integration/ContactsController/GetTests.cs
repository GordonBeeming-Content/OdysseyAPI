using System.Net;
using System.Net.Http.Json;
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
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("https://localhost:7091");
    var expectedStatusCode = HttpStatusCode.OK;

    // Act
    var response = await _httpClient.GetAsync("/Contacts");

    // Assert
    Assert.True(response.IsSuccessStatusCode);
    Assert.Equal(expectedStatusCode, response.StatusCode);

    var result = await response.Content.ReadFromJsonAsync<List<ContactModel>>();
    Assert.NotNull(result);
    Assert.NotEmpty(result);
  }
}
