using Bank.Loan.Domain.Enums;
using FluentValidation;
using MediatR;

namespace Bank.Loan.Domain.Commands;

public class CreateLoanCommand : IRequest<long> 
{
    public LoanType Type { get; set; } //TODO add stringenum converter for swagger
    public decimal Amount { get; set; }
    public required string Ccy { get; set; }
    public int MonthAmount { get; set; }
}

public class CreateLoanCommandValidator : AbstractValidator<CreateLoanCommand> //TODO add validators to other models too
{
    public CreateLoanCommandValidator() 
    {
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(300);
        RuleFor(x => x.Ccy).Matches("^[A-Z]{3}$");
        RuleFor(x => x.MonthAmount).InclusiveBetween(3, 48);
    }
}