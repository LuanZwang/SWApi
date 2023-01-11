using Microsoft.AspNetCore.Mvc;
using SWApi.Application.Service.Interface;
using SWApi.Domain.Dto.Api.Commom;
using SWApi.Domain.Dto.Api.Planet;

namespace SWApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(contentType: "application/json")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public class PlanetsController : ControllerBase
    {
        private readonly IPlanetService _planetService;

        public PlanetsController(IPlanetService planetService)
        {
            _planetService = planetService;
        }

        [HttpGet, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlanetDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Only valid GUIDs are allowed.");

            var planetResult = _planetService.GetById(id);

            if (planetResult is null)
                return NotFound();

            return Ok(planetResult);
        }

        [HttpGet, Route("name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlanetDto[]))]
        public IActionResult GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Planet name for search should be informed.");
            else if (name.Length > 50)
                return BadRequest("Planet name for search should not be greater than 50 characters.");

            return Ok(_planetService.GetByName(name));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllDto<PlanetDto>))]
        public IActionResult GetAll([FromQuery] int? page, [FromQuery] int? pageSize)
        {
            if (pageSize.HasValue && pageSize.Value > 100)
                return BadRequest("Page size cannot be greater than 100.");

            return Ok(_planetService.GetAll(page, pageSize));
        }

        [HttpDelete, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Only valid GUIDs are allowed.");
            }

            var deleted = _planetService.Delete(id);

            return deleted ? NoContent() : NotFound();
        }
    }
}