using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace OdysseyAPI.Models;

#pragma warning disable CS8618
public sealed class ContactModel
{
  public int Id { get; set; }

  public string Name { get; set; }

  public string Number { get; set; }

  public string? AvatarUrl { get; set; }
}
