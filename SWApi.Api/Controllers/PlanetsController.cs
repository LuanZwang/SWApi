using Microsoft.AspNetCore.Mvc;
using SWApi.Application.Service.Interface;
using SWApi.Domain.Dto.Api.Planet;

namespace SWApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class PlanetsController : ControllerBase
{
    private readonly IPlanetService _planetService;
    
    public PlanetsController(IPlanetService planetService)
    {
        _planetService = planetService;
    }

    /// <summary>
    /// Searches for a planet by its id and returns it.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlanetDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Get(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Only valid GUIDs are allowed.");

        return Ok(_planetService.GetById(id));
    }

    [HttpGet, Route("name/{name}")]
    public IActionResult GetByName(string name)
    {
        _planetService.GetByName(name);

        return Ok();
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] int? page, [FromQuery] int? pageSize)
    {
        return Ok(_planetService.GetAll(page, pageSize));
    }

    [HttpDelete, Route("{id}")]
    public IActionResult Delete(Guid id)
    {
        var deleted = _planetService.Delete(id);


        return deleted ? NoContent() : NotFound();
    }
}