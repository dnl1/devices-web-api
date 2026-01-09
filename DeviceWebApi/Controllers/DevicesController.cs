using Devices.Application.Commands;
using Devices.Application.DTOs;
using Devices.Application.Interfaces;
using DeviceWebApi.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DeviceWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DevicesController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceDto>>> GetAll()
        {
            var result = await _deviceService.GetAllDevicesAsync();

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return Ok();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DeviceDto>> GetById(int id)
        {
            var result = await _deviceService.GetDeviceAsync(id);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound();
        }

        [HttpPost]
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
    }
}