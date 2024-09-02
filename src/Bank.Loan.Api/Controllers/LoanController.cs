using Bank.Loan.Application.Loan.Queries.LoanForProcessing;
using Bank.Loan.Domain;
using Bank.Loan.Domain.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Loan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LoanController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoanController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLoanCommand request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(request, cancellationToken);
        
        return StatusCode(201, id);
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateLoanCommand request, CancellationToken cancellationToken)
    {
        await _mediator.Send(request, cancellationToken);
        
        return NoContent();
    }
    
    [HttpPost("[action]/{id:int}")]
    public async Task<IActionResult> Send([FromRoute] int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new SendLoanCommand { ID = id }, cancellationToken);
        
        return NoContent();
    }
    
    [HttpPost("[action]")]
    public async Task<IActionResult> CreateAndSend([FromBody] CreateLoanCommand request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(request, cancellationToken);
        
        await _mediator.Send(new SendLoanCommand { ID = id }, cancellationToken);
        
        return NoContent();
    }
    
    [HttpPut("[action]")]
    public async Task<IActionResult> UpdateAndSend([FromBody] UpdateLoanCommand request, CancellationToken cancellationToken)
    {
        await _mediator.Send(request, cancellationToken);
        
        await _mediator.Send(new SendLoanCommand { ID = request.ID }, cancellationToken);
        
        return NoContent();
    }
    
    [HttpGet("ForProcessing")]
    [Authorize(Roles = "Admin")] //TODO: commonize userroles and use it here
    public async Task<IActionResult> GetLoanForProcessing(CancellationToken cancellationToken)
    {
        var loan = await _mediator.Send(new LoanForProcessingQuery(), cancellationToken);
        
        return Ok(loan);
    }
    
    [HttpPut("[action]")]
    [Authorize(Roles = "Admin")] //TODO: commonize userroles and use it here
    public async Task<IActionResult> Process(ProcessLoanCommand processLoanCommand, CancellationToken cancellationToken)
    {
       await _mediator.Send(processLoanCommand, cancellationToken);
        
        return NoContent();
    }
}