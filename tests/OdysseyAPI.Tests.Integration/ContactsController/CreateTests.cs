using System.Net;
using System.Net.Http.Json;
using OdysseyAPI.Models;

namespace OdysseyAPI.Tests.Integration.ContactsController;

public class CreateTests
{
  [Fact]
  public async Task Create_WhenCalledWithInValidModel_ShouldReturnBadRequest()
  {
    // Arrange
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("https://localhost:7091");
    var expectedStatusCode = HttpStatusCode.BadRequest;
    CreateContactRequest? request = null;

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await httpClient.PutAsync($"/Contacts", requestContent);

    // Assert
    Assert.False(response.IsSuccessStatusCode);
    Assert.Equal(expectedStatusCode, response.StatusCode);
  }

  [Fact]
  public async Task Create_WhenCalledWithValidId_ShouldReturnAContactModel()
  {
    // Arrange
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("https://localhost:7091");
    var expectedStatusCode = HttpStatusCode.Created;
    var expectedLocationHeaderStartsWith = "/Contacts/";
    var request = new CreateContactRequest
    {
      Name = $"Gordon Beeming - {Guid.NewGuid()}",
      Number = "078 555 1234",
    };

    // Act
    var requestContent = JsonContent.Create(request);
    var response = await httpClient.PutAsync($"/Contacts", requestContent);

    // Assert
    Assert.True(response.IsSuccessStatusCode);
    Assert.Equal(expectedStatusCode, response.StatusCode);
    var locationHeader = response.Headers.Location?.ToString();
    Assert.NotNull(locationHeader);
    Assert.StartsWith(expectedLocationHeaderStartsWith, locationHeader);

    var result = await response.Content.ReadFromJsonAsync<ContactModel>();
    Assert.NotNull(result);
    var expectedLocationHeader = $"{expectedLocationHeaderStartsWith}{result.Id}";
    Assert.Equal(expectedLocationHeader, locationHeader);
  }
}
