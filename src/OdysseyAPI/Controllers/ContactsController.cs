﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdysseyAPI.Data;
using OdysseyAPI.Models;

namespace OdysseyAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public sealed class ContactsController : ControllerBase
{
  private readonly ILogger<ContactsController> _logger;
  private readonly PhonebookDbContext _phonebookDbContext;

  public ContactsController(ILogger<ContactsController> logger, PhonebookDbContext phonebookDbContext)
  {
    _logger = logger;
    _phonebookDbContext = phonebookDbContext;
  }

  [HttpGet]
  [ProducesResponseType(typeof(List<ContactModel>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
  public async Task<ActionResult> Get()
  {
    _logger.LogInformation("Getting contacts");
    var result = await _phonebookDbContext.Contacts
      .Select(o => new ContactModel
      {
        Id = o.Id,
        Name = o.Name,
        Number = o.Number,
        AvatarUrl = o.AvatarUrl,
      })
      .ToListAsync();
    return Ok(result);
  }

  [HttpGet("{id}")]
  [ProducesResponseType(typeof(ContactModel), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
  public async Task<ActionResult> GetById(int id)
  {
    _logger.LogInformation("Getting contact {Contact Id}", id);
    var response = await _phonebookDbContext.Contacts
      .Select(o => new ContactModel
      {
        Id = o.Id,
        Name = o.Name,
        Number = o.Number,
        AvatarUrl = o.AvatarUrl,
      })
      .FirstOrDefaultAsync(o => o.Id == id);
    if (response == null)
    {
      return NotFound();
    }
    return Ok(response);
  }

  [HttpPut]
  [ProducesResponseType(typeof(ContactModel), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
  [Consumes("application/json")]
  public async Task<ActionResult> Create(CreateContactRequest request, [FromServices] IGravatarService gravatarService)
  {
    _logger.LogInformation("Creating new contact");
    var newContact = new Contact
    {
      Name = request.Name,
      Number = request.Number,
      AvatarUrl = request.AvatarUrl,
      Email = request.Email,
    };
    if (string.IsNullOrWhiteSpace(newContact.AvatarUrl) && !string.IsNullOrWhiteSpace(newContact.Email))
    {
      newContact.AvatarUrl = await gravatarService.GetGravatarForEmail(newContact.Email, true);
    }
    _phonebookDbContext.Contacts.Add(newContact);
    await _phonebookDbContext.SaveChangesAsync();

    var dbContact = await _phonebookDbContext.Contacts
      .FirstOrDefaultAsync(o => o.Id == newContact.Id);
    var response = new ContactModel
    {
      Id = dbContact!.Id,
      Name = dbContact.Name,
      Number = dbContact.Number,
      AvatarUrl = dbContact.AvatarUrl,
    };
    var getUri = Url.Action(nameof(GetById), new { id = response.Id, });
    return Created(getUri!, response);
  }

  [HttpPost("{id}")]
  [ProducesResponseType(typeof(ContactModel), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
  [Consumes("application/json")]
  public async Task<ActionResult> Update(int id, UpdateContactRequest request, [FromServices] IGravatarService gravatarService)
  {
    _logger.LogInformation("Updating contact {Contact Id}", id);

    var dbContact = await _phonebookDbContext.Contacts
      .FirstOrDefaultAsync(o => o.Id == id);
    if (dbContact == null)
    {
      return NotFound();
    }

    dbContact.Name = request.Name;
    dbContact.Number = request.Number;
    dbContact.AvatarUrl = request.AvatarUrl;
    dbContact.Email = request.Email;
    if (string.IsNullOrWhiteSpace(dbContact.AvatarUrl) && !string.IsNullOrWhiteSpace(dbContact.Email))
    {
      dbContact.AvatarUrl = await gravatarService.GetGravatarForEmail(dbContact.Email, true);
    }
    await _phonebookDbContext.SaveChangesAsync();

    dbContact = await _phonebookDbContext.Contacts
      .FirstOrDefaultAsync(o => o.Id == id);
    var response = new ContactModel
    {
      Id = dbContact!.Id,
      Name = dbContact.Name,
      Number = dbContact.Number,
      AvatarUrl = dbContact.AvatarUrl,
    };
    return Ok(response);
  }

  [HttpDelete("{id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
  [Consumes("application/json")]
  public async Task<ActionResult> Delete(int id)
  {
    _logger.LogInformation("Deleting contact {Contact Id}", id);

    var dbContact = await _phonebookDbContext.Contacts
      .FirstOrDefaultAsync(o => o.Id == id);
    if (dbContact == null)
    {
      return NotFound();
    }
    _phonebookDbContext.Contacts.Remove(dbContact);
    await _phonebookDbContext.SaveChangesAsync();
    return Ok();
  }
}
