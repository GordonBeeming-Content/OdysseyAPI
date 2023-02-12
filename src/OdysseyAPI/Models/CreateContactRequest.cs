using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace OdysseyAPI.Models;

#pragma warning disable CS8618
public sealed class CreateContactRequest
{
  [Required]
  public string Name { get; set; }

  [Required]
  public string Number { get; set; }
}
