using Microsoft.AspNetCore.Authorization;
using GasMonitoringApi.Data;
using GasMonitoringApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GasMonitoringApi.Controllers;

[ApiController]
[Route("api/wells")]
public class WellsController : ControllerBase
{
    private readonly AppData data;

    public WellsController(AppData data)
    {
        this.data = data;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        lock (data.SyncRoot)
        {
            return Ok(data.Wells.ToList());
        }
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        lock (data.SyncRoot)
        {
            var well = data.Wells.FirstOrDefault(w => w.Id == id);

            if (well == null)
                return NotFound("Скважина не найдена.");

            return Ok(well);
        }
    }
    [Authorize(Roles = "Admin")]    
    [HttpPost]
    public IActionResult Create(Well well)
    {
        lock (data.SyncRoot)
        {
            well.Id = data.GetNextWellId();
            data.Wells.Add(well);

            return CreatedAtAction(
                nameof(GetById),
                new { id = well.Id },
                well);
        }
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Well updatedWell)
    {
        lock (data.SyncRoot)
        {
            int index = data.Wells.FindIndex(w => w.Id == id);

            if (index == -1)
                return NotFound("Скважина не найдена.");

            updatedWell.Id = id;
            data.Wells[index] = updatedWell;

            return NoContent();
        }
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        lock (data.SyncRoot)
        {
            var well = data.Wells.FirstOrDefault(w => w.Id == id);

            if (well == null)
                return NotFound("Скважина не найдена.");

            if (data.Measurements.Any(m => m.WellId == id))
            {
                return Conflict(
                    "Сначала удалите измерения этой скважины.");
            }

            data.Wells.Remove(well);

            return NoContent();
        }
    }
}