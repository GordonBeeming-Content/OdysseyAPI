using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace OdysseyAPI.Models;

#pragma warning disable CS8618
public sealed class UpdateContactRequest
{
  [Required]
  public string Name { get; set; }

  [Required]
  public string Number { get; set; }

  public string? Email { get; set; }

  public string? AvatarUrl { get; set; }
}
