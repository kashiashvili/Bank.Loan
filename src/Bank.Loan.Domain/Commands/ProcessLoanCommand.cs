using Bank.Loan.Domain.Enums;
using MediatR;

namespace Bank.Loan.Domain.Commands;

public class ProcessLoanCommand : IRequest<Unit>
{
    public long ID { get; set; }
    public LoanStatus Status { get; set; }
}