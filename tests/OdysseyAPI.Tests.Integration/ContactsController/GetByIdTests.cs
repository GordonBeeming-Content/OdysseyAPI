using System.Net;
using System.Net.Http.Json;
using OdysseyAPI.Models;

namespace OdysseyAPI.Tests.Integration.ContactsController;

public class GetByIdTests
{
  [Fact]
  public async Task GetById_WhenCalledWithInValidId_ShouldReturnNotFound()
  {
    // Arrange
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("https://localhost:7091");
    var id = 999;
    var expectedStatusCode = HttpStatusCode.NotFound;

    // Act
    var response = await httpClient.GetAsync($"/Contacts/{id}");

    // Assert
    Assert.False(response.IsSuccessStatusCode);
    Assert.Equal(expectedStatusCode, response.StatusCode);
  }

  [Fact]
  public async Task GetById_WhenCalledWithValidId_ShouldReturnAContactModel()
  {
    // Arrange
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("https://localhost:7091");
    var id = 1;
    var expectedStatusCode = HttpStatusCode.OK;

    // Act
    var response = await httpClient.GetAsync($"/Contacts/{id}");

    // Assert
    Assert.True(response.IsSuccessStatusCode);
    Assert.Equal(expectedStatusCode, response.StatusCode);

    var result = await response.Content.ReadFromJsonAsync<ContactModel>();
    Assert.NotNull(result);
    Assert.Equal(id, result.Id);
  }
}
