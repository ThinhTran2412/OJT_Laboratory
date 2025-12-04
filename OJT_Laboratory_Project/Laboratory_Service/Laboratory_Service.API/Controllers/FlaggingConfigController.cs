using Laboratory_Service.Application.FlaggingConfigs.Commands;
using Laboratory_Service.Application.FlaggingConfigs.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Laboratory_Service.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlaggingConfigController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FlaggingConfigController> _logger;

    public FlaggingConfigController(IMediator mediator, ILogger<FlaggingConfigController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Đồng bộ cấu hình flagging từ nguồn bên ngoài vào hệ thống.
    /// </summary>
    [HttpPost("sync")]
    public async Task<IActionResult> SyncFlaggingConfigs([FromBody] SyncFlaggingConfigCommand command)
    {
        if (command == null || command.Configs == null || command.Configs.Count == 0)
        {
            return BadRequest(new { Message = "Request payload is empty." });
        }

        var result = await _mediator.Send(command);

        _logger.LogInformation("Flagging config sync requested. Result: {Message}", result.Message);

        return Ok(result);
    }
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var configs = await _mediator.Send(new GetAllFlaggingConfigsQuery());
        return Ok(configs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var config = await _mediator.Send(new GetFlaggingConfigByIdQuery(id));
        if (config == null) return NotFound();
        return Ok(config);
    }
}

