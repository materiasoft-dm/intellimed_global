using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IntelliMed.Core.DTOs;
using IntelliMed.Core.Interfaces;

namespace IntelliMed.Api.Controllers;

/// <summary>
/// The practice's own billing item catalog — a simple code/description/fee list the practice
/// manages itself. No external catalog, no automatic rebate calculation.
/// </summary>
[ApiController]
[Route("api/billing-items")]
public class BillingItemsController : ControllerBase
{
    private readonly IBillingItemRepository _repository;

    public BillingItemsController(IBillingItemRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<BillingItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllActive() => Ok(await _repository.GetAllActiveAsync());

    [HttpGet("search")]
    [ProducesResponseType(typeof(List<BillingItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string? query) => Ok(await _repository.SearchAsync(query));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BillingItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateBillingItemDto dto)
    {
        var id = await _repository.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBillingItemDto dto)
    {
        await _repository.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpPost("{id:int}/archive")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Archive(int id)
    {
        await _repository.ArchiveAsync(id);
        return NoContent();
    }
}
