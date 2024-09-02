using System.Text.Json;
using Bank.Loan.Application.Infrastructure;
using MediatR;

namespace Bank.Loan.Application.Loan.Queries.LoanForProcessing;

public class LoanForProcessingQuery : IRequest<Domain.Loan>
{
}

public class LoanForProcessingQueryHandler : IRequestHandler<LoanForProcessingQuery, Domain.Loan>
{
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IPendingLoanService _pendingLoanService;

    public LoanForProcessingQueryHandler(IRabbitMqService rabbitMqService, IPendingLoanService pendingLoanService)
    {
        _rabbitMqService = rabbitMqService;
        _pendingLoanService = pendingLoanService;
    }

    public async Task<Domain.Loan> Handle(LoanForProcessingQuery request, CancellationToken cancellationToken)
    {
        var (loan, deliveryTag) = _rabbitMqService.FetchJsonMessage<Domain.Loan>();
        if (loan == null)
        {
            throw new ApplicationException(); //TODO change exception
        }
        
        _pendingLoanService.AddPendingLoan(deliveryTag, loan.Id);

        return loan;
    }
}