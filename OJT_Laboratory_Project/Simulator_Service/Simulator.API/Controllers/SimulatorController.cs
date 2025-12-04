using MediatR;
using Microsoft.AspNetCore.Mvc;
using Simulator.Application.SimulateRawData.Command;

[ApiController]
[Route("api/[controller]")]
public class SimulatorController : ControllerBase
{
    private readonly IMediator _mediator;

    public SimulatorController(IMediator mediator)
    {
        _mediator = mediator;
    }

    //[HttpPost("send-test-result")]
    //public async Task<IActionResult> SendTestResult()
    //{
    //    // Giả sử Counter = 999 để test
    //    var rawResult = await _mediator.Send(new SimulateRawDataCommand(999));
    //    var result = await _mediator.Send(new SendRawTestResultCommand(rawResult));
    //    return Ok(result);
    //}
}
