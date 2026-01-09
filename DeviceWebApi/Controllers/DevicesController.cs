using Devices.Application.Commands;
using Devices.Application.DTOs;
using Devices.Application.Interfaces;
using Devices.Domain.Enums;
using DeviceWebApi.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DeviceWebApi.Controllers
{
    [ApiController]
    [Route("devices")]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DevicesController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        /// <summary>
        /// Get all devices.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DeviceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DeviceDto>>> GetAll()
        {
            var result = await _deviceService.GetAllAsync();

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return Ok();
        }

        /// <summary>
        /// Get a device by its id.
        /// </summary>
        /// <param name="id">Device id.</param>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeviceDto>> GetById(int id)
        {
            var result = await _deviceService.GetByIdAsync(id);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound();
        }

        /// <summary>
        /// Get devices by brand.
        /// </summary>
        /// <param name="brand">Device brand.</param>
        [HttpGet("brand/{brand}")]
        [ProducesResponseType(typeof(IEnumerable<DeviceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<DeviceDto>>> GetByBrand(string brand)
        {
            if (string.IsNullOrWhiteSpace(brand))
            {
                return BadRequest("Brand cannot be empty");
            }

            var result = await _deviceService.GetByBrandAsync(brand);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return StatusCode(500, result.Error);
        }

        /// <summary>
        /// Get devices by state.
        /// </summary>
        /// <param name="state">Device state.</param>
        [HttpGet("state/{state}")]
        [ProducesResponseType(typeof(IEnumerable<DeviceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<DeviceDto>>> GetByState(string state)
        {
            if (!Enum.IsDefined(typeof(DeviceState), state))
            {
                var validStates = string.Join(", ", Enum.GetNames(typeof(DeviceState)));
                return BadRequest($"Invalid state. Valid states: {validStates}");
            }

            var deviceState = Enum.Parse<DeviceState>(state, true);
            var result = await _deviceService.GetByStateAsync(deviceState);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return StatusCode(500, result.Error);
        }

        /// <summary>
        /// Creates a new device.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DeviceDto>> Create(CreateDeviceRequest createDto)
        {
            var command = new CreateDeviceCommand(createDto.Name, createDto.Brand, createDto.State);

            var result = await _deviceService.CreateAsync(command);
            if (result.IsSuccess)
            {
                var device = result.Value;
                return CreatedAtAction(nameof(GetById), new { id = device.Id }, device);
            }

            return BadRequest(result.Error);
        }

        /// <summary>
        /// Updates an existing device.
        /// </summary>
        /// <param name="id">Device identifier.</param>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeviceDto>> Update(
            int id,
            [FromBody] UpdateDeviceCommand command)
        {
            var result = await _deviceService.UpdateAsync(id, command);

            if (!result.IsSuccess)
                return result.Error.Contains("not found")
                    ? NotFound(result.Error)
                    : BadRequest(result.Error);

            return Ok(result.Value);
        }

        /// <summary>
        /// Partially updates a device.
        /// </summary>
        /// <param name="id">Device identifier.</param>
        /// <param name="updates">Fields to update.</param>
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeviceDto>> PartialUpdate(
            int id,
            [FromBody] Dictionary<string, object> updates)
        {
            var result = await _deviceService.PartialUpdateAsync(id, updates);

            if (!result.IsSuccess)
                return result.Error.Contains("not found")
                    ? NotFound(result.Error)
                    : BadRequest(result.Error);

            return Ok(result.Value);
        }

        /// <summary>
        /// Deletes a device.
        /// </summary>
        /// <param name="id">Device identifier.</param>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _deviceService.DeleteAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return NoContent();
        }
    }
}