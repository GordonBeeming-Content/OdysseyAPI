using System.Net;
using System.Net.Http.Json;
using OdysseyAPI.Models;

namespace OdysseyAPI.Tests.Integration.ContactsController;

public class GetTests
{
  [Fact]
  public async Task Get_WhenCalled_ShouldReturnAListOfContactModel()
  {
    // Arrange
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("https://localhost:7091");
    var expectedStatusCode = HttpStatusCode.OK;

    // Act
    var response = await httpClient.GetAsync("/Contacts");

    // Assert
    Assert.True(response.IsSuccessStatusCode);
    Assert.Equal(expectedStatusCode, response.StatusCode);

    var result = await response.Content.ReadFromJsonAsync<List<ContactModel>>();
    Assert.NotNull(result);
    Assert.NotEmpty(result);
  }
}
