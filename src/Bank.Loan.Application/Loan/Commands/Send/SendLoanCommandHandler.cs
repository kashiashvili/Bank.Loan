using Bank.Loan.Application.Infrastructure;
using Bank.Loan.Application.Repository;
using Bank.Loan.Domain.Commands;
using Bank.Loan.Domain.Enums;
using MediatR;

namespace Bank.Loan.Application.Loan.Commands.Send;

public class SendLoanCommandHandler : IRequestHandler<SendLoanCommand, Unit>
{
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IRepository<Domain.Loan> _loanRepository;

    public SendLoanCommandHandler(IRabbitMqService rabbitMqService, IRepository<Domain.Loan> loanRepository)
    {
        _rabbitMqService = rabbitMqService;
        _loanRepository = loanRepository;
    }
    public async Task<Unit> Handle(SendLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = await _loanRepository.GetByIdAsync(request.ID, cancellationToken);

        _rabbitMqService.SendJsonMessage(loan);
        
        loan.UpdateStatus(LoanStatus.Sent);
        await _loanRepository.UpdateAsync(loan, cancellationToken);
        
        return Unit.Value;
    }
}