﻿using OdysseyAPI.Models;

namespace OdysseyAPI.Tests.Integration.ContactsController;

[Collection(OdysseyAPICollection.Definition)]
public sealed class CreateTests
{
  private readonly HttpClient _httpClient;
  private readonly OdysseyAPIFactory _factory;
  private readonly string _userId;

  public CreateTests(OdysseyAPIFactory factory)
  {
    _userId = Guid.NewGuid().ToString();
    _factory = factory;
    _httpClient = _factory.CreateAuthenticatedClient(_userId);
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
}
