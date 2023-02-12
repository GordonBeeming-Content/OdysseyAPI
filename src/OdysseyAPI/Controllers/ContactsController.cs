using Microsoft.AspNetCore.Mvc;
using OdysseyAPI.Data;
using OdysseyAPI.Models;

namespace OdysseyAPI.Controllers;

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
  public async Task<ActionResult> Get()
  {
    _logger.LogInformation("Getting contacts");
    var result = await _phonebookDbContext.Contacts
      .Select(o => new ContactModel
      {
        Id = o.Id,
        Name = o.Name,
        Number = o.Number

      })
      .ToListAsync();
    return Ok(result);
  }

  [HttpGet("{id}")]
  [ProducesResponseType(typeof(ContactModel), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  public async Task<ActionResult> GetById(int id)
  {
    _logger.LogInformation("Getting contact {Contact Id}", id);
    var result = await _phonebookDbContext.Contacts
      .Select(o => new ContactModel
      {
        Id = o.Id,
        Name = o.Name,
        Number = o.Number

      })
      .FirstOrDefaultAsync(o => o.Id == id);
    if (result == null)
    {
      return NotFound();
    }
    return Ok(result);
  }

  [HttpPut]
  [ProducesResponseType(typeof(ContactModel), StatusCodes.Status201Created)]
  [Consumes("application/json")]
  public async Task<ActionResult> Create(ContactModel model)
  {
    _logger.LogInformation("Creating new contact");
    var contact = new Contact
    {
      Name = model.Name,
      Number = model.Number
    };
    _phonebookDbContext.Contacts.Add(contact);
    await _phonebookDbContext.SaveChangesAsync();

    var dbContact = await _phonebookDbContext.Contacts
      .FirstOrDefaultAsync(o => o.Id == contact.Id);
    model = new ContactModel
    {
      Id = dbContact!.Id,
      Name = dbContact.Name,
      Number = dbContact.Number
    };
    var getUri = Url.Action(nameof(GetById), new { id = model.Id, });
    return Created(getUri!, model);
  }

  [HttpPost]
  [ProducesResponseType(typeof(ContactModel), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [Consumes("application/json")]
  public async Task<ActionResult> Update(ContactModel model)
  {
    _logger.LogInformation("Updating contact {Contact Id}", model.Id);
    var contact = await _phonebookDbContext.Contacts
      .FirstOrDefaultAsync(o => o.Id == model.Id);
    if (contact == null)
    {
      return NotFound();
    }
    contact.Name = model.Name;
    contact.Number = model.Number;
    await _phonebookDbContext.SaveChangesAsync();
    return Ok(model);
  }
}
