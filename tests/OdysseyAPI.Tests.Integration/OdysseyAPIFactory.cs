using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using OdysseyAPI.Data;
using WebMotions.Fake.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;

namespace OdysseyAPI.Tests.Integration;

public sealed class OdysseyAPIFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
  private readonly TestcontainerDatabase _dbContainer;

  public OdysseyAPIFactory()
  {
    _dbContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
        .WithDatabase(new MsSqlTestcontainerConfiguration
        {
          Password = "!U6tQrYmDPWvjBX6-Pfi",
        })
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithExposedPort(1433)
        .WithPortBinding(1433, true)
        .Build();
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureAppConfiguration(config =>
    {
      config.AddCommandLine(new[] { $"ConnectionStrings:DefaultConnection=Server={_dbContainer.Hostname},{_dbContainer.Port};Database=Odyssey-Phonebook;User Id={_dbContainer.Username};Password={_dbContainer.Password};MultipleActiveResultSets=true;TrustServerCertificate=True" });
    });

    builder.ConfigureTestServices(services =>
    {
      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = FakeJwtBearerDefaults.AuthenticationScheme;
      }).AddFakeJwtBearer();
    });
  }

  public new HttpClient CreateClient()
  {
    var httpClient = base.CreateClient();
    return httpClient;
  }

  public HttpClient CreateAuthenticatedClient(string userId)
  {
    var httpClient = CreateClient();
    httpClient.SetFakeBearerToken(userId.ToString());
    return httpClient;
  }

  public async Task InitializeAsync()
  {
    await _dbContainer.StartAsync();
    RunMigrations();
  }

  public new async Task DisposeAsync()
  {
    await _dbContainer.DisposeAsync();
    await base.DisposeAsync();
  }

  private void RunMigrations()
  {
    using var serviceScope = Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    var applicationDbContext = serviceScope.ServiceProvider.GetRequiredService<PhonebookDbContext>();
    applicationDbContext.Database.Migrate();
  }
}
