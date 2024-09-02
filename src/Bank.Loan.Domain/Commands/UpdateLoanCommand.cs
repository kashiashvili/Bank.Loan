using Bank.Loan.Domain.Enums;
using MediatR;

namespace Bank.Loan.Domain.Commands;

public class UpdateLoanCommand : IRequest<Unit>
{
    public int ID { get; set; }
    public LoanType Type { get; set; }
    public decimal Amount { get; set; }
    public required string Ccy { get; set; }
    public int MonthAmount { get; set; }
}