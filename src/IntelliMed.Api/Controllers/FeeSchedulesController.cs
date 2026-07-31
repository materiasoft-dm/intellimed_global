using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IntelliMed.Core.DTOs;
using IntelliMed.Core.Interfaces;

namespace IntelliMed.Api.Controllers;

/// <summary>
/// Named price lists that can override the practice's own billing-item catalog prices
/// (e.g. a corporate contract rate). Entirely user-managed, no automatic calculation.
/// </summary>
[ApiController]
[Route("api/fee-schedules")]
public class FeeSchedulesController : ControllerBase
{
    private readonly IFeeScheduleRepository _repository;

    public FeeSchedulesController(IFeeScheduleRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<FeeScheduleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllActive() => Ok(await _repository.GetAllActiveAsync());

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FeeScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var schedule = await _repository.GetByIdAsync(id);
        return schedule == null ? NotFound() : Ok(schedule);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateFeeScheduleDto dto)
    {
        var id = await _repository.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFeeScheduleDto dto)
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

    [HttpGet("{id:int}/items")]
    [ProducesResponseType(typeof(List<FeeScheduleItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetItems(int id) => Ok(await _repository.GetItemsAsync(id));

    [HttpPut("{id:int}/items")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SaveItem(int id, [FromBody] SaveFeeScheduleItemDto dto)
    {
        await _repository.SaveItemAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}/items/{billingItemId:int}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveItem(int id, int billingItemId)
    {
        await _repository.RemoveItemAsync(id, billingItemId);
        return NoContent();
    }

    /// <summary>Live line-item price lookup for the invoice entry UI — a plain catalog/price-list lookup, no calculation.</summary>
    [HttpPost("resolve-line")]
    [ProducesResponseType(typeof(ResolveLineResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ResolveLine([FromBody] ResolveLineRequest request)
    {
        var result = await _repository.ResolveLineAsync(request);
        return result == null ? NotFound() : Ok(result);
    }
}
