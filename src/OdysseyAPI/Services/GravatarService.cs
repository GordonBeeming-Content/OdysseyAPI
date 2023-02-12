using System.Security.Cryptography;
using System.Text;

namespace OdysseyAPI.Services
{
  public interface IGravatarService
  {
    Task<string?> GetGravatarForEmail(string email, bool nullIfNotAvailable = false);
  }

  public sealed class GravatarService : IGravatarService
  {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;

    public GravatarService(IHttpClientFactory httpClientFactory)
    {
      _httpClientFactory = httpClientFactory;
      _httpClient = _httpClientFactory.CreateClient("Gravatar");
    }

    public async Task<string?> GetGravatarForEmail(string email, bool nullIfNotAvailable = false)
    {
      var emailHash = GetEmailHash(email);
      if (!nullIfNotAvailable)
      {
        return $"{_httpClient.BaseAddress}avatar/{emailHash}";
      }
      var url = $"avatar/{emailHash}";
      var response = await _httpClient.GetAsync($"{url}?d=404");
      if (response.IsSuccessStatusCode)
      {
        return $"{_httpClient.BaseAddress}{url}";
      }
      return null;
    }

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
  }
}
