using Microsoft.AspNetCore.Authorization;
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
        Number = o.Number

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
        Number = o.Number

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
  public async Task<ActionResult> Create(CreateContactRequest request)
  {
    _logger.LogInformation("Creating new contact");
    var newContact = new Contact
    {
      Name = request.Name,
      Number = request.Number
    };
    _phonebookDbContext.Contacts.Add(newContact);
    await _phonebookDbContext.SaveChangesAsync();

    var dbContact = await _phonebookDbContext.Contacts
      .FirstOrDefaultAsync(o => o.Id == newContact.Id);
    var response = new ContactModel
    {
      Id = dbContact!.Id,
      Name = dbContact.Name,
      Number = dbContact.Number
    };
    var getUri = Url.Action(nameof(GetById), new { id = response.Id, });
    return Created(getUri!, response);
  }
}
