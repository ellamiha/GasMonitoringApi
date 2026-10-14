using Microsoft.AspNetCore.Authorization;
using GasMonitoringApi.Data;
using GasMonitoringApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GasMonitoringApi.Controllers;

[ApiController]
[Route("api/measurements")]
public class MeasurementsController : ControllerBase
{
    private readonly AppData data;

    public MeasurementsController(AppData data)
    {
        this.data = data;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        lock (data.SyncRoot)
        {
            return Ok(data.Measurements.ToList());
        }
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        lock (data.SyncRoot)
        {
            var measurement = data.Measurements
                .FirstOrDefault(m => m.Id == id);

            if (measurement == null)
                return NotFound("Измерение не найдено.");

            return Ok(measurement);
        }
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Create(Measurement measurement)
    {
        lock (data.SyncRoot)
        {
            if (!data.Wells.Any(w => w.Id == measurement.WellId))
                return BadRequest("Указанная скважина не существует.");

            measurement.Id = data.GetNextMeasurementId();
            data.Measurements.Add(measurement);

            return CreatedAtAction(
                nameof(GetById),
                new { id = measurement.Id },
                measurement);
        }
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Measurement updatedMeasurement)
    {
        lock (data.SyncRoot)
        {
            int index = data.Measurements.FindIndex(m => m.Id == id);

            if (index == -1)
                return NotFound("Измерение не найдено.");

            if (!data.Wells.Any(
                    w => w.Id == updatedMeasurement.WellId))
            {
                return BadRequest("Указанная скважина не существует.");
            }

            updatedMeasurement.Id = id;
            data.Measurements[index] = updatedMeasurement;

            return NoContent();
        }
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        lock (data.SyncRoot)
        {
            var measurement = data.Measurements
                .FirstOrDefault(m => m.Id == id);

            if (measurement == null)
                return NotFound("Измерение не найдено.");

            data.Measurements.Remove(measurement);

            return NoContent();
        }
    }
}