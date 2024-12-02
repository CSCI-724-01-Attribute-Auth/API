using API.Authorization;
using API.Data;
using API.Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class PersonController : Controller
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet("all")]
        [UseAttributeAuthorizer]
        [ProducesResponseType(typeof(List<Person>), StatusCodes.Status200OK)]
        public IActionResult All()
        {
            return StatusCode(StatusCodes.Status200OK, new { persons = _personService.GetAllPersons() });
        }

        [HttpGet("")]
        [UseAttributeAuthorizer]
        [ProducesResponseType(typeof(Person), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GetPersonById([FromQuery] int id)
        {
            var person = _personService.GetPerson(id);

            if (person == null) {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            return StatusCode(StatusCodes.Status200OK, new { person });
        }

        [HttpPost("")]
        [UseAttributeAuthorizer]
        [ProducesResponseType(typeof(Person), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreatePerson(
            [FromQuery] string name,
            [FromQuery] DateTime birthDate,
            [FromQuery] int mostFamousMovieId
        )
        {
            try
            {
                var createdPerson = _personService.CreatePerson(name, birthDate, mostFamousMovieId);

                return StatusCode(StatusCodes.Status201Created, new { person = createdPerson });
            }
            catch (InvalidDataException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
        }
    }
}
