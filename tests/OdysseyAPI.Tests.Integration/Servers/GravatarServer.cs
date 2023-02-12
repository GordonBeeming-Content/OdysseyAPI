using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace OdysseyAPI.Tests.Integration.Servers;

public sealed class GravatarServer : IDisposable 
{
  private WireMockServer _server = null!;

  public void Start()
  {
    _server = WireMockServer.Start();
  }

  public string? Url => _server.Url;

  public void SetupAvatar(string email)
  {
    var emailHash = GetEmailHash(email);

    _server.Given(Request.Create().WithPath($"/avatar/{emailHash}").UsingGet())
      .RespondWith(Response.Create().WithStatusCode(200)
                                    .WithHeader("content-type", "image/jpeg")
                                    .WithBody("binary not used"));
  }

  public void Dispose()
  {
    _server.Stop();
    _server.Dispose();
  }

  #region nothing to see here

  // Ideally we'd have this logic under unit tests to verify it's solid and then reuse a service that does this hashing specifically outside the Gravatar Service

  private string GetEmailHash(string email)
  {
    email = email.Trim().ToLowerInvariant();
    return CreateMD5Hash(email);
  }

  private string CreateMD5Hash(string email)
  {
    using var md5 = MD5.Create();
    byte[] inputBytes = Encoding.ASCII.GetBytes(email);
    byte[] hashBytes = md5.ComputeHash(inputBytes);
    var sb = new StringBuilder();
    foreach (var t in hashBytes)
    {
      sb.Append(t.ToString("x2"));
    }
    return sb.ToString().ToLowerInvariant();
  }
  #endregion
}
