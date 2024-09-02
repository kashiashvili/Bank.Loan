using Bank.Loan.Application.Infrastructure;
using Bank.Loan.Application.Repository;
using Bank.Loan.Domain.Commands;
using Bank.Loan.Domain.Enums;
using MediatR;

namespace Bank.Loan.Application.Loan.Commands.Process;

public class ProcessLoanCommandHandler : IRequestHandler<ProcessLoanCommand, Unit>
{
    private readonly IPendingLoanService _pendingLoanService;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IRepository<Domain.Loan> _loanRepository;

    public ProcessLoanCommandHandler(IPendingLoanService pendingLoanService, IRabbitMqService rabbitMqService, IRepository<Domain.Loan> loanRepository)
    {
        _pendingLoanService = pendingLoanService;
        _rabbitMqService = rabbitMqService;
        _loanRepository = loanRepository;
    }

    public async Task<Unit> Handle(ProcessLoanCommand request, CancellationToken cancellationToken)
    {
        if (!_pendingLoanService.TryRemovePendingLoan(out var deliveryTag, request.ID)) //TODO must be atomic handler
        {
            throw new KeyNotFoundException("Loan not found."); //TODO change exception
        }

        if (request.Status == LoanStatus.Processing || request.Status == LoanStatus.Sent)
        {
            throw new ApplicationException(); //TODO change exception
        }

        var loan = await _loanRepository.GetByIdAsync(request.ID, cancellationToken);
        loan.UpdateStatus(request.Status); 
        await _loanRepository.UpdateAsync(loan, cancellationToken);
        _rabbitMqService.AcknowledgeMessage(deliveryTag);

        return Unit.Value;
    }
}