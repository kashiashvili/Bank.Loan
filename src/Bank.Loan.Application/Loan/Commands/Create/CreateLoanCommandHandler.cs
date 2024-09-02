using Bank.Loan.Application.Repository;
using Bank.Loan.Domain.Commands;
using Bank.Loan.Domain.Enums;
using FluentValidation;
using MediatR;

namespace Bank.Loan.Application.Loan.Commands.Create;

public class CreateLoanCommandHandler : IRequestHandler<CreateLoanCommand, long>
{
    private readonly IRepository<Domain.Loan> _loanRepository;
    private readonly IValidator<CreateLoanCommand> _validator;

    public CreateLoanCommandHandler(IRepository<Domain.Loan> loanRepository, IValidator<CreateLoanCommand> validator)
    {
        _loanRepository = loanRepository;
        _validator = validator;
    }

    public async Task<long> Handle(CreateLoanCommand request, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            throw new ApplicationException(); //TODO change exception with badrequest status code ex
        }
        
        var loan = new Domain.Loan(request.Type, request.Amount, request.Ccy, request.MonthAmount, LoanStatus.Processing);

        var addedLoan = await _loanRepository.AddAsync(loan, cancellationToken);
        
        return addedLoan.Id;
    }
}