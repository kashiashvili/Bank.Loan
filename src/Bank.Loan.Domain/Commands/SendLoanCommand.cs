using Bank.Loan.Domain.Enums;
using MediatR;

namespace Bank.Loan.Domain.Commands;

public class SendLoanCommand : IRequest<Unit>
{
    public long ID { get; set; }
}